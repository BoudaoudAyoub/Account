namespace Account.Domain.Constants;
public static class ErrorConstant
{
    public const string USER_FIRST_NAME_MUST_NOT_BE_NULL_OR_EMPTY = "User firstname must not be null or empty";
    public const string USER_LAST_NAME_MUST_NOT_BE_NULL_OR_EMPTY = "User lastname must not be null or empty";
    public const string USER_ALREADY_EXISTS = "Username: {0} has already exist";
    public const string USER_COULD_NOT_BE_REGISTRED = "User could not be registred";
    public const string ROLE_COULD_NOT_BE_ADDED = "Role coul not be assigned";
    public const string CONNECTION_STRING_CANNOT_BE_NULL = "Connection string cannot be null";
    public const string SYSTEM_INVALID_DATA_MESSAGE = "The system couldn't recognize the provided data. Please ensure the input is correct";
}
