using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;

namespace DirectTransact.WebApi.PayAT
{
#pragma warning disable

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public const string TRANSMISSIONDATEANDTIME = "yyyy-MM-ddTHH:mm:ss";

    public IConfiguration Configuration { get; }

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

      services.AddMvc().AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.IgnoreNullValues = true;
      });

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "DirectTransact.WebApi.PayAT.STUB.B", Version = "v1" });

        // Load xml file for more
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        try { c.IncludeXmlComments(xmlPath); } catch { }

        c.DescribeAllEnumsAsStrings();
      });

      services.AddLogging(logging =>
      {
        logging.AddConfiguration(Configuration);
        logging.AddConsole();
        logging.AddDebug();
        logging.AddEventSourceLogger();
      });

      services.AddSingleton<IConfiguration>(Configuration);
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
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
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("../swagger/v1/swagger.json", "DirectTransact.WebApi.PayAT.STUB.B");
        c.InjectStylesheet("../css/swagger.min.css");
      });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}