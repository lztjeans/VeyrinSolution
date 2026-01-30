Veyrin.Scribe

Veyrin.Scribe 負責紀錄、日誌、報表與檔案輸出，是系統資訊的「書寫者」。

✨ Features

Logging

Audit / Trace

Report output

檔案寫入與管理工具

📦 Provides

Logger、ILogWriter

AuditRecord

ReportBuilder

📚 Example
var logger = Logger.Create("app.log");
logger.Info("Application started.");

🔗 Relation

可被所有模組使用，也常與 Pulse 整合紀錄事件。