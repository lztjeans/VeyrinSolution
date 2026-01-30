Veyrin.Pulse

Veyrin.Pulse 用於系統狀態監控、事件、計時與脈動管理，是系統的「心跳層」。

✨ Features

Heartbeat / Health check

Event bus / Signal dispatcher

Runtime metrics

監控 hook

📦 Provides

PulseEngine

EventSignal

IPulseMonitor

📚 Example
PulseEngine.StartHeartbeat(interval: 1000);
PulseEngine.On("system.ready", () => Console.WriteLine("System Ready"));

🔗 Relation

常被 Core 或 Extend 用於事件系統與生命週期管理。