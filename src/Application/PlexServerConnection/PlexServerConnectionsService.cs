using Application.Contracts;
using AutoMapper;
using Data.Contracts;
using PlexApi.Contracts;
using WebAPI.Contracts;

namespace PlexRipper.Application;

public class PlexServerConnectionsService : IPlexServerConnectionsService
{
    #region Fields

    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IPlexApiService _plexApiService;
    private readonly ISignalRService _signalRService;

    #endregion

    #region Constructors

    public PlexServerConnectionsService(IMediator mediator, IMapper mapper, ISignalRService signalRService, IPlexApiService plexApiService)
    {
        _mediator = mediator;
        _mapper = mapper;
        _signalRService = signalRService;
        _plexApiService = plexApiService;
    }

    #endregion

    #region Methods

    #region Public

    /// <inheritdoc />
    public async Task<Result<List<PlexServerStatus>>> CheckAllConnectionsOfPlexServerAsync(int plexServerId)
    {
        var plexServerResult = await _mediator.Send(new GetPlexServerByIdQuery(plexServerId, true));
        if (plexServerResult.IsFailed)
            return plexServerResult.ToResult();

        // Create connection check tasks for all connections
        var connectionTasks = plexServerResult.Value
            .PlexServerConnections
            .Select(async plexServerConnection => await CheckPlexServerConnectionStatusAsync(plexServerConnection));

        var tasksResult = await Task.WhenAll(connectionTasks);
        var x = Result.Merge(tasksResult);

        if (tasksResult.Any(statusResult => statusResult.Value.IsSuccessful))
            return Result.Ok(x.Value.ToList());

        return Result.Fail($"All connections to plex server with id: {plexServerId} failed to connect").LogError();
    }

    public async Task<Result> CheckAllConnectionsOfPlexServersByAccountIdAsync(int plexAccountId)
    {
        var plexAccountResult = await _mediator.Send(new GetPlexAccountByIdQuery(plexAccountId, true));
        if (plexAccountResult.IsFailed)
        {
            return plexAccountResult
                .WithError($"Could not retrieve any PlexAccount from database with id {plexAccountId}.")
                .LogError();
        }

        var plexServers = plexAccountResult.Value.PlexServers;

        var serverTasks = plexServers.Select(async plexServer => await CheckAllConnectionsOfPlexServerAsync(plexServer.Id));

        var tasksResult = await Task.WhenAll(serverTasks);
        return Result.OkIf(tasksResult.Any(x => x.IsSuccess),
                $"None of the servers that were checked for {nameof(PlexAccount)} with id {plexAccountId} were connectable")
            .LogIfFailed();
    }

    public async Task<Result<PlexServerStatus>> CheckPlexServerConnectionStatusAsync(PlexServerConnection plexServerConnection, bool trimEntries = true)
    {
        // The call-back action from the httpClient
        async void Action(PlexApiClientProgress progress)
        {
            var checkStatusProgress = _mapper.Map<ServerConnectionCheckStatusProgress>(progress);
            checkStatusProgress.PlexServerConnection = plexServerConnection;
            await _signalRService.SendServerConnectionCheckStatusProgressAsync(checkStatusProgress);
        }

        // Request status
        var serverStatusResult = await _plexApiService.GetPlexServerStatusAsync(plexServerConnection.Id, Action);
        if (serverStatusResult.IsFailed)
            return serverStatusResult.LogError();

        // Add plexServer status to DB, the PlexServerStatus table functions as a server log.
        var result = await _mediator.Send(new CreatePlexServerStatusCommand(serverStatusResult.Value));
        if (result.IsFailed)
            return result.ToResult();

        if (trimEntries)
        {
            // Ensure that there are not too many PlexServerStatuses stored.
            var trimResult = await _mediator.Send(new TrimPlexServerStatusCommand(serverStatusResult.Value.PlexServerId));
            if (trimResult.IsFailed)
                return trimResult.ToResult();
        }

        return serverStatusResult.Value;
    }

    #endregion

    #endregion

    #region CRUD

    public Task<Result<PlexServerConnection>> GetPlexServerConnectionAsync(int plexServerConnectionId)
    {
        return _mediator.Send(new GetPlexServerConnectionByIdQuery(plexServerConnectionId));
    }

    public async Task<Result<List<PlexServerConnection>>> GetAllPlexServerConnectionsAsync()
    {
        return await _mediator.Send(new GetAllPlexServerConnectionsQuery());
    }

    #endregion
}