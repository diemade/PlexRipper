﻿using FluentResults;
using MediatR;
using PlexRipper.Domain;

namespace PlexRipper.Application
{
    public class GetPlexTvShowEpisodeByIdQuery : IRequest<Result<PlexTvShowEpisode>>
    {
        public GetPlexTvShowEpisodeByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }

    }
}