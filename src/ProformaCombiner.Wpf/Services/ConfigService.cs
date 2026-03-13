using Microsoft.Extensions.Configuration;

namespace ProformaCombiner.Wpf.Services;

public static class ConfigService
{
    public static AppConfig Load()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

        var configRoot = builder.Build();

        return new AppConfig
        {
            AT200ExcelPath = configRoot["AT200ExcelPath"] ?? string.Empty,
            AT300ExcelPath = configRoot["AT300ExcelPath"] ?? string.Empty,
            SheetName = configRoot["SheetName"] ?? string.Empty
        };
    }
}
