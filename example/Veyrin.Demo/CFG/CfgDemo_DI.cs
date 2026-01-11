/*
using Veyrin.Config;
using Veyrin.Config.Models;
using Veyrin.Config.Providers;
using Veyrin.Demo;
//============Program.cs=========================
var builder = WebApplication.CreateBuilder(args);

// --- 1. 初始化 ConfigHelper ---
string basePath = AppContext.BaseDirectory;
string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

ConfigHelper.Initialize(
    new JsonConfigProvider(basePath, env),
    new EnvironmentVariableProvider()
);

// --- 2. 註冊設定到 DI ---
builder.Services.AddSingleton(ConfigHelper.GetSection<AppSettings>("AppSettings"));
builder.Services.AddSingleton(ConfigHelper.GetSection<DatabaseConfig>("Database"));

// 3. 註冊其他服務
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
*/
//============Controller/Service=========================
/*
using Microsoft.AspNetCore.Mvc;

public class HomeController : ControllerBase
{
    private readonly AppSettings _appSettings;
    private readonly DatabaseConfig _dbConfig;

    public HomeController(AppSettings appSettings, DatabaseConfig dbConfig)
    {
        _appSettings = appSettings;
        _dbConfig = dbConfig;
    }

    [HttpGet("/info")]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            AppName = _appSettings.AppName,
            MaxItems = _appSettings.MaxItems,
            DbHost = _dbConfig.Host,
            DbPort = _dbConfig.Port
        });
    }
}

 
 */