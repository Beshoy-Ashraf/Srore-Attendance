namespace Domain.Exceptions;

/// <summary>The caller is authenticated but is not allowed to perform this action or see this resource.</summary>
public class ForbiddenException(string message) : Exception(message)
{
}
