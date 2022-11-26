using Microsoft.AspNetCore.Authentication.JwtBearer;
using TimelineService.Application.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using MediatR;
using StackExchange.Redis;
using TimelineService.Domain;
using TimelineService.Application.Services;

namespace TimelineService.Application
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
            services.AddSingleton<IConnectionMultiplexer>(opt =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")));
            services.AddSingleton<ITimelineRepositoy, TimelineRepository>();
            services.AddHostedService<HonkDataCollector>();

            return services;
        }
    }
}
