using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Steeltoe.Discovery.Client;
using Steeltoe.Management.Endpoint.Health;
using Users.Web.Middleware;
using Users.Web.Modules;
using Users.Web.Services;

namespace Users.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddGrpc();
            services.AddMemoryCache(opt =>
            {
                var cache = _configuration.GetSection("Cache").Get<MemoryCacheOptions>();
                opt.ExpirationScanFrequency = cache.ExpirationScanFrequency;
                opt.CompactionPercentage = cache.CompactionPercentage;
                opt.SizeLimit = cache.SizeLimit;
            });

            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";
            });

            services.AddHealthActuator(_configuration);
            services.AddDiscoveryClient(_configuration);
        }
        
        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<AggregationModule>()
                .RegisterModule<MapperModule>()
                .RegisterModule<OperationModule>()
                .RegisterModule<RepositoryModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCorrelationId();
            app.UseSerilogRequestLogging(opt =>
            {
                opt.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
            });
            
            app.UseRouting();

            app.UseMiniProfiler();
            app.UseHealthActuator();
            app.UseDiscoveryClient();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<UserService>();
                endpoints.MapGrpcService<HealthService>();
            });
        }
    }
}
