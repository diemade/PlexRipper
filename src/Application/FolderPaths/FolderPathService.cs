﻿using Application.Contracts;
using Data.Contracts;
using FileSystem.Contracts;

namespace PlexRipper.Application;

public class FolderPathService : IFolderPathService
{
    private readonly IMediator _mediator;

    private readonly IDirectorySystem _directorySystem;

    public FolderPathService(IMediator mediator, IDirectorySystem directorySystem)
    {
        _mediator = mediator;
        _directorySystem = directorySystem;
    }

    public Task<Result<List<FolderPath>>> GetAllFolderPathsAsync()
    {
        return _mediator.Send(new GetAllFolderPathsQuery());
    }

    public async Task<Result<FolderPath>> CreateFolderPath(FolderPath folderPath)
    {
        var folderPathId = await _mediator.Send(new CreateFolderPathCommand(folderPath));
        if (folderPathId.IsFailed)
            return folderPathId.ToResult();

        return await _mediator.Send(new GetFolderPathByIdQuery(folderPathId.Value));
    }

    public Task<Result<FolderPath>> UpdateFolderPathAsync(FolderPath folderPath)
    {
        return _mediator.Send(new UpdateFolderPathCommand(folderPath));
    }

    public Task<Result> DeleteFolderPathAsync(int folderPathId)
    {
        return _mediator.Send(new DeleteFolderPathCommand(folderPathId));
    }

    /// <inheritdoc/>
    public async Task<Result<FolderPath>> GetDownloadFolderAsync()
    {
        return await _mediator.Send(new GetFolderPathByIdQuery(1));
    }

    /// <inheritdoc/>
    public async Task<Result<FolderPath>> GetMovieDestinationFolderAsync()
    {
        return await _mediator.Send(new GetFolderPathByIdQuery(2));
    }

    /// <inheritdoc/>
    public async Task<Result<FolderPath>> GetTvShowDestinationFolderAsync()
    {
        return await _mediator.Send(new GetFolderPathByIdQuery(3));
    }

    public async Task<Result<FolderPath>> GetDestinationFolderByMediaType(PlexMediaType mediaType)
    {
        switch (mediaType)
        {
            case PlexMediaType.Movie:
                return await GetMovieDestinationFolderAsync();
            case PlexMediaType.TvShow:
            case PlexMediaType.Season:
            case PlexMediaType.Episode:
                return await GetTvShowDestinationFolderAsync();
            default:
                return await GetDownloadFolderAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<Result<Dictionary<PlexMediaType, FolderPath>>> GetDefaultDestinationFolderDictionary()
    {
        var folderPaths = await _mediator.Send(new GetAllFolderPathsQuery());
        if (folderPaths.IsFailed)
            return folderPaths.ToResult();

        var dict = new Dictionary<PlexMediaType, FolderPath>
        {
            { PlexMediaType.Movie, folderPaths.Value.FirstOrDefault(x => x.Id == 2) },
            { PlexMediaType.TvShow, folderPaths.Value.FirstOrDefault(x => x.Id == 3) },
            { PlexMediaType.Season, folderPaths.Value.FirstOrDefault(x => x.Id == 3) },
            { PlexMediaType.Episode, folderPaths.Value.FirstOrDefault(x => x.Id == 3) },
            { PlexMediaType.Music, folderPaths.Value.FirstOrDefault(x => x.Id == 4) },
            { PlexMediaType.Album, folderPaths.Value.FirstOrDefault(x => x.Id == 4) },
            { PlexMediaType.Song, folderPaths.Value.FirstOrDefault(x => x.Id == 4) },
            { PlexMediaType.Photos, folderPaths.Value.FirstOrDefault(x => x.Id == 5) },
            { PlexMediaType.OtherVideos, folderPaths.Value.FirstOrDefault(x => x.Id == 6) },
            { PlexMediaType.Games, folderPaths.Value.FirstOrDefault(x => x.Id == 7) },
            { PlexMediaType.None, folderPaths.Value.FirstOrDefault(x => x.Id == 1) },
            { PlexMediaType.Unknown, folderPaths.Value.FirstOrDefault(x => x.Id == 1) },
        };

        return Result.Ok(dict);
    }

    public async Task<Result> CheckIfFolderPathsAreValid(PlexMediaType mediaType = PlexMediaType.None)
    {
        var folderPaths = await GetAllFolderPathsAsync();
        if (folderPaths.IsFailed)
            return folderPaths.ToResult();

        if (mediaType is PlexMediaType.None or PlexMediaType.Unknown)
            return folderPaths.ToResult();

        var errors = new List<IError>();
        foreach (var folderPath in folderPaths.Value)
        {
            var folderPathExitsResult = _directorySystem.Exists(folderPath.DirectoryPath);
            if (folderPathExitsResult.IsFailed)
            {
                errors.AddRange(folderPathExitsResult.Errors);
                continue;
            }

            if (folderPath.MediaType == mediaType && !folderPathExitsResult.Value)
                errors.Add(new Error($"The {folderPath.DisplayName} is not a valid or existing directory"));
        }

        return errors.Count > 0 ? new Result().WithErrors(errors).LogError() : Result.Ok();
    }
}