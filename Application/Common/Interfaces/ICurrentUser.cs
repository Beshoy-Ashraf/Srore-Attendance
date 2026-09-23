namespace Application.Common.Interfaces;

/// <summary>The caller of the current request, taken from the validated JWT (never from the request body).</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }

    /// <summary>The caller's IP address as seen by the API, when known.</summary>
    string? IpAddress { get; }
}
