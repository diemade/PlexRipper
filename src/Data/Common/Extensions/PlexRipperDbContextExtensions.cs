﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using PlexRipper.Domain;

namespace PlexRipper.Data.Common
{
    public static class PlexRipperDbContextExtensions
    {
        public static IQueryable<PlexLibrary> IncludeTvShows(this IQueryable<PlexLibrary> plexLibrary, bool topLevelOnly = false)
        {
            if (topLevelOnly)
            {
                return plexLibrary
                    .Include(x => x.TvShows);
            }

            return plexLibrary
                .Include(x => x.TvShows)
                .ThenInclude(x => x.Seasons)
                .ThenInclude(x => x.Episodes);
        }

        public static IQueryable<PlexLibrary> IncludeMovies(this IQueryable<PlexLibrary> plexLibrary)
        {
            return plexLibrary
                .Include(x => x.Movies);
        }

        #region PlexLibrary

        public static IQueryable<PlexLibrary> IncludeServer(this IQueryable<PlexLibrary> plexLibrary)
        {
            return plexLibrary.Include(x => x.PlexServer);
        }

        #endregion

        #region PlexMovie

        public static IQueryable<PlexMovie> IncludePlexLibrary(this IQueryable<PlexMovie> plexMovies)
        {
            return plexMovies.Include(x => x.PlexLibrary);
        }

        public static IQueryable<PlexMovie> IncludeServer(this IQueryable<PlexMovie> plexMovies)
        {
            return plexMovies.Include(x => x.PlexServer);
        }

        #endregion

        #region PlexTvShow

        public static IQueryable<PlexTvShow> IncludePlexLibrary(this IQueryable<PlexTvShow> plexTvShows)
        {
            return plexTvShows.Include(x => x.PlexLibrary);
        }

        public static IQueryable<PlexTvShow> IncludeServer(this IQueryable<PlexTvShow> plexTvShows)
        {
            return plexTvShows.Include(x => x.PlexServer);
        }

        #endregion

        #region PlexTvShow

        public static IQueryable<PlexTvShowSeason> IncludePlexLibrary(this IQueryable<PlexTvShowSeason> plexTvShowSeason)
        {
            return plexTvShowSeason.Include(x => x.PlexLibrary);
        }

        public static IQueryable<PlexTvShowSeason> IncludeServer(this IQueryable<PlexTvShowSeason> plexTvShowSeason)
        {
            return plexTvShowSeason.Include(x => x.PlexServer);
        }

        #endregion

        #region PlexTvShow

        public static IQueryable<PlexTvShowEpisode> IncludePlexLibrary(this IQueryable<PlexTvShowEpisode> plexTvShowEpisode)
        {
            return plexTvShowEpisode.Include(x => x.PlexLibrary);
        }

        public static IQueryable<PlexTvShowEpisode> IncludeServer(this IQueryable<PlexTvShowEpisode> plexTvShowEpisode)
        {
            return plexTvShowEpisode.Include(x => x.PlexServer);
        }

        #endregion
    }
}