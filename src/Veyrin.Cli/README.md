Veyrin.Cli

Veyrin.Cli 提供指令列工具框架，讓你能建立 CLI 指令、參數解析與操作流程。

✨ Features

CLI 指令與子指令系統

自動產生 help / usage

Command handler 架構

支援開發自己的工具

📦 Provides

Command, CommandHandler

CliApp 啟動器

參數解析器 ArgumentParser

📚 Example
var cli = new CliApp();
cli.Register(new BuildCommand());
cli.Run(args);

🔗 Relation

可與 DevKit 整合，提供開發流程工具。