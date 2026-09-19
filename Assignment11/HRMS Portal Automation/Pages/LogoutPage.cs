using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class LogoutPage : BasePage
{
    private const string LogoutText = "Logout";

    public LogoutPage(IPage page) : base(page) { }

    private ILocator LogoutLink => Page.GetByText(LogoutText, new() { Exact = true }).First;

    public async Task LogoutAsync(string profileAvatarText)
    {
        var profileAvatar = Page.GetByText(profileAvatarText, new() { Exact = true }).First;
        await profileAvatar.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await profileAvatar.ClickAsync();

        await LogoutLink.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await LogoutLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
