using AutoMapper;
using LionwoodSoftware.ResponseHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RouterApi.Extensions;
using Serilog;

namespace RouterApi
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
            .AddNewtonsoftJson();

            services.ResolveSwagger();
            services.ResolveServices();
            services.ResolveCors();
            services.ResolveApiClients();
            services.ResolveRepositories();
            services.ResolveAuthentication();
            services.ResolveObserversAndEventHandlers();

            services.RegisterEventBus();

            services.AddAutoMapper(typeof(Startup));
            services.AddHttpContextAccessor();
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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Router API V1");
                });
                app.UseCors("AllowAllDev");
            }
            else
            {
                app.UseCors("AllowWebInterfaceCalls");
            }

            app.ConfigureEventBus();
            app.UseRouting();
            app.UseExceptionMiddleware();
            app.UseAuthorization();

            app.UseSerilogRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
