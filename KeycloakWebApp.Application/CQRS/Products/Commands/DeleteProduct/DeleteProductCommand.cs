using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest;
