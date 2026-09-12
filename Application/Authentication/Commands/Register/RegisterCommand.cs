using Application.Authentication.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Authentication.Commands.Register;

public record RegisterCommand(string Username, string Email, string Password, string DisplayName, string ProfilePictureUrl, UserRole Role) : IRequest<AuthResponseDto>;