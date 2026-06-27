using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, string Description, decimal Price) : IRequest<int>;
