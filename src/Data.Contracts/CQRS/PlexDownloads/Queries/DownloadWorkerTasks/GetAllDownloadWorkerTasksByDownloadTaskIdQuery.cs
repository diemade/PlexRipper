﻿using FluentResults;
using MediatR;
using PlexRipper.Domain;

namespace Data.Contracts;

public class GetAllDownloadWorkerTasksByDownloadTaskIdQuery : IRequest<Result<List<DownloadWorkerTask>>>
{
    public GetAllDownloadWorkerTasksByDownloadTaskIdQuery(int downloadTaskId)
    {
        DownloadTaskId = downloadTaskId;
    }

    public int DownloadTaskId { get; }
}