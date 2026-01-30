Veyrin.Data

Veyrin.Data 提供資料層抽象，包括資料模型、repository、資料來源與序列化工具。

✨ Features

資料模型（Entities, DTOs）

Repository pattern

支援多種資料來源（DB、File、Memory）

資料序列化 / 反序列化工具

📦 Provides

IDataRepository

Entity Base Types

DataContext

DataSerializer

📚 Example
var repo = new FileDataRepository("data.json");
var users = repo.Query<User>().ToList();

🔗 Relation

高度依賴 Config；通常會被 Core 或 Extend 使用。