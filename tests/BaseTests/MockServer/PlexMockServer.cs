﻿using PlexRipper.PlexApi.Common;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace PlexRipper.BaseTests;

/// <summary>
/// Used to mock a individual Plex server, this is not the same as the PlexApi which is a central server
/// Source: https://github.com/WireMock-Net/WireMock.Net/
/// </summary>
public class PlexMockServer : IDisposable
{
    private readonly PlexMockServerConfig _config;
    private readonly Action<PlexApiDataConfig> _fakeDataConfig;

    #region Constructor

    public PlexMockServer(Action<PlexMockServerConfig> options = null) : this(PlexMockServerConfig.FromOptions(options)) { }

    public PlexMockServer(PlexMockServerConfig options = null)
    {
        _config = options;
        _fakeDataConfig = _config?.FakeDataConfig;

        Server = WireMockServer.Start(new WireMockServerSettings()
        {
            ThrowExceptionWhenMatcherFails = true,
        });

        ServerUri = new Uri(Server.Urls[0]);
        DownloadUri = new Uri($"{Server.Urls[0]}{PlexMockServerConfig.FileUrl}");
        Server = Setup();
        Log.Debug($"Created {nameof(PlexMockServer)} with url: {ServerUri}");
    }

    #endregion

    #region Properties

    public WireMockServer Server { get; }

    public Uri DownloadUri { get; }

    public Uri ServerUri { get; }

    public long DownloadFileSizeInBytes => _config.DownloadFileSizeInMb * 1024;

    public bool IsStarted => Server.IsStarted;

    #endregion

    #region Public Methods

    private WireMockServer Setup()
    {
        SetupServerIdentity();

        // Setup the Plex libraries
        var librarySections = FakePlexApiData.GetLibraryMediaContainer(_fakeDataConfig);

        Server
            .Given(Request.Create().WithPath(PlexApiPaths.LibrarySectionsPath).WithParam("X-Plex-Token").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(librarySections));

        // Setup the media metadata for each library
        foreach (var librarySection in librarySections.MediaContainer.Directory)
        {
            var libraryData = FakePlexApiData.GetPlexLibrarySectionAllResponse(librarySection, _fakeDataConfig);
            var url = PlexApiPaths.GetLibrariesSectionsPath(librarySection.Key);
            Log.Debug($"Url registered: {url}");
            Server
                .Given(Request.Create().WithPath(url).WithParam("X-Plex-Token").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(libraryData));
        }

        SetupDownloadableFile();
        return Server;
    }

    private void SetupServerIdentity()
    {
        Server
            .Given(Request.Create().WithPath(PlexApiPaths.ServerIdentityPath).UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(FakePlexApiData.GetPlexServerIdentityResponse(_fakeDataConfig)));
    }

    private void SetupDownloadableFile()
    {
        if (_config.DownloadFileSizeInMb > 0)
        {
            var downloadFile = FakeData.GetDownloadFile(_config.DownloadFileSizeInMb);

            Server
                .Given(Request.Create().WithPath(PlexMockServerConfig.FileUrl).WithParam("X-Plex-Token").UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(206)
                        .WithBody(downloadFile)
                );
        }
    }

    #endregion

    public void Dispose()
    {
        Server.Stop();
        Server?.Dispose();
    }
}