using Invento.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Application.Common;

namespace Invento.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand
        : ICommand<ApiResponse<Guid>>
    {
        public string Name { get; set; }
            = string.Empty;
    }
}
