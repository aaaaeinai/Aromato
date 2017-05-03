﻿using Aromato.Application;
using Aromato.Application.Web;
using Aromato.Domain;
using Aromato.Domain.Employee;
using Aromato.Domain.Inventory;
using Aromato.Infrastructure.Events;
using Aromato.Infrastructure.Logging;
using Aromato.Infrastructure.PostgreSQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Aromato.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            env.ConfigureNLog("nlog.config");
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PostgresUnitOfWork>();

            // Add framework services.
            services.AddMvc(options =>
            {
            });
            
            // Register repositories.
            services.AddScoped<IEmployeeRepository, PostgresEmployeeRepository>();
            services.AddScoped<IInventoryRepository, PostgresInventoryRepository>();

            // Register services.
            services.AddScoped<IInventoryService, InventoryWebService>();
            services.AddScoped<IEmployeeService, EmployeeWebService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // set primary logger
            AromatoLogging.LoggerFactory = loggerFactory.AddNLog().AddConsole();
            DomainEvent.Dispatcher = new AutoFacEventDispatcher();

            LogManager.Configuration.Variables["connectionString"] = Configuration.GetConnectionString("aromato");

            app.UseMvc();
            app.AddNLogWeb();
        }

    }
}
