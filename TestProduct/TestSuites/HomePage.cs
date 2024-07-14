using Microsoft.Playwright;
using Xunit;

namespace TestSuites;

public class HomePage
{
    public HomePage()
    {
    }

    public async Task VisitPage()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();

        var page = await browser.NewPageAsync();
        var response = await page.GotoAsync("https://google.se");

        Assert.NotNull(response);
        Assert.True(response!.Ok);
    }
}