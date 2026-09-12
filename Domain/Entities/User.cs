using Domain.Enums;

namespace Domain.Entities;

public class User(
    string username,
    string email,
    string passwordHash,
    string displayName,
    string profilePictureUrl,
    UserRole role)
{
      public Guid Id { get; set; }
      public string Username { get; set; } = username;
      public string PasswordHash { get; set; } = passwordHash;
      public string Email { get; set; } = email;
      public string DisplayName { get; set; } = displayName;
      public string ProfilePictureUrl { get; set; } = profilePictureUrl;
      public UserRole Role { get; set; } = role;

      public Guid? StoreId { get; set; }
      public Store? Store { get; set; }

      public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

      public DateTime CreatedDate { get; set; }
      public DateTime? UpdateDate { get; set; }
      public DateTime? DeleteDate { get; set; }
}