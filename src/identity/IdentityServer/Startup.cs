using System;
using System.IO;
using AutoMapper;
using IdentityServer.Extensions;
using LionwoodSoftware.Repository.Interfaces;
using LionwoodSoftware.Repository.MongoDB;
using LionwoodSoftware.ResponseHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return new UnprocessableEntityObjectResult(ModelStateValidator.Result(context));
                };
            })
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddHttpContextAccessor();

            services.ResolveIdentity();
            services.ResolveCors();
            services.ResolveServices();
            services.ResolveSwagger();
            services.AddAutoMapper(typeof(Startup));

            services.AddTransient(LoadRepositoryConfig);

            // Persist key to path /key. If running in docker create volume for - host-mounted-drive:/key
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"/key"))
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API V1");
                });
                app.UseCors("AllowAllDev");
            }
            else
            {
                app.UseCors("AllowWebInterfaceCalls");
            }

            app.UseRouting();
            app.UseExceptionMiddleware();
            app.UseAuthorization();
            app.UseIdentityServer();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private IRepositoryConfiguration LoadRepositoryConfig(IServiceProvider provider)
        {
            return new MongoDbConfig(
                Environment.GetEnvironmentVariable("MONGODB_TYPE"),
                Environment.GetEnvironmentVariable("MONGODB_NAME"),
                Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING"),
                Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME"));
        }
    }
}
