using System.Diagnostics;
using System.Text;

namespace Build;

public class Builder
{
    public static async Task Run(string testProject)
    {
        Console.OutputEncoding = Encoding.UTF8;

        await CheckVersion();
        await RunTestWatch(testProject);
    }

    private static async Task CheckVersion()
    {
        var process = new Process()
        {
            StartInfo = new()
            {
                FileName = "dotnet",
                Arguments = "--version",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();

        var actualVersion = Version.Parse(output);
        var expectedVersion = new Version("7.0.401");
        if (expectedVersion != actualVersion)
            throw new Exception($"Expected dotnet version: {expectedVersion}, actual: {actualVersion}");
    }

    private static async Task RunTestWatch(string testProject)
    {
        WriteLine("⏱  Starting test watch⏱", ConsoleColor.Yellow);

        var process = new Process()
        {
            EnableRaisingEvents = true,
            StartInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"watch run --project {testProject} --quiet --non-interactive",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            }
        };

        process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            var dotnetWatchSpam = e?.Data?.StartsWith("dotnet watch") ?? false;
            if (dotnetWatchSpam) return;

            var allOk = e?.Data?.StartsWith("✅ All tests OK ✅") ?? false;
            var failedTest = e?.Data?.StartsWith("❌") ?? false;

            var color = allOk
                ? ConsoleColor.Green
                : failedTest
                ? ConsoleColor.Red
                : (ConsoleColor?)null;

            WriteLine(e?.Data, color);
        });

        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            var dotnetWatchSpam = e?.Data?.StartsWith("dotnet watch") ?? false;

            if (!dotnetWatchSpam)
                WriteLine(e?.Data, ConsoleColor.Red);
        });

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        await process.WaitForExitAsync();
    }

    private static void WriteLine(string? message, ConsoleColor? color = null)
    {
        Console.ForegroundColor = color ?? ConsoleColor.Gray;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
