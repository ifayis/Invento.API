using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Categories.Queries
{
    public class SearchCategoryQuery : IRequest<IEnumerable<CategoryDto>>
    {
        public string Search { get; set; } = "";
    }
}
