using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeaMaki.Persistence;
using AutoMapper;
using TeaMaki.Menu.ProductAggregate;
using System.Threading.Tasks;
using System.Threading;
using Consul;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.Logging;

namespace TeaMaki.Menu
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
            services.AddAutoMapper(typeof(MenuProfile));
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddSingleton<IHostedService, ConsulRegisterService>();
            services.Configure<MenuConfiguration>(Configuration.GetSection("Menu"));
            services.Configure<ConsulConfiguration>(Configuration.GetSection("Consul"));

            var consulAddress = Configuration.GetSection("Consul")["Url"];

            services.AddSingleton<IConsulClient, ConsulClient>(provider =>
                new ConsulClient(config => config.Address = new Uri(consulAddress)));

            services.AddPersistenceLibrary(Configuration.GetSection("Database"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(
                ui =>
                {
                    ui.SwaggerEndpoint("swagger/v1/swagger.json", "Menu v1");
                    ui.RoutePrefix = "";
                }
            );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }


    public class ConsulConfiguration
    {
        public string Url { get; set; }
    }

    public class MenuConfiguration
    {
        public string Url { get; set; }
        public string ServiceName { get; set; }
        public string ServiceId { get; set; }
    }

    public class ConsulRegisterService : IHostedService
    {
        private IConsulClient _consulClient;
        private ConsulConfiguration _consulConfiguration;
        private MenuConfiguration _menuConfiguration;
        private ILogger<ConsulRegisterService> _logger;

        public ConsulRegisterService(IConsulClient consulClient,
            IOptions<MenuConfiguration> menuConfiguration,
            IOptions<ConsulConfiguration> consulConfiguration,
            ILogger<ConsulRegisterService> logger)
        {
            _consulClient = consulClient;
            _consulConfiguration = consulConfiguration.Value;
            _menuConfiguration = menuConfiguration.Value;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var menuUri = new Uri(_menuConfiguration.Url);

            var serviceRegistration = new AgentServiceRegistration()
            {
                Address = menuUri.Host,
                Name = _menuConfiguration.ServiceName,
                Port = menuUri.Port,
                ID = _menuConfiguration.ServiceId,
                Tags = new[] { _menuConfiguration.ServiceName }
            };

            await _consulClient.Agent.ServiceDeregister(_menuConfiguration.ServiceId, cancellationToken);
            await _consulClient.Agent.ServiceRegister(serviceRegistration, cancellationToken);

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _consulClient.Agent.ServiceDeregister(_menuConfiguration.ServiceId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when trying to de-register", ex);
            }
        }
    }



    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<ProductToPut, Product>();
        }
    }
}
