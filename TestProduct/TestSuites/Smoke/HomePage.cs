using Microsoft.Playwright;
using Xunit;

namespace TestSuites.Smoke;

public class HomePage
{
    public HomePage()
    {
    }

    public async Task VisitPage()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            WebRootPath = "C:/Users/bjeee/source/repos/TDD/Client/bin/Release/net8.0/publish/wwwroot"
        });
        builder.WebHost.UseUrls("http://127.0.0.1:1234");
        builder.Services.AddDirectoryBrowser();
        using var app = builder.Build();
        app.UseFileServer(new FileServerOptions()
        {
            EnableDirectoryBrowsing = true,
            StaticFileOptions =
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            }
        });
        await app.StartAsync();

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();

        var page = await browser.NewPageAsync();
        var response = await page.GotoAsync(@"http://127.0.0.1:1234/");

        Assert.NotNull(response);
        Assert.True(response!.Ok);

        await Assertions.Expect(page.GetByTestId("test")).ToHaveTextAsync("Hello, world!");
    }
}