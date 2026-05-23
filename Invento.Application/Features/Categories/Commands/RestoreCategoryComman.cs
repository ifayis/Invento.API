using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Categories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Categories.Commands
{
    public class RestoreCategoryCommand
        : ICommand<ApiResponse<CategoryDto>>
    {
        public Guid Id { get; set; }
    }
}
