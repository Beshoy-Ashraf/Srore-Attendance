using System.Reflection;
using Application.Common.Behaviors;
using Application.Common.Interfaces;
using Application.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
      public static IServiceCollection AddApplicationServices(this IServiceCollection services)
      {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidateBehavior<,>));
            services.AddScoped<IAccessService, AccessService>();

            return services;
      }
}