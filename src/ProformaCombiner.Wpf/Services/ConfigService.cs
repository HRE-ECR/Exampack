using Microsoft.Extensions.Configuration;

namespace ProformaCombiner.Wpf.Services;

public static class ConfigService
{
    public static AppConfig Load()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

        var root = builder.Build();

        return new AppConfig
        {
            AT200ExcelPath = root["AT200ExcelPath"] ?? string.Empty,
            AT300ExcelPath = root["AT300ExcelPath"] ?? string.Empty,
            SheetName = root["SheetName"] ?? string.Empty
        };
    }
}
