# CrossApp

Наскрізний проєкт з крос-платформного програмування.
Предметна область: Я вибрав - склад (товари, партії, залишки, переміщення)
Призначення: Керування складом, привозка нових партій товарів, їх поповнення. Також переміщення і скільки залишилося товарів.

## Запуск

dotnet build
dotnet build src/Core/Core.csproj
dotnet run --project src/Cli
dotnet run --project src/Cli -- --json

## Середовище

.NET SDK 10.0, Windows 11 x64 / Ubuntu 24.04 x64

# Додаткове завдання

Перед тим потрібно зробити публікації командами - dotnet publish src/Cli -c Release -r win-x64 --self-contained true | dotnet publish src/Cli -c Release -r linux-x64 --self-contained true



**Записую числа** - Eduard@DESKTOP-SVCE6R0 MINGW64 /d/ЛНУ/3\_курс/1\_семестр/Cross/laba/CrossApp (main

$ du -sh src/Cli/bin/Release/net8.0/win-x64/publish/

71M     src/Cli/bin/Release/net8.0/win-x64/publish/



Eduard@DESKTOP-SVCE6R0 MINGW64 /d/ЛНУ/3\_курс/1\_семестр/Cross/laba/CrossApp (main

$ du -sh src/Cli/bin/Release/net8.0/linux-x64/publish/

71M     src/Cli/bin/Release/net8.0/linux-x64/publish/

# Лабораторна робота № 2

## Запуск
dotnet build
dotnet build src/Core/Core.csproj
dotnet run --project src/Cli
dotnet run --project src/Cli -- --json

# Публікація (режим)
1. Self-contained
dotnet publish src/Cli -c Release -r win-x64 --self-contained true - Не потрібен встановлений runtime

# Framework-dependent
2. dotnet publish src/Cli -c Release -r win-x64 --self-contained false - Потрібен встановлений runtime

# Порівнянн рижім публікації:
$ du -sh src/Cli/bin/Release/net10.0/win-x64/publish/
78M     src/Cli/bin/Release/net10.0/win-x64/publish/
$ du -sh src/Cli/bin/Release/net10.0/win-x64/publish/
241K    src/Cli/bin/Release/net10.0/win-x64/publish/

# Запуск з каталогу publish
1. Self-contained
./publish/self-contained/Cli.exe
2.  Framework-dependent
./publish/framework-dependent/Cli.exe