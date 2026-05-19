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
    public class UpdateCategoryCommand
        : ICommand<ApiResponse<CategoryDto>>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
            = string.Empty;

        public bool IsDeleted { get; set; }
    }
}
