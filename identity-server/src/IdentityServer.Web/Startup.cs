using System;
using Autofac;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Web.Configuration;
using IdentityServer.Web.IdentityServer4;
using IdentityServer.Web.IdentityServer4.Store;
using IdentityServer.Web.IdentityServer4.Validators;
using IdentityServer.Web.Middleware;
using IdentityServer.Web.Modules;
using IdentityServer.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenCensus.Trace.Sampler;
using OpenTelemetry.Trace.Configuration;
using OpenTelemetry.Trace.Samplers;
using Serilog;
using StackExchange.Profiling.SqlFormatters;
using StackExchange.Profiling.Storage;
using Steeltoe.Discovery.Client;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Metrics;

namespace IdentityServer.Web
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
            services.AddControllers();

            services.AddGrpc();
            services.AddMemoryCache(opt =>
            {
                var cache = Configuration.GetSection("Cache").Get<MemoryCacheOptions>();
                opt.ExpirationScanFrequency = cache.ExpirationScanFrequency;
                opt.CompactionPercentage = cache.CompactionPercentage;
                opt.SizeLimit = cache.SizeLimit;
            });

            services.AddMiniProfiler(options =>
            {
                var mini = Configuration.GetSection("Profiler").Get<Profiler>();
                options.RouteBasePath = mini.Route;
                options.SqlFormatter = new InlineFormatter();
                options.Storage = new PostgreSqlStorage(Configuration.GetConnectionString("Postgres"));
            });

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddClientStore<ClientStore>()
                .AddResourceStore<ResourceStore>()
                .AddProfileService<ProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
            
            services.AddHealthActuator(Configuration);
            services.AddDiscoveryClient(Configuration);
            
            services.AddScoped(_ =>
                Configuration.GetSection("ConnectionStrings")
                    .GetSection("RavenDb")
                    .Get<RavenDbConfiguration>());

            services.AddOpenTelemetry((provider, builder) =>
            {
                builder
                    .UseZipkin(opt =>
                    {
                        opt.ServiceName = "Identity-Server";
                        opt.Endpoint = new Uri(Configuration.GetSection("Tracing").GetValue<string>("Host"));
                    })
                    .AddRequestCollector()
                    .AddDependencyCollector();
            });
        }
        
        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<RepositoryModule>()
                .RegisterModule<AggregationModule>()
                .RegisterModule<ApplicationModule>()
                .RegisterModule<MapperModule>();

            builder.RegisterType<SHA256Algorithm>()
                .As<IHashAlgorithm>()
                .SingleInstance();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMiniProfiler();

            app.UseCorrelationId();
            app.UseSerilogRequestLogging(opt =>
            {
                opt.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
            });
            
            //app.UseHttpsRedirection();
            app.UseRouting();

            app.UseIdentityServer();
            
            app.UseHealthActuator();
            app.UseDiscoveryClient();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<ResourceService>();
            });
        }
    }
}
