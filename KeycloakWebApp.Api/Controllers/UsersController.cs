using KeycloakWebApp.Application.Common.Models;
using KeycloakWebApp.Application.CQRS.Identity.Commands.GetAccessToken;
using KeycloakWebApp.Application.CQRS.Users.Commands.AssignRole;
using KeycloakWebApp.Application.CQRS.Users.Commands.RegisterUser;
using KeycloakWebApp.Application.CQRS.Users.Commands.RemoveRole;
using KeycloakWebApp.Application.CQRS.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakWebApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "admin,moderator")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _mediator.Send(new GetUsersQuery());

        return Ok(users);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);

            return Ok(new { Message = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("{id}/roles")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignRole(string id, [FromBody] string roleName)
    {
        await _mediator.Send(new AssignRoleCommand(id, roleName));
        return NoContent();
    }

    [HttpDelete("{id}/roles/{roleName}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RemoveRole(string id, string roleName)
    {
        await _mediator.Send(new RemoveRoleCommand(id, roleName));
        return NoContent();
    }

    [HttpPost("access-token")]
    [AllowAnonymous]
    public async Task<ActionResult> GetAccessToken([FromBody] GetAccessTokenCommand command)
    {

        await _mediator.Send(command);

        return Ok();

    }
}
