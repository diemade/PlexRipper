﻿using PlexRipper.Domain.Entities;
using System.Threading.Tasks;

namespace PlexRipper.Application.Common.Interfaces
{
    public interface IPlexAuthenticationService
    {
        Task<string> GetPlexToken(PlexAccount plexAccount);
    }
}