using Dapper;
using KeycloakWebApp.Application.Common.Models;
using KeycloakWebApp.Application.Data;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        
        const string sql = "SELECT Id, Name, Description, Price, IsListed FROM Products;";

        return await connection.QueryAsync<ProductDto>(sql);
    }
}
