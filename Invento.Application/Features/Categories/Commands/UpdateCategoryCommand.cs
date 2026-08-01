using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Categories.DTOs;
using System.Text.Json.Serialization;

namespace Invento.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand 
        : ICommand<ApiResponse<CategoryDto>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
