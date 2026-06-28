using KeycloakWebApp.Application.Common.Models;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<IEnumerable<ProductDto>>;
