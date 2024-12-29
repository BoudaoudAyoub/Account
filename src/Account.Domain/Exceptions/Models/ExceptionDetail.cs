using Account.SharedKernel.HttpReponses;
namespace Account.Domain.Exceptions.Models;
public sealed class ExceptionDetail
{
    public int StatusCode { get; set; } = HttpResponseType.BadRequest;
    public string? Error { get; set; } = default!;
    public List<Errors>? Errors { get; set; } = default!;
}

public sealed class Errors
{
    public int StatusCode { get; set; } = default!;
    public string[] _Errors { get; set; } = default!;
}