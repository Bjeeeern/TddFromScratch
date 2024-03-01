using System.Diagnostics;
using System.Text;

namespace Build;

public class Runner
{
    public static async Task Run()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("⏱  Starting test watch⏱");
        Console.ResetColor();

        var process = new Process()
        {
            EnableRaisingEvents = true,
            StartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = "watch run --project Test --quiet --non-interactive",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            }
        };

        process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
        process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        await process.WaitForExitAsync();
    }

    private static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        var dotnetWatchSpam = e?.Data?.StartsWith("dotnet watch") ?? false;
        var allOk = e?.Data?.StartsWith("✅ All tests OK ✅") ?? false;

        if (allOk)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(e?.Data);
            Console.ResetColor();
        }
        else if (!dotnetWatchSpam)
            Console.WriteLine(e?.Data);
    }

    private static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        var dotnetWatchSpam = e?.Data?.StartsWith("dotnet watch") ?? false;

        if (!dotnetWatchSpam)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e?.Data);
            Console.ResetColor();
        }
    }
}
