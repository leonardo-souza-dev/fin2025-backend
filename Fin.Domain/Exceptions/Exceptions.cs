namespace Fin.Domain.Exceptions
{
    public class UserNotFoundException(string param, string email) : Exception
    {
        public override string Message => $"User with {param} {email} not found";
    }

    public class InvalidCredentialsException : Exception
    {
    }

    public class UserAlreadExistsException(string param, string email) : Exception
    {
        public override string Message => $"User with {param} {email} already exists";
    }
}