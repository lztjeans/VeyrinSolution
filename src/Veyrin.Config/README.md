Veyrin.Config

Veyrin.Config 提供統一設定管理，支援環境載入、檔案解析與設定驗證。

✨ Features

JSON / YAML / Environment Variables 設定支援

設定物件模型與自動綁定

設定快取與注入

設定覆寫與合併策略

📦 Provides

ConfigLoader

IConfigProvider

ConfigValidator

📚 Example
var config = ConfigLoader.Load("appsettings.json");
var dbConfig = config.GetSection<DatabaseConfig>("database");

🔗 Relation

適用於 Data, Core, Cli 等所有需要設定的模組。