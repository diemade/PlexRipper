﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlexRipper.Domain.Entities;
using PlexRipper.Domain.Entities.JoinTables;

namespace PlexRipper.Infrastructure.Persistence.Configurations
{
    public class PlexAccountServerConfiguration : IEntityTypeConfiguration<PlexAccountServer>
    {
        public void Configure(EntityTypeBuilder<PlexAccountServer> builder)
        {
            builder
                .HasKey(bc => new { bc.PlexAccountId, bc.PlexServerId });
        }
    }
}
