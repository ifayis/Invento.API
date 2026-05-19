using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Company.DTOs;

namespace Invento.Application.Features.Company.Commands
{
    public class UpdateCompanyProfileCommand
        : ICommand<ApiResponse<CompanyProfileDto>>
    {
        public string CompanyName { get; set; }
            = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? LogoUrl { get; set; }

        public string? TaxNumber { get; set; }

        public string? Website { get; set; }
    }
}
