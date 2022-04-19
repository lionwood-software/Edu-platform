using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using LionwoodSoftware.EventBus;
using LionwoodSoftware.EventBus.Abstractions;
using LionwoodSoftware.EventBus.RabbitMQ;
using LionwoodSoftware.MediaStorage;
using LionwoodSoftware.MediaStorage.Interfaces;
using LionwoodSoftware.Repository.Interfaces;
using LionwoodSoftware.Repository.MongoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using SchoolApi.IntegrationEvents.EventHandling;
using SchoolApi.IntegrationEvents.Events;
using SchoolApi.Services;
using SchoolApi.Services.Interfaces;
using System;

namespace SchoolApi.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ResolveServices(this IServiceCollection services)
        {
            services.AddTransient<IRepository, MongoDbRepository>();
            services.AddSingleton<IChatService, ChatService>();

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

        public static IServiceCollection ResolveApiClients(this IServiceCollection services)
        {
            services.AddHttpClient();

            return services;
        }

        public static IServiceCollection ResolveSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "School API", Version = "v1" });
            });

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
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("GlobalAdminOnly", policy => policy.RequireClaim(JwtClaimTypes.Role, "GLOBAL_ADMIN"));
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
                    policy.WithOrigins(corses)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllDev", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }

        public static IServiceCollection RegisterEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var isValidPort = int.TryParse(Environment.GetEnvironmentVariable("EVENTBUS_PORT"), out int port);

                var factory = new ConnectionFactory()
                {
                    HostName = Environment.GetEnvironmentVariable("EVENTBUS_HOSTNAME"),
                    DispatchConsumersAsync = true,
                    Port = isValidPort ? port : 5672,
                    VirtualHost = Environment.GetEnvironmentVariable("EVENTBUS_VIRTUAL_HOST"),
                    UserName = Environment.GetEnvironmentVariable("EVENTBUS_USERNAME"),
                    Password = Environment.GetEnvironmentVariable("EVENTBUS_PASSWORD")
                };

                return new DefaultRabbitMQPersistentConnection(factory, logger);
            });

            var schoolUID = Program.AppSchoolId;

            services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new RabbitMQEventBus(rabbitMQPersistentConnection, logger, sp, eventBusSubcriptionsManager, queueName: "event_bus_queue_school" + schoolUID, brokerName: "nit_event_bus");
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.AddTransient<CreatedDefaultSchoolChatsIntegrationEventHandler>();

            return services;
        }

        public static void ConfigureEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            var schoolUID = Program.AppSchoolId;

            eventBus.Subscribe<CreatedDefaultSchoolChatsIntegrationEvent, CreatedDefaultSchoolChatsIntegrationEventHandler>(typeof(CreatedDefaultSchoolChatsIntegrationEvent).Name + "-" + schoolUID);
        }
    }
}
