using KeycloakWebApp.Application.CQRS.Products.DTOs;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<IEnumerable<ProductDto>>;
