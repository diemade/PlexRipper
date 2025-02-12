﻿using Data.Contracts;
using FluentValidation;
using Logging.Interface;
using Microsoft.EntityFrameworkCore;
using PlexRipper.Data.Common;

namespace PlexRipper.Data.PlexTvShows;

public class GetPlexTvShowByIdWithEpisodesQueryValidator : AbstractValidator<GetPlexTvShowByIdWithEpisodesQuery>
{
    public GetPlexTvShowByIdWithEpisodesQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetPlexTvShowByIdWithEpisodesQueryHandler : BaseHandler, IRequestHandler<GetPlexTvShowByIdWithEpisodesQuery, Result<PlexTvShow>>
{
    public GetPlexTvShowByIdWithEpisodesQueryHandler(ILog log, PlexRipperDbContext dbContext) : base(log, dbContext) { }

    public async Task<Result<PlexTvShow>> Handle(GetPlexTvShowByIdWithEpisodesQuery request, CancellationToken cancellationToken)
    {
        var query = PlexTvShowsQueryable.IncludeEpisodes();

        if (request.IncludeLibrary)
            query = query.IncludePlexLibrary();

        if (request.IncludePlexServer)
            query = query.IncludePlexServer();

        var plexTvShow = await query.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (plexTvShow == null)
            return ResultExtensions.EntityNotFound(nameof(PlexTvShow), request.Id);

        plexTvShow.Seasons = plexTvShow.Seasons.OrderByNatural(x => x.Title).ToList();

        return Result.Ok(plexTvShow);
    }
}