//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
//if (args.Length == 0)
//{
//    Console.WriteLine("用法：dotnet run -- <你的名字>");
//}
//else
//{
//    Console.WriteLine($"哈囉，{args[0]}！歡迎使用 HelloCli 😄");
//}

// 模擬入口點 Program.cs
using Veyrin.Cli.Commands;
using Veyrin.Cli.Terminal;

var executor = new CommandExecutor();

// 註冊指令: download --url http://... --out ./file.zip --force
executor.Register("download", async (args) =>
{
    string url = args.GetOption("url") ?? throw new Exception("URL is required");
    string output = args.GetOption("out") ?? "download.tmp";
    bool force = args.HasFlag("force");

    Console.WriteLine($"Downloading from {url}...");
    // 這裡可以呼叫之前的 PulseClient 執行下載
});
await executor.ExecuteAsync(args);


////////////////////////////////////////////////////////////////
executor.Register("fetch", async (args) =>
{
    ConsoleWriter.Header("Veyrin Pulse Fetcher");

    using (var spinner = new Spinner())
    {
        // 模擬 Pulse 下載
        await Task.Delay(2000);
        spinner.Stop("Data fetched successfully.");
    }

    ConsoleWriter.Info("Processing result...");
    ConsoleWriter.Success("Mission Accomplished!");
});

//////////////////////////////////////////////////////////////////
// 模擬實體類別
//public class UserReport
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public string Role { get; set; }
//    public string Status { get; set; }
//}

// 在 CLI 指令中使用
//executor.Register("list-users", async (args) =>
//{
//    var users = new List<UserReport>
//    {
//        new() { Id = 1, Name = "Veyrin Admin", Role = "Owner", Status = "Active" },
//        new() { Id = 2, Name = "Guest User", Role = "Viewer", Status = "Pending" },
//        new() { Id = 3, Name = "System Bot", Role = "Worker", Status = "Active" }
//    };

//    TableWriter.Write(users, "System User Report");
//});
//////////////////////////////////////////////////////////////////
