﻿using System.Collections.Generic;
using Bogus;
using Environment;
using PlexRipper.Domain;

namespace PlexRipper.BaseTests
{
    public static partial class FakeData
    {
        public static Faker<T> ApplyBaseDownloadTask<T>(this Faker<T> faker, UnitTestDataConfig config = null) where T : DownloadTask
        {
            config ??= new UnitTestDataConfig();

            return faker
                .StrictMode(true)
                .UseSeed(config.Seed)
                .RuleFor(x => x.Id, _ => 0)
                .RuleFor(x => x.DownloadStatus, _ => DownloadStatus.Queued)
                .RuleFor(x => x.Title, f => f.Company.CompanyName())
                .RuleFor(x => x.FullTitle, f => f.Company.CompanyName())
                .RuleFor(x => x.Year, f => f.Random.Int(1900, 2030))
                .RuleFor(x => x.DownloadTaskType, _ => DownloadTaskType.Movie)
                .RuleFor(x => x.Priority, _ => 0)
                .RuleFor(x => x.DataReceived, _ => 0)
                .RuleFor(x => x.DataTotal, f => f.Random.Long(1, 10000000))
                .RuleFor(x => x.Percentage, _ => 0)
                .RuleFor(x => x.DownloadSpeed, _ => 0)
                .RuleFor(x => x.DownloadWorkerTasks, _ => new())
                .RuleFor(x => x.FileName, f => f.System.FileName() + ".mp4")
                .RuleFor(x => x.FileLocationUrl, f => f.System.FilePath())
                .RuleFor(x => x.DownloadDirectory, f => f.System.FilePath())
                .RuleFor(x => x.DestinationDirectory, f => f.System.FilePath())
                .RuleFor(x => x.ParentId, _ => null)
                .RuleFor(x => x.Parent, _ => null)
                .RuleFor(x => x.Key, _ => _random.Next(1, 10000))
                .RuleFor(x => x.Created, f => f.Date.Recent(30))
                .RuleFor(x => x.Quality, f => f.PickRandom("sd", "720", "1080"))
                .RuleFor(x => x.PlexServerId, _ => 0)
                .RuleFor(x => x.PlexServer, _ => null)
                .RuleFor(x => x.PlexLibraryId, _ => 0)
                .RuleFor(x => x.PlexLibrary, _ => null)
                .RuleFor(x => x.Children, _ => new List<DownloadTask>())
                .RuleFor(x => x.DownloadFolder,  _ => null)
                .RuleFor(x => x.DownloadFolderId, _ => 1)
                .RuleFor(x => x.DestinationFolder,  _ => null)
                .RuleFor(x => x.DestinationFolderId, _ => 2)
                .RuleFor(x => x.RootDownloadTask,  _ => null)
                .RuleFor(x => x.RootDownloadTaskId, _ => null);
        }

        #region Movie

        public static Faker<DownloadTask> GetMovieDownloadTask(UnitTestDataConfig config = null)
        {
            config ??= new UnitTestDataConfig();

            return new Faker<DownloadTask>()
                .ApplyBaseDownloadTask(config)
                .UseSeed(config.Seed)
                .RuleFor(x => x.MediaType, PlexMediaType.Movie)
                .RuleFor(x => x.DownloadTaskType, _ => DownloadTaskType.Movie)
                .RuleFor(x => x.Children, _ => GetMovieDataDownloadTask(config).Generate(1))
                .RuleFor(x => x.DownloadUrl, f => f.Internet.Url())
                .RuleFor(x => x.DownloadFolderId, _ => 1)
                .RuleFor(x => x.DestinationFolderId, _ => 2)
                .FinishWith((_, downloadTask) =>
                {
                    downloadTask.Children.ForEach(x =>
                    {
                        x.Parent = x;
                        x.ParentId = x.Id;
                    });
                });
        }

        public static Faker<DownloadTask> GetMovieDataDownloadTask(UnitTestDataConfig config = null)
        {
            config ??= new UnitTestDataConfig();

            return new Faker<DownloadTask>()
                .ApplyBaseDownloadTask(config)
                .UseSeed(config.Seed)
                .RuleFor(x => x.MediaType, PlexMediaType.Movie)
                .RuleFor(x => x.DownloadTaskType, _ => DownloadTaskType.Movie)
                .RuleFor(x => x.DownloadUrl, f => f.Internet.Url());
        }

        #endregion

        #region TvShow

        public static Faker<DownloadTask> GetTvShowDownloadTask(UnitTestDataConfig config = null)
        {
            config ??= new UnitTestDataConfig();

            return new Faker<DownloadTask>()
                .ApplyBaseDownloadTask(config)
                .UseSeed(config.Seed)
                .RuleFor(x => x.MediaType, PlexMediaType.TvShow)
                .RuleFor(x => x.DownloadTaskType, _ => DownloadTaskType.TvShow)
                .RuleFor(x => x.DownloadUrl, _ => "")
                .RuleFor(x => x.Children, _ => GetTvShowSeasonDownloadTask(config).Generate(config.TvShowSeasonCountMax))
                .FinishWith((_, downloadTask) =>
                {
                    downloadTask.Children.ForEach(x =>
                    {
                        x.Parent = x;
                        x.ParentId = x.Id;
                    });
                });
        }

        public static Faker<DownloadTask> GetTvShowSeasonDownloadTask(UnitTestDataConfig config = null)
        {
            config ??= new UnitTestDataConfig();

            return new Faker<DownloadTask>()
                .ApplyBaseDownloadTask(config)
                .UseSeed(config.Seed)
                .RuleFor(x => x.MediaType, PlexMediaType.Season)
                .RuleFor(x => x.DownloadTaskType, _ => DownloadTaskType.Season)
                .RuleFor(x => x.DownloadUrl, _ => "")
                .RuleFor(x => x.Children, _ => GetTvShowEpisodeDownloadTask(config).Generate(config.TvShowEpisodeCountMax))
                .FinishWith((_, downloadTask) =>
                {
                    downloadTask.Children.ForEach(x =>
                    {
                        x.Parent = x;
                        x.ParentId = x.Id;
                    });
                });
        }

        public static Faker<DownloadTask> GetTvShowEpisodeDownloadTask(UnitTestDataConfig config = null)
        {
            config ??= new UnitTestDataConfig();

            return new Faker<DownloadTask>()
                .ApplyBaseDownloadTask(config)
                .UseSeed(config.Seed)
                .RuleFor(x => x.MediaType, PlexMediaType.Episode)
                .RuleFor(x => x.DownloadTaskType, _ => DownloadTaskType.Episode)
                .RuleFor(x => x.DownloadUrl, f => f.Internet.Url())
                .RuleFor(x => x.Children, _ => GetTvShowEpisodeDataDownloadTask(config).Generate(1))
                .FinishWith((_, downloadTask) =>
                {
                    downloadTask.Children.ForEach(x =>
                    {
                        x.Parent = x;
                        x.ParentId = x.Id;
                    });
                });
        }

        public static Faker<DownloadTask> GetTvShowEpisodeDataDownloadTask(UnitTestDataConfig config = null)
        {
            config ??= new UnitTestDataConfig();

            return new Faker<DownloadTask>()
                .ApplyBaseDownloadTask(config)
                .UseSeed(config.Seed)
                .RuleFor(x => x.MediaType, PlexMediaType.Episode)
                .RuleFor(x => x.DownloadUrl, f => f.Internet.Url())
                .RuleFor(x => x.DownloadTaskType, _ => DownloadTaskType.Episode);
        }

        #endregion

        #region DownloadWorkerTasks

        public static Faker<DownloadWorkerTask> GetDownloadWorkerTask(UnitTestDataConfig config = null)
        {
            config ??= new UnitTestDataConfig();

            var partIndex = 1;
            return new Faker<DownloadWorkerTask>()
                .StrictMode(true)
                .UseSeed(config.Seed)
                .RuleFor(x => x.Id, _ => 0)
                .RuleFor(x => x.FileName, f => f.System.FileName() + ".mp4")
                .RuleFor(x => x.StartByte, f => f.Random.Long(0))
                .RuleFor(x => x.EndByte, f => f.Random.Long(0))
                .RuleFor(x => x.BytesReceived, 0)
                .RuleFor(x => x.PartIndex, _ => partIndex++)
                .RuleFor(x => x.TempDirectory, f => f.System.FilePath())
                .RuleFor(x => x.ElapsedTime, 0)
                .RuleFor(x => x.DownloadUrl, f => f.System.FilePath())
                .RuleFor(x => x.DownloadStatus, DownloadStatus.Queued)
                .RuleFor(x => x.DownloadTaskId, _ => 0)
                .RuleFor(x => x.DownloadTask, _ => null)
                .RuleFor(x => x.DownloadWorkerTaskLogs, new List<DownloadWorkerLog>());
        }

        #endregion
    }
}