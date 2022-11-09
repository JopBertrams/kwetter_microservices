using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using HonkService.Application.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using MediatR;
using Plain.RabbitMQ;
using RabbitMQ.Client;

namespace HonkService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            string domain = $"https://{configuration["Auth0:Domain"]}/";
            services.AddMediatR(typeof(DependencyInjection).Assembly);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = domain;
                    options.Audience = configuration["Auth0:Audience"];
                    // If the access token does not have a `sub` claim, `User.Identity.Name` will be `null`. Map it to a different claim by setting the NameClaimType below.
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("write:honks", policy => policy.Requirements.Add(new HasScopeRequirement("write:honks", domain)));
            });
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqps://gzzpbcle:b4SqtWxK0tLpbeFa7roTWnkT4bfxmQBm@sparrow.rmq.cloudamqp.com/gzzpbcle"));
            services.AddScoped<Plain.RabbitMQ.IPublisher>(x => new Publisher(x.GetService<IConnectionProvider>(),
                "honk_exchange",
                ExchangeType.Topic));

            return services;
        }
    }
}
