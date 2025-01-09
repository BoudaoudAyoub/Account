namespace Account.API.Application.Models;
public class UserAccountFilterModel
{
    public int Page { get; set; }
    public int MaxRow { get; set; }
    public bool Deleted { get; set; }
}