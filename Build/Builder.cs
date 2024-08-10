using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Build;

public class Builder
{
    const string TestProject = "TestProduct";

    public static async Task Run()
    {
        Console.OutputEncoding = Encoding.UTF8;

        await CheckDotNetVersion();
        await CheckWasmToolsVersion();
        await CheckGitVersion();
        await CheckNpmVersion();
        await RestoreNugetPackages();
        await CheckForPlaywrightInstallation();
        await RunPublish("Client");
        await RunTestWatch();
    }

    private static async Task CheckNpmVersion()
    {
        var process = new Process()
        {
            StartInfo = new()
            {
                FileName = "cmd.exe",
                Arguments = $"/c npm --version",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();

        var actualVersion = output.Trim();
        var expectedVersion = "10.4.0";
        if (expectedVersion != actualVersion)
            throw new Exception($"Expected npm version: '{expectedVersion}', actual: '{actualVersion}'.");
    }

    private async static Task CheckForPlaywrightInstallation()
    {
        var playwrightBrowserPath = FluentPath.From(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
            .Combine("ms-playwright")
            .GetFullPath()
            .Build();

        if (!Directory.Exists(playwrightBrowserPath))
        {
            var testProjectBinPath = FluentPath.From(GetCurrentSourceFilePath())
                .Combine("..", TestProject, "bin", "debug")
                .GetFullPath()
                .Build();

            if (!File.Exists(testProjectBinPath))
            {
                await RunBuild(TestProject);
            }

            var targetFramework = new DirectoryInfo(testProjectBinPath)
                .GetDirectories()
                .Select(d => d.Name)
                .OrderDescending()
                .First();

            var playwrightSetupPath = FluentPath.From(testProjectBinPath)
                .Combine(targetFramework, "playwright.ps1")
                .Build();

            if (!File.Exists(playwrightSetupPath))
            {
                throw new Exception($"{playwrightSetupPath} missing.");
            }

            throw new Exception($"Playwright browsers not installed at '{playwrightBrowserPath}'.\nPlease run {playwrightSetupPath} to install.");
        }
    }

    static string GetCurrentSourceFilePath([CallerFilePath] string filePath = "") => Path.GetDirectoryName(filePath)!;

    private static async Task CheckGitVersion()
    {
        var process = new Process()
        {
            StartInfo = new()
            {
                FileName = "git",
                Arguments = "--version",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();

        var actualVersion = output.Split(" ").Last().Trim();
        var expectedVersion = "2.41.0.windows.3";
        if (expectedVersion != actualVersion)
            throw new Exception($"Expected git version: {expectedVersion}, actual: {actualVersion}.");
    }

    private static async Task CheckDotNetVersion()
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
        var expectedVersion = new Version("8.0.303");
        if (expectedVersion != actualVersion)
            throw new Exception($"Expected dotnet version: {expectedVersion}, actual: {actualVersion}.");
    }

    private static async Task CheckWasmToolsVersion()
    {
        var process = new Process()
        {
            StartInfo = new()
            {
                FileName = "dotnet",
                Arguments = " workload list",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            }
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();

        var row = output
            .Split('\n')
            .Where(l => l.StartsWith("wasm-tools"))
            .FirstOrDefault() ?? throw new Exception($"No wasm-tools installed.");

        var actualVersion = Regex.Match(row, @"wasm-tools\s+([\d\./]+)\s+").Groups
            .Cast<Group>()
            .Skip(1)
            .Select(g => g.Value)
            .FirstOrDefault() ?? throw new Exception($"No wasm-tools version match found.");

        var expectedVersion = "8.0.7/8.0.100";
        if (expectedVersion != actualVersion)
            throw new Exception($"Expected wasm-tools version: {expectedVersion}, actual: {actualVersion}.");
    }

    private static async Task RestoreNugetPackages()
    {
        var process = new Process()
        {
            StartInfo = new()
            {
                FileName = "dotnet",
                Arguments = @"restore .\AllProjects.sln --use-lock-file",
                CreateNoWindow = true,
            }
        };

        process.Start();
        await process.WaitForExitAsync();
    }

    private static async Task RunTestWatch()
    {
        WriteLine("⏱  Starting test watch⏱", ConsoleColor.Yellow);

        var process = new Process()
        {
            EnableRaisingEvents = true,
            StartInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"watch run --project {TestProject} --quiet --non-interactive",
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

            var output = e?.Data ?? string.Empty;
            var allOk = output.StartsWith("✅ All tests OK ✅");
            var failedTest = output.StartsWith("❌");
            var buildError = new Regex(@": error \w+: ").IsMatch(output);
            var buildWarning = new Regex(@": warning \w+: ").IsMatch(output);

            var color = allOk
                ? ConsoleColor.Green
                : buildWarning
                ? ConsoleColor.Yellow
                : failedTest | buildError
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

    private static async Task RunBuild(string project)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = "dotnet",
            Arguments = $"build {project}/ --verbosity quiet",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        var process = new Process()
        {
            EnableRaisingEvents = true,
            StartInfo = psi,
        };

        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            WriteLine(e?.Data, ConsoleColor.Red);
        });

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        await process.WaitForExitAsync();
    }

    private static async Task RunPublish(string project)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = "dotnet",
            Arguments = $"publish {project}/ --verbosity quiet",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        var process = new Process()
        {
            EnableRaisingEvents = true,
            StartInfo = psi,
        };

        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
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
