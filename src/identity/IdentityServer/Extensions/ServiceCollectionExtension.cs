using System;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using IdentityModel;
using IdentityServer.Interfaces;
using IdentityServer.Services;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using LionwoodSoftware.MediaStorage;
using LionwoodSoftware.MediaStorage.Interfaces;
using LionwoodSoftware.Repository.Interfaces;
using LionwoodSoftware.Repository.MongoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;

namespace IdentityServer.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ResolveServices(this IServiceCollection services)
        {
            services.AddTransient<IRepository, MongoDbRepository>();
            services.AddTransient<IAttachmentService, AttachmentService>();
            services.AddTransient<ISystemService, SystemService>();

            services.AddTransient<IStorageService>(x =>
            {
                var isValid = bool.TryParse(Environment.GetEnvironmentVariable("STORAGE_USE_SSL"), out bool useSSL);

                return new MinioStorageService(
                    isValid ? useSSL : false,
                    Environment.GetEnvironmentVariable("STORAGE_ENDPOINT"),
                    Environment.GetEnvironmentVariable("STORAGE_ACCESS_KEY"),
                    Environment.GetEnvironmentVariable("STORAGE_SECRET_KEY"),
                    Environment.GetEnvironmentVariable("STORAGE_REGION"));
            });

            return services;
        }

        public static IServiceCollection ResolveSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "identity API", Version = "v1" });
            });

            return services;
        }

        public static IServiceCollection ResolveCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                // this defines a CORS policy called "AllowWebInterfaceCalls"
                options.AddPolicy("AllowWebInterfaceCalls", policy =>
                {
                    var corses = Environment.GetEnvironmentVariable("ALLOWED_CORS") ?? string.Empty;
                    policy.WithOrigins(corses.Split(","))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "AllowWebInterfaceCalls"
                options.AddPolicy("AllowAllDev", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }

        public static IServiceCollection ResolveIdentity(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = "ConfirmEmail";
            });

            services.AddIdentityMongoDbProvider<MongoUser, MongoRole>(identityOptions =>
            {
                identityOptions.Password.RequiredLength = 8;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.Password.RequireDigit = false;
            }, mongoIdentityOptions =>
            {
                mongoIdentityOptions.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
                mongoIdentityOptions.RolesCollection = "Roles";
                mongoIdentityOptions.UsersCollection = "Users";
            });

            ConfigureMongoDriver2IgnoreExtraElements();

            services.AddIdentityServer(
                    options =>
                    {
                        options.Events.RaiseSuccessEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseErrorEvents = true;
                        options.PublicOrigin = Environment.GetEnvironmentVariable("IDENTITY_AUTHORITY");
                    });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.SaveToken = true;
                    options.Authority = Environment.GetEnvironmentVariable("IDENTITY_AUTHORITY");
                    options.ApiName = Environment.GetEnvironmentVariable("IDENTITY_API_NAME");
                    options.ApiSecret = Environment.GetEnvironmentVariable("IDENTITY_API_SECRET");
                    options.RequireHttpsMetadata = false;
                    options.RoleClaimType = JwtClaimTypes.Role;
                    options.NameClaimType = JwtClaimTypes.Name;
                });

            return services;
        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
