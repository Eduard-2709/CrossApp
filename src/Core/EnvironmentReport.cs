namespace Core;

/// <summary>
/// Record для зберігання інформації про середовище виконання.
/// Record обрано тому, що це незмінні дані (DTO), а не сутність з поведінкою.
/// </summary>
public sealed record EnvironmentReport(
    string OsDescription,
    string FrameworkDescription,
    string ProcessArchitecture,
    string DetectedRid,
    string ReportedRid,
    string BaseDirectory,
    string ClrVersion,
    string CurrentDirectory
);