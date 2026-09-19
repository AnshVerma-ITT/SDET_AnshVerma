using HRIntimeAutomation.Context;
using HRIntimeAutomation.TestData;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class FooterSteps
{
    private readonly ScenarioTestContext _context;

    public FooterSteps(ScenarioTestContext context) => _context = context;

    [When("I hover over and open all footer social media links")]
    public async Task WhenIHoverOverAndOpenAllFooterSocialMediaLinks() =>
        _context.FooterLinkResults = await _context.FooterPage
            .OpenSocialLinksAsync(FooterTestData.SocialLinks);

    [Then("all footer social media links should open valid respective pages")]
    public void ThenAllFooterSocialMediaLinksShouldOpenValidRespectivePages()
    {
        Assert.That(_context.FooterLinkResults,
            Has.Count.EqualTo(FooterTestData.SocialLinks.Count));

        Assert.Multiple(() =>
        {
            foreach (var expected in FooterTestData.SocialLinks)
            {
                var actual = _context.FooterLinkResults.Single(result => result.Name == expected.Name);
                Assert.That(actual.MatchingLinkCount, Is.EqualTo(1),
                    $"{expected.Name} should have exactly one footer link.");
                Assert.That(actual.IsVisible, Is.True,
                    $"{expected.Name} footer link was not visible.");
                Assert.That(actual.Href, Is.EqualTo(expected.Href),
                    $"{expected.Name} footer href was incorrect.");
                Assert.That(actual.Target, Is.EqualTo(FooterTestData.NewWindowTarget),
                    $"{expected.Name} footer link should open a new window.");
                Assert.That(actual.OpenedUrl, Is.Not.Null.And.Not.Empty,
                    $"{expected.Name} did not open a URL.");

                if (Uri.TryCreate(actual.OpenedUrl, UriKind.Absolute, out var openedUri))
                {
                    Assert.That(expected.AllowedHosts, Does.Contain(openedUri.Host).IgnoreCase,
                        $"{expected.Name} opened an unexpected host: {openedUri.Host}");
                }
                else
                {
                    Assert.Fail($"{expected.Name} opened an invalid URL: {actual.OpenedUrl}");
                }
            }
        });
    }
}
