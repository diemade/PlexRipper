﻿using System.IO;

namespace PlexRipper.Domain
{
    public interface IMount
    {
        long AvailableFreeSpace { get; }
        string DriveFormat { get; }
        DriveType DriveType { get; }
        bool IsReady { get; }
        MountOptions MountOptions { get; }
        string Name { get; }
        string RootDirectory { get; }
        long TotalFreeSpace { get; }
        long TotalSize { get; }
        string VolumeLabel { get; }
        string VolumeName { get; }
    }
}
