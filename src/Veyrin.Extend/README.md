Veyrin.Extend

Veyrin.Extend 是插件與擴充系統，用於建立可插拔式功能、第三方整合與模組擴展點。

✨ Features

Plugin / Extension Lifecycle

Extension point 註冊與管理

Module discovery

第三方系統整合

📦 Provides

IExtension

PluginManager

ExtensionContext

📚 Example
pluginManager.Load("plugins/");
pluginManager.InitializeAll();

🔗 Relation

承接 Core 並擴充至 Pulse、Scribe 等其他功能。