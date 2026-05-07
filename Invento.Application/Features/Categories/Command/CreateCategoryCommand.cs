using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Categories.Command
{
    public class CreateCategoryCommand : IRequest<Guid>
    {
        public string Name { get; set; } = "";
    }
}
