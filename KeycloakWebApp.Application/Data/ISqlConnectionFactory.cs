using System.Data;

namespace KeycloakWebApp.Application.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
