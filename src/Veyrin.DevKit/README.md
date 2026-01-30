Veyrin.DevKit

Veyrin.DevKit 是給開發者使用的工具包，提供 scaffolding、debug、mock、加速開發的輔助工具。

✨ Features

自動建立專案/模組的 scaffolding

Mock 工具

測試輔助

自動化 script

📦 Provides

ProjectScaffolder

DevMock

File generation utilities

Debug helper

📚 Example
var project = ProjectScaffolder.CreateModule("MyModule");
project.Generate();

🔗 Relation

適合與 Cli 搭配建立 command scaffold 工具。