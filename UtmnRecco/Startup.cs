using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UtmnRecco.Services;
using VkNet;

namespace UtmnRecco
{
	public class Startup
	{
		IConfiguration config;

		public Startup()
		{
			config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("config.json", false)
				.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// create console logger
			var loggerFactory = new LoggerFactory();
			loggerFactory.AddConsole(LogLevel.Debug);
			var consoleLogger = loggerFactory.CreateLogger("ConsoleLogger");
			services.AddSingleton(consoleLogger);

			var vkApi = new VkApi(new NLog.NullLogger(new NLog.LogFactory()));
			services.AddSingleton(s => vkApi);
			services.AddSingleton(s => new AnalyzingService(
				config["vkLogin"],
				config["vkPassword"],
				vkApi,
				consoleLogger));
			services.AddMvc();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			app.UseStaticFiles();
			app.UseMvc();
		}
	}
}