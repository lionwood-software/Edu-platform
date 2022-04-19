using System;
using AutoMapper;
using LionwoodSoftware.Repository.Interfaces;
using LionwoodSoftware.Repository.MongoDB;
using LionwoodSoftware.ResponseHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolApi.Extensions;
using SchoolApi.Migrations;

namespace SchoolApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

            services.ResolveServices();
            services.ResolveSwagger();
            services.ResolveIdentity();
            services.ResolveCors();
            services.ResolveApiClients();
            services.AddAutoMapper(typeof(Startup));

            services.RegisterEventBus();

            services.AddTransient(LoadRepositoryConfig);
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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "School API V1");
                });

                app.UseCors("AllowAllDev");
            }
            else
            {
                app.UseCors("AllowWebInterfaceCalls");
            }

            app.ConfigureEventBus();

            app.UseExceptionMiddleware();

            app.ApplyMigrations();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private IRepositoryConfiguration LoadRepositoryConfig(IServiceProvider provider)
        {
            return new MongoDbConfig(
                "mongoDB",
                "school",
                Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING"),
                Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME"));
        }
    }
}
