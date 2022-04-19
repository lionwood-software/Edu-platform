using System;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace NitApi.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ResolveApiClients(this IServiceCollection services)
        {
            services.AddHttpClient();

            return services;
        }

        public static IServiceCollection ResolveIdentity(this IServiceCollection services)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.SaveToken = true;
                    options.Authority = Environment.GetEnvironmentVariable("IDENTITY_AUTHORITY");
                    options.ApiName = Environment.GetEnvironmentVariable("API_NAME");
                    options.ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
                    options.RequireHttpsMetadata = false;
                    options.RoleClaimType = JwtClaimTypes.Role;
                    options.NameClaimType = JwtClaimTypes.Name;
                })
                .AddIdentityServerAuthentication("RouterKey", o =>
                {
                    o.Authority = Environment.GetEnvironmentVariable("IDENTITY_AUTHORITY");
                    o.ApiName = Environment.GetEnvironmentVariable("ROUTER_API_NAME");
                    o.ApiSecret = Environment.GetEnvironmentVariable("ROUTER_API_SECRET");
                    o.SupportedTokens = SupportedTokens.Both;
                    o.RequireHttpsMetadata = false;
                })
                .AddIdentityServerAuthentication("IdentityKey", o =>
                {
                    o.Authority = Environment.GetEnvironmentVariable("IDENTITY_AUTHORITY");
                    o.ApiName = Environment.GetEnvironmentVariable("IDENTITY_API_NAME");
                    o.ApiSecret = Environment.GetEnvironmentVariable("IDENTITY_API_SECRET");
                    o.SupportedTokens = SupportedTokens.Both;
                    o.RequireHttpsMetadata = false;
                })
                .AddIdentityServerAuthentication("SchoolKey", o =>
                {
                    o.Authority = Environment.GetEnvironmentVariable("IDENTITY_AUTHORITY");
                    o.ApiName = Environment.GetEnvironmentVariable("SCHOOL_API_NAME");
                    o.ApiSecret = Environment.GetEnvironmentVariable("SCHOOL_API_SECRET");
                    o.SupportedTokens = SupportedTokens.Both;
                    o.RequireHttpsMetadata = false;
                });

            return services;
        }

        public static IServiceCollection ResolveCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowWebInterfaceCalls", policy =>
                {
                    var corses = Environment.GetEnvironmentVariable("ALLOWED_CORS") ?? string.Empty;
                    policy.WithOrigins(corses.Split(","))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });

                options.AddPolicy("AllowAllDev", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
