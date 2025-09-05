namespace Fin.Domain.Exceptions
{
    public class UserNotFoundException : Exception
    {
        private readonly string? _param;
        private readonly string? _email;

        public UserNotFoundException()
        {
            
        }

        public UserNotFoundException(string param, string email)
        {
            this._param = param;
            this._email = email;
        }

        public override string Message
        {
            get
            {
                var paramEmailText = "";
                if (!string.IsNullOrEmpty(this._param) && !string.IsNullOrEmpty(this._email))
                    paramEmailText = $"{this._param} {this._email}";
                return $"User not found {paramEmailText}";
            }
        }
    }

    public class InvalidCredentialsException : Exception
    {
    }

    public class UserAlreadExistsException(string param, string email) : Exception
    {
        public override string Message => $"User with {param} {email} already exists";
    }
}