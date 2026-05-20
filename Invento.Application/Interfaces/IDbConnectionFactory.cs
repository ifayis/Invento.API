using System.Data;

namespace Invento.Application.Common.Interface
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
