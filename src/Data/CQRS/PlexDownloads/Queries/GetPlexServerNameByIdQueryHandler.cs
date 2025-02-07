﻿using Data.Contracts;
using FluentValidation;
using Logging.Interface;
using Microsoft.EntityFrameworkCore;
using PlexRipper.Data.Common;

namespace PlexRipper.Data;

public class GetPlexServerNameByIdQueryValidator : AbstractValidator<GetPlexServerNameByIdQuery>
{
    public GetPlexServerNameByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetPlexServerNameByIdQueryHandler : BaseHandler, IRequestHandler<GetPlexServerNameByIdQuery, Result<string>>
{
    public GetPlexServerNameByIdQueryHandler(ILog log, PlexRipperDbContext dbContext) : base(log, dbContext) { }

    public async Task<Result<string>> Handle(GetPlexServerNameByIdQuery request, CancellationToken cancellationToken)
    {
        var serverName = await PlexServerQueryable
            .Where(x => x.Id == request.Id)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? "Server Name Not Found";

        return Result.Ok(serverName);
    }
}