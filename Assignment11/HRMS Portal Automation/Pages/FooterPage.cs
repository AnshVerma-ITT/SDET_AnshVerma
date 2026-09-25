using HRIntimeAutomation.Models;
using HRIntimeAutomation.TestData;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class FooterPage : BasePage
{
    private const string LinkSelectorTemplate = "a[href='{0}']";
    private const string HrefAttribute = "href";
    private const string TargetAttribute = "target";
    private const string AboutUrlPrefix = "about:";

    public FooterPage(IPage page) : base(page) { }

    public async Task<IReadOnlyList<FooterLinkResult>> OpenSocialLinksAsync(
        IEnumerable<SocialLinkData> socialLinks)
    {
        var results = new List<FooterLinkResult>();

        foreach (var socialLink in socialLinks)
        {
            var link = Page.Locator(string.Format(LinkSelectorTemplate, socialLink.Href));
            var linkCount = await link.CountAsync();
            var isVisible = linkCount == 1 && await link.IsVisibleAsync();

            if (!isVisible)
            {
                results.Add(new FooterLinkResult(
                    socialLink.Name,
                    linkCount,
                    isVisible,
                    null,
                    null,
                    null,
                    null));
                continue;
            }

            await link.ScrollIntoViewIfNeededAsync();
            await link.HoverAsync();

            var href = await link.GetAttributeAsync(HrefAttribute);
            var target = await link.GetAttributeAsync(TargetAttribute);
            var popup = await Page.RunAndWaitForPopupAsync(() => link.ClickAsync());
            string? openedUrl;
            string? openedTitle;

            try
            {
                try
                {
                    await popup.WaitForURLAsync(url =>
                        !url.StartsWith(AboutUrlPrefix, StringComparison.OrdinalIgnoreCase));
                    await popup.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                }
                catch (TimeoutException)
                {
                    // External sites can keep trackers loading; capture the final URL below.
                }

                openedUrl = popup.Url;
                openedTitle = await popup.TitleAsync();
            }
            finally
            {
                await popup.CloseAsync();
            }

            results.Add(new FooterLinkResult(
                socialLink.Name,
                linkCount,
                isVisible,
                href,
                target,
                openedUrl,
                openedTitle));
        }

        return results;
    }
}
