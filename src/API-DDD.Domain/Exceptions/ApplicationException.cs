namespace API_DDD.Domain.Exceptions
{
    public abstract class AppException : Exception
    {
        public string ErrorCode { get; }

        protected AppException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string entityName, object id)
            : base("NOT_FOUND", $"{entityName} with id '{id}' was not found.")
        {
        }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message) : base("CONFLICT", message) { }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "You do not have permission to perform this action.")
            : base("FORBIDDEN", message)
        {
        }
    }

    public class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(string message, IDictionary<string, string[]>? errors = null)
            : base("VALIDATION_ERROR", message)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }
}