using Dapper;
using KeycloakWebApp.Application.Data;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public CreateProductCommandHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = @"
            INSERT INTO Products (Name, Description, Price, IsListed) 
            VALUES (@Name, @Description, @Price, true) 
            RETURNING Id;";

        var id = await connection.ExecuteScalarAsync<int>(sql, new { request.Name, request.Description, request.Price });

        return id;
    }
}
