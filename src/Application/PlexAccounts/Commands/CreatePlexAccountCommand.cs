﻿using FluentResults;
using FluentValidation;
using MediatR;
using PlexRipper.Application.Common.Interfaces.DataAccess;
using PlexRipper.Domain;
using PlexRipper.Domain.Base;
using PlexRipper.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace PlexRipper.Application.PlexAccounts
{
    public class CreatePlexAccountCommand : IRequest<Result<int>>
    {
        public PlexAccount PlexAccount { get; }

        public CreatePlexAccountCommand(PlexAccount plexAccount)
        {
            PlexAccount = plexAccount;
        }
    }

    public class CreatePlexAccountCommandValidator : AbstractValidator<CreatePlexAccountCommand>
    {
        public CreatePlexAccountCommandValidator()
        {
            RuleFor(x => x.PlexAccount.Id).Equal(0).WithMessage("The Id should be 0 when creating a new PlexAccount");
            RuleFor(x => x.PlexAccount.Username).NotEmpty().MinimumLength(5);
            RuleFor(x => x.PlexAccount.Password).NotEmpty().MinimumLength(5);
            RuleFor(x => x.PlexAccount.DisplayName).NotEmpty();
        }
    }

    public class CreateAccountHandler : BaseHandler, IRequestHandler<CreatePlexAccountCommand, Result<int>>
    {
        private readonly IPlexRipperDbContext _dbContext;

        public CreateAccountHandler(IPlexRipperDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<int>> Handle(CreatePlexAccountCommand command, CancellationToken cancellationToken)
        {
            Log.Debug("Creating a new Account in DB");

            await _dbContext.PlexAccounts.AddAsync(command.PlexAccount);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(command.PlexAccount).GetDatabaseValuesAsync();

            return Result.Ok(command.PlexAccount.Id);
        }
    }
}