using Dapper;
using KeycloakWebApp.Application.Data;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Commands.ChangeProductStatus;

public class ChangeProductStatusCommandHandler : IRequestHandler<ChangeProductStatusCommand>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public ChangeProductStatusCommandHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task Handle(ChangeProductStatusCommand request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        const string sql = "UPDATE Products SET IsListed = @IsListed WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { request.IsListed, request.Id });
    }
}
