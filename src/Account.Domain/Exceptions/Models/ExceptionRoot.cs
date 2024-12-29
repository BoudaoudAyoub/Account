namespace Account.Domain.Exceptions.Models;
public sealed class ExceptionRoot
{
    public string Instance { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public DateTime ExceptionLogTime { get; set; } = default!;
    public ExceptionDetail ErrorResponse { get; set; } = default!;
}