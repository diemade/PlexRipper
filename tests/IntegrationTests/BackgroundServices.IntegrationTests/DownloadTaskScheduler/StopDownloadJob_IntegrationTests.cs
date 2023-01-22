using System.Linq;
using System.Threading.Tasks;
using PlexRipper.Data.Common;

namespace BackgroundServices.IntegrationTests.DownloadTaskScheduler;

[Collection("Sequential")]
public class StopDownloadJob_IntegrationTests : BaseIntegrationTests
{
    public StopDownloadJob_IntegrationTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task ShouldStartAndStopDownloadJob_WhenDownloadTaskHasBeenStopped()
    {
        // Arrange
        Seed = 45644875;
        var serverUri = SpinUpPlexServer(config => { config.DownloadFileSizeInMb = 50; });
        await SetupDatabase(config =>
        {
            config.MockServerUris.Add(serverUri);
            config.PlexServerCount = 1;
            config.PlexLibraryCount = 3;
            config.MovieCount = 10;
            config.MovieDownloadTasksCount = 5;
            config.DownloadFileSizeInMb = 50;
        });

        SetupMockPlexApi();

        await CreateContainer(config => { config.DownloadSpeedLimitInKib = 5000; });

        var downloadTask = DbContext
            .DownloadTasks
            .IncludeDownloadTasks()
            .FirstOrDefault();
        downloadTask.ShouldNotBeNull();
        var childDownloadTask = downloadTask.Children[0];

        // TODO fix the assertion that the DownloadTaskHasFinished
        // Container.Mediator. DownloadTaskTracker.DownloadTaskFinished.Subscribe(value =>
        //     value.Id.ShouldBe(childDownloadTask.Id)
        // );

        // Act
        var startResult = await Container.DownloadTaskScheduler.StartDownloadTaskJob(childDownloadTask.Id, childDownloadTask.PlexServerId);
        await Task.Delay(2000);
        var stopResult = await Container.DownloadTaskScheduler.StopDownloadTaskJob(childDownloadTask.Id);
        await Container.SchedulerService.AwaitScheduler();

        // Assert
        startResult.IsSuccess.ShouldBeTrue();
        stopResult.IsSuccess.ShouldBeTrue();
        var downloadTaskDb = DbContext.DownloadTasks
            .IncludeDownloadTasks()
            .FirstOrDefault(x => x.Id == childDownloadTask.Id);
        downloadTaskDb.ShouldNotBeNull();
        downloadTaskDb.DownloadStatus.ShouldBe(DownloadStatus.Stopped);
    }
}