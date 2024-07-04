using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Features;
using LiveMedyAIProject.Services;
using Microsoft.Extensions.Logging;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddHttpClient<IOpenAiService, OpenAiService>();
        services.AddSingleton<PdfToImageConverter>();


       // Register OpenAiVisionClient with API key from configuration using a factory method
        services.AddHttpClient<OpenAiVisionClient>()
                .AddTypedClient((httpClient, sp) => 
                {
                    var apiKey = sp.GetRequiredService<IConfiguration>()["OpenAi:ApiKey"];
                    var logger = sp.GetRequiredService<ILogger<OpenAiVisionClient>>();
                    return new OpenAiVisionClient(httpClient, apiKey, logger);
                });

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 268435456; // 256 MB
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
