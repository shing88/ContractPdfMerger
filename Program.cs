using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ContractPdfMerger.Infrastructure;
using ContractPdfMerger.Application;
using ContractPdfMerger.UI;

namespace ContractPdfMerger;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.File($"logs/{DateTime.Now:yyyyMMdd}.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 90)
            .CreateLogger();

        try
        {
            Log.Information("Starting Contract PDF Merger application");
            
            System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var host = CreateHostBuilder(configuration).Build();
            
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
                
                // Initialize default data if needed
                SeedDefaultData(context);
            }

            var mainForm = host.Services.GetRequiredService<MainForm>();
            System.Windows.Forms.Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed");
            MessageBox.Show($"アプリケーションの起動に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(IConfiguration configuration) =>
        Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                // Add configuration
                services.AddSingleton(configuration);

                // Database with connection string from configuration
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Services
                services.AddScoped<IPdfMergerService, PdfMergerService>();
                services.AddScoped<IDocumentRepository, DocumentRepository>();
                services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();

                // Forms
                services.AddTransient<MainForm>();
                services.AddTransient<AdminForm>();
            });

    private static void SeedDefaultData(AppDbContext context)
    {
        if (!context.DocumentTypes.Any())
        {
            context.DocumentTypes.AddRange(
                new Domain.DocumentType { TypeCode = "TERMS", TypeName = "利用規約" },
                new Domain.DocumentType { TypeCode = "PRIVACY", TypeName = "プライバシーポリシー" },
                new Domain.DocumentType { TypeCode = "SUPPLEMENT", TypeName = "補足資料" }
            );
            context.SaveChanges();
            Log.Information("Default document types seeded");
        }
    }
}