namespace Shop_Cam_BE.Application.Common.Exceptions;

public class KeycloakUsernameAlreadyExistsException : Exception
{
    public KeycloakUsernameAlreadyExistsException()
        : base("Username đã tồn tại.") { }
}

public class KeycloakEmailAlreadyExistsException : Exception
{
    public KeycloakEmailAlreadyExistsException()
        : base("Email đã tồn tại.") { }
}
