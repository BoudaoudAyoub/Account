using MediatR;
namespace Account.API.Application.Commands.UserAccountCommands.Commands;
public abstract class UserAccountBaseCommand<TReponse> : IRequest<TReponse>
{
    public string Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Role { get; set; } = default!;
}