using System.Security.Claims;
using Application.Common.Interfaces;

namespace API.Auth;

/// <summary>Reads the caller's identity from the validated JWT for the current HTTP request.</summary>
public class HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
      public Guid? UserId
      {
            get
            {
                  var value = httpContextAccessor.HttpContext?.User
                      .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                  return Guid.TryParse(value, out var id) ? id : null;
            }
      }

      public string? IpAddress =>
          httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
