using MediatR;

namespace KeycloakWebApp.Application.CQRS.Products.Commands.ChangeProductStatus;

public record ChangeProductStatusCommand(int Id, bool IsListed) : IRequest;
