namespace Invento.Application.Features.Company.DTOs
{
    public class CompanyProfileDto
    {
        public Guid Id { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? LogoUrl { get; set; }

        public string? TaxNumber { get; set; }

        public string? Website { get; set; }
    }
}
