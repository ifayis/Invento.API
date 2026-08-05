namespace Invento.Application.Features.Customers.DTOs
{
    public  class CustomerDeleteDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
