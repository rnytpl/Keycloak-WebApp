using Dapper;
using KeycloakWebApp.Application.Data;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Commands.ChangeProductStatus;

public class ChangeProductStatusCommandHandler(ISqlConnectionFactory sqlConnectionFactory) : IRequestHandler<ChangeProductStatusCommand>
{
    public async Task Handle(ChangeProductStatusCommand request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();
        const string sql = "UPDATE Products SET IsListed = @IsListed WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { request.IsListed, request.Id });
    }
}
