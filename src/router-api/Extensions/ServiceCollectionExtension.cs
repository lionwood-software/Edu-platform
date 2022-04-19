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
using RouterApi.ApiClients.Identity;
using RouterApi.IntegrationEvents.EventHandling;
using RouterApi.IntegrationEvents.Events;
using RouterApi.Interfaces.Repositories;
using RouterApi.Interfaces.Services;
using RouterApi.Observers.Request;
using RouterApi.Repositories;
using RouterApi.Services;
using System;
using System.Net.Http;

namespace RouterApi.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ResolveServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestService, RequestService>();

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

            services.AddTransient(x =>
            {
                var httpClientFactory = x.GetRequiredService<IHttpClientFactory>();

                return new IdentityApiClient(httpClientFactory, Environment.GetEnvironmentVariable("IDENTITY_AUTHORITY"));
            });

            return services;
        }

        public static IServiceCollection ResolveSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RouterApi", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                     },
                     new string[] { }
                   }
                });
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
                // this defines a CORS policy called "AllowAllDev" which allow any calls from any origin
                options.AddPolicy("AllowAllDev", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }

        public static IServiceCollection ResolveAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
              .AddIdentityServerAuthentication(options =>
              {
                  options.SaveToken = true;
                  options.Authority = Environment.GetEnvironmentVariable("IDENTITY_AUTHORITY");
                  options.ApiName = Environment.GetEnvironmentVariable("ROUTER_API_NAME");
                  options.ApiSecret = Environment.GetEnvironmentVariable("ROUTER_API_SECRET");
                  options.RequireHttpsMetadata = false;
                  options.RoleClaimType = JwtClaimTypes.Role;
                  options.NameClaimType = JwtClaimTypes.Name;
              });
            return services;
        }

        public static IServiceCollection ResolveRepositories(this IServiceCollection services)
        {
            services.AddTransient(LoadRepositoryConfig);
            services.AddTransient<IRepository, MongoDbRepository>();

            services.AddTransient<IRequestRepository, RequestRepository>();

            return services;
        }

        public static IServiceCollection ResolveObserversAndEventHandlers(this IServiceCollection services)
        {
            services.AddSingleton<RequestObserver>();
            services.AddSingleton<RequestEventHandler>();

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

            services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new RabbitMQEventBus(rabbitMQPersistentConnection, logger, sp, eventBusSubcriptionsManager, queueName: "event_bus_queue_router", brokerName: "nit_event_bus");
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.AddTransient<UpdatedSchoolIntegrationEventHandler>();

            return services;
        }

        public static void ConfigureEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<UpdatedSchoolIntegrationEvent, UpdatedSchoolIntegrationEventHandler>();
        }

        private static IRepositoryConfiguration LoadRepositoryConfig(IServiceProvider provider)
        {
            return new MongoDbConfig(
                "MongoDB",
                "router-api",
                Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING"),
                Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME"));
        }
    }
}
