﻿using PlexRipper.Application.Common.Interfaces;
using PlexRipper.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace PlexRipper.Application.TodoLists.Commands.CreateTodoList
{
    public partial class CreateTodoListCommand : IRequest<int>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, int>
    {
        private readonly IPlexRipperDbContext _context;

        public CreateTodoListCommandHandler(IPlexRipperDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoList();

            entity.Title = request.Title;

            _context.TodoLists.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}