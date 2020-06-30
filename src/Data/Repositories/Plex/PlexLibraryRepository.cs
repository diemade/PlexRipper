﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlexRipper.Application.Common.Interfaces.Repositories;
using PlexRipper.Data.Common.Interfaces;
using PlexRipper.Domain.Entities;
using PlexRipper.Domain;

namespace PlexRipper.Data.Repositories
{
    public class PlexLibraryRepository : Repository<PlexLibrary>, IPlexLibraryRepository
    {
        public PlexLibraryRepository(IPlexRipperDbContext context) : base(context) { }

        public async Task<IList<PlexLibrary>> GetLibraries(int serverId)
        {
            var x = await FindAllAsync(x => x.PlexServerId == serverId);
            return x.ToList();
        }
    }
}