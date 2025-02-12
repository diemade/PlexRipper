﻿using Data.Contracts;
using FluentValidation;
using Logging.Interface;
using Microsoft.EntityFrameworkCore;
using PlexRipper.Data.Common;

namespace PlexRipper.Data.FileManager;

public class DeleteFileTaskByIdValidator : AbstractValidator<DeleteFileTaskByIdCommand>
{
    public DeleteFileTaskByIdValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class DeleteFileTaskByIdHandler : BaseHandler, IRequestHandler<DeleteFileTaskByIdCommand, Result>
{
    public DeleteFileTaskByIdHandler(ILog log, PlexRipperDbContext dbContext) : base(log, dbContext) { }

    public async Task<Result> Handle(DeleteFileTaskByIdCommand command, CancellationToken cancellationToken)
    {
        var downloadFileTask = await _dbContext.FileTasks.AsTracking()
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (downloadFileTask == null)
            return ResultExtensions.EntityNotFound(nameof(DownloadFileTask), command.Id);

        _dbContext.FileTasks.Remove(downloadFileTask);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}