﻿namespace Account.SharedKernel.HttpReponses;
public static class HttpResponseType
{
    public const int Unknown = 0;
    public const int Ok = 200;
    public const int Created = 201;
    public const int Accepted = 202;
    public const int NoContent = 204;
    public const int BadRequest = 400;
    public const int Unauthorized = 401;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int MethodNotAllowed = 405;
    public const int InternalServerError = 500;
    public const int NotImplemented = 501;
}