using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>> 
    {
        public string? Search { get; set; }
    }

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }
}
