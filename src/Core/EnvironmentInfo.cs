using System.Runtime.InteropServices;

namespace Core;

/// <summary>
/// Клас для збору інформації про середовище.
/// Статичний клас, бо містить лише алгоритм (поведінку), а не дані.
/// </summary>
public static class EnvironmentInfo
{
    /// <summary>
    /// Збирає інформацію про середовище та повертає її як EnvironmentReport.
    /// НЕ друкує нічого — це робота CLI.
    /// </summary>
    public static EnvironmentReport Collect() => new(
        RuntimeInformation.OSDescription,
        RuntimeInformation.FrameworkDescription,
        RuntimeInformation.ProcessArchitecture.ToString(),
        DetectRid(),
        RuntimeInformation.RuntimeIdentifier,
        AppContext.BaseDirectory,
        Environment.Version.ToString(),
        Environment.CurrentDirectory
    );

    /// <summary>
    /// Ручне визначення RID: показує, з чого складається рядок win-x64.
    /// </summary>
    private static string DetectRid()
    {
        string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win"
                  : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux"
                  : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx"
                  : "unknown";

        string arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            Architecture.Arm => "arm",
            _ => "unknown"
        };

        return $"{os}-{arch}";
    }
}