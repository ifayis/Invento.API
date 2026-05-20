namespace Invento.Application.Interfaces
{
    public interface ICurrentTenantService
    {
        Guid TenantId { get; }
    }
}
