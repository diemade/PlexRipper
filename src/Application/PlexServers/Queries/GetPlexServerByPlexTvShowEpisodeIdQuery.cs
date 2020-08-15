﻿using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PlexRipper.Application.Common.Interfaces.DataAccess;
using PlexRipper.Domain.Base;
using PlexRipper.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlexRipper.Domain;

namespace PlexRipper.Application.PlexServers.Queries
{
    public class GetPlexServerByPlexTvShowEpisodeIdQuery : IRequest<Result<PlexServer>>
    {
        public GetPlexServerByPlexTvShowEpisodeIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }

    public class GetPlexServerByPlexTvShowEpisodeIdQueryValidator : AbstractValidator<GetPlexServerByPlexTvShowEpisodeIdQuery>
    {
        public GetPlexServerByPlexTvShowEpisodeIdQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }


    public class GetPlexServerByPlexTvShowEpisodeIdQueryHandler : BaseHandler,
        IRequestHandler<GetPlexServerByPlexTvShowEpisodeIdQuery, Result<PlexServer>>
    {
        private readonly IPlexRipperDbContext _dbContext;

        public GetPlexServerByPlexTvShowEpisodeIdQueryHandler(IPlexRipperDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<PlexServer>> Handle(GetPlexServerByPlexTvShowEpisodeIdQuery request,
            CancellationToken cancellationToken)
        {
            var plexTvShowEpisode = await _dbContext.PlexTvShowEpisodes
                .Include(x => x.TvShowSeason)
                .ThenInclude(x => x.TvShow)
                .ThenInclude(x => x.PlexLibrary)
                .ThenInclude(x => x.PlexServer)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (plexTvShowEpisode == null)
            {
                return ResultExtensions.Get404NotFoundResult();
            }

            var plexServer = plexTvShowEpisode?.TvShowSeason?.TvShow?.PlexLibrary?.PlexServer;
            if (plexServer == null)
            {
                return Result.Fail($"Could not retrieve the PlexServer from PlexTvShowEpisode with id: {request.Id}");
            }
            return Result.Ok(plexServer);
        }
    }
}