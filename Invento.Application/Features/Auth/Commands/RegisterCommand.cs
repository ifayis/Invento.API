using Invento.Application.Abstractions;
using Invento.Application.Features.Auth.DTOs;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Auth.Commands
{
    public class RegisterCommand
        : ICommand<ApiResponse<AuthResponseDto>>
    {
        public string CompanyName { get; set; }
            = string.Empty;

        public string FullName { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public string Password { get; set; }
            = string.Empty;
    }
}
