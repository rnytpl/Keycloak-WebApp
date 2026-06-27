using Dapper;
using KeycloakWebApp.Application.Data;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public DeleteProductCommandHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        const string sql = "DELETE FROM Products WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { request.Id });
    }
}
