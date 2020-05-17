﻿namespace PlexRipper.Domain.Entities.Plex
{
    public class PlexLibrary : BaseEntity
    {
        public int SectionId { get; set; }

        public int Count { get; set; }

        public string Key { get; set; }

        public string Title { get; set; }

        public bool HasAccess { get; set; }

        public PlexServer PlexServer { get; set; }

    }
}