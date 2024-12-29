namespace Account.SharedKernel.HttpReponses;
public static class HttpResponseMessageType
{
    public const string Unknown = "Unknown status code";
    public const string Ok = "Fetched data successfully";
    public const string Created = "Created successfully";
    public const string Updated = "Updated successfully";
    public const string Deleted = "Deleted successfully";
    public const string Accepted = "Request was accepted";
    public const string NoContent = "No content, but might have updated something";
    public const string BadRequest = "Bad request";
    public const string Unauthorized = "Access unauthorized";
    public const string Forbidden = "Access forbidden";
    public const string NotFound = "Not found";
    public const string MethodNotAllowed = "Method not allowed";
    public const string InternalServerError = "Internal server error";
    public const string NotImplemented = "Not implemented";
}