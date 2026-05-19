using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Categories.DTOs;
using MediatR;
using System;

namespace Invento.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : 
        ICommand<ApiResponse<CategoryDto>>
    {
        public string Name { get; set; }
    }
}
