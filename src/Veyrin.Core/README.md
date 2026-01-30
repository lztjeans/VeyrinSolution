Veyrin.Core

Veyrin.Core 是整個 Veyrin 生態系的核心模組，負責提供最基礎的型別、抽象介面、核心服務與跨模組通用功能。

✨ Features

系統核心抽象 (interfaces / base classes)

共同模型與工具

不依賴其他 Veyrin 模組

其他所有模組都可依賴 Core

📦 Provides

IService, IProvider 等核心介面

Domain 與 System 基礎模型

Service orchestration 工具

通用 Utility 與內部小型工具集

📚 Example
var service = new CoreService();
service.Initialize();
service.Run();

🔗 Relation

其他模組像 Config、Data、Pulse 都會在這層之上延伸功能。