﻿using Data.Contracts;
using FluentValidation;
using Logging.Interface;
using PlexRipper.Data.Common;

namespace PlexRipper.Data;

public class AddDownloadWorkerTasksCommandValidator : AbstractValidator<AddDownloadWorkerTasksCommand>
{
    public AddDownloadWorkerTasksCommandValidator()
    {
        RuleForEach(x => x.DownloadWorkerTasks)
            .ChildRules(task =>
            {
                task.RuleFor(x => x.Id).Equal(0);
                task.RuleFor(x => x.DownloadTaskId).GreaterThan(0);
                task.RuleFor(x => x.FileName).NotEmpty();
                task.RuleFor(x => x.FileLocationUrl).NotEmpty();
                task.RuleFor(x => x.TempDirectory).NotEmpty();
            });
    }
}

public class AddDownloadWorkerTasksCommandHandler : BaseHandler, IRequestHandler<AddDownloadWorkerTasksCommand, Result<bool>>
{
    public AddDownloadWorkerTasksCommandHandler(ILog log, PlexRipperDbContext dbContext) : base(log, dbContext) { }

    public async Task<Result<bool>> Handle(AddDownloadWorkerTasksCommand command, CancellationToken cancellationToken)
    {
        command.DownloadWorkerTasks.ForEach(x => x.DownloadTask = null);
        await _dbContext.DownloadWorkerTasks.AddRangeAsync(command.DownloadWorkerTasks, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(true);
    }
}