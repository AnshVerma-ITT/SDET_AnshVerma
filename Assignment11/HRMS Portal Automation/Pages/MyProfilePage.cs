using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class MyProfilePage : BasePage
{
    private const string OrganizationText = "Organization";
    private const string MyProfileText = "My profile";
    private const string JobAndSkillsText = "Job & Skills";
    private const string EmploymentText = "Employment";
    private const string ResignationText = "Resignation";
    private const string JobText = "Job";
    private const string WorkSchemeRecordText = "Work Scheme Record";
    private const string SchemeDetailsText = "Scheme Details";
    private const string OrganizationMenuSelector = "xpath=following-sibling::div[1]";
    private const string MenuLinkSelector = "a";
    private const string BreadcrumbSelector = "a.mantine-Text-root.mantine-Anchor-root";
    private const string FieldRowSelector = "xpath=ancestor::div[contains(@class,'mantine-Grid-root')][1]";
    private const string SchemeCardSelector = "xpath=ancestor::div[contains(@class,'mantine-Card-root')][1]";
    private const string SchemeDetailsSelector = "[aria-haspopup='dialog']";
    private const string DateOfApplySelector = "input[name='dateofApply']:visible";
    private const string LastWorkingDaySelector = "input[name='lastWorkingDay']:visible";
    private const string AriaExpandedAttribute = "aria-expanded";
    private const string AriaControlsAttribute = "aria-controls";
    private const string AriaHiddenAttribute = "aria-hidden";
    private const string ExpandedValue = "true";
    private const string VisibleMenuValue = "false";

    public MyProfilePage(IPage page) : base(page) { }

    public ILocator JobAndSkillsTab => Page.GetByRole(AriaRole.Tab,
        new() { Name = JobAndSkillsText, Exact = true });
    public ILocator EmploymentTab => Page.GetByRole(AriaRole.Tab,
        new() { Name = EmploymentText, Exact = true });
    public ILocator ResignationTab => Page.GetByRole(AriaRole.Tab,
        new() { Name = ResignationText, Exact = true });
    public ILocator JobAccordionButton => Page.GetByRole(AriaRole.Button,
        new() { Name = JobText, Exact = true });
    public ILocator WorkSchemeAccordionButton => Page.GetByRole(AriaRole.Button)
        .Filter(new() { HasText = WorkSchemeRecordText }).First;
    public ILocator ResignationPanel => Page.GetByRole(AriaRole.Tabpanel,
        new() { Name = ResignationText, Exact = true });
    public ILocator DateOfApplyInput => ResignationPanel.Locator(DateOfApplySelector);
    public ILocator LastWorkingDateInput => ResignationPanel.Locator(LastWorkingDaySelector);
    public ILocator SchemeDialog => Page.GetByRole(AriaRole.Dialog,
        new() { Name = SchemeDetailsText, Exact = true });

    private ILocator OrganizationButton => Page.GetByRole(AriaRole.Button)
        .Filter(new() { HasText = OrganizationText });
    private ILocator OrganizationMenu => OrganizationButton.Locator(OrganizationMenuSelector);
    private ILocator MyProfileLink => OrganizationMenu.Locator(MenuLinkSelector)
        .Filter(new() { HasText = MyProfileText });
    private ILocator MyProfileBreadcrumb => Page.Locator(BreadcrumbSelector)
        .Filter(new() { HasText = MyProfileText });

    public async Task NavigateToMyProfileAsync()
    {
        if (!await IsExpandedAsync(OrganizationMenu))
            await OrganizationButton.ClickAsync();

        await OrganizationMenu.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await MyProfileLink.ClickAsync();
        await MyProfileBreadcrumb.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public ILocator EmployeeName(string employeeName) =>
        Page.GetByText(employeeName, new() { Exact = true });

    public ILocator TopFieldRow(string label) =>
        Page.GetByText(label, new() { Exact = true }).First.Locator(FieldRowSelector);

    public Task OpenJobAndSkillsAsync() => JobAndSkillsTab.ClickAsync();

    public async Task<ILocator?> ExpandJobSectionAsync()
    {
        await JobAccordionButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        if (!string.Equals(await JobAccordionButton.GetAttributeAsync(AriaExpandedAttribute), ExpandedValue,
                StringComparison.OrdinalIgnoreCase))
        {
            await JobAccordionButton.ClickAsync();
        }

        return await PanelControlledByAsync(JobAccordionButton);
    }

    public async Task<ILocator?> ExpandWorkSchemeAsync()
    {
        await WorkSchemeAccordionButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        if (!string.Equals(await WorkSchemeAccordionButton.GetAttributeAsync(AriaExpandedAttribute), ExpandedValue,
                StringComparison.OrdinalIgnoreCase))
        {
            await WorkSchemeAccordionButton.ClickAsync();
        }

        return await PanelControlledByAsync(WorkSchemeAccordionButton);
    }

    public ILocator CurrentScheme(ILocator workSchemePanel, string schemeName) =>
        workSchemePanel.GetByText(schemeName, new() { Exact = true })
            .Locator(SchemeCardSelector);

    public ILocator SchemeName(ILocator workSchemePanel, string schemeName) =>
        workSchemePanel.GetByText(schemeName, new() { Exact = true });

    public ILocator SchemeDetailsButton(ILocator currentScheme) =>
        currentScheme.Locator(SchemeDetailsSelector);

    public Task OpenSchemeDetailsAsync(ILocator currentScheme) =>
        SchemeDetailsButton(currentScheme).ClickAsync();

    public async Task OpenEmploymentResignationAsync()
    {
        await EmploymentTab.ClickAsync();
        await ResignationTab.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await ResignationTab.ClickAsync();
        await ResignationPanel.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    private async Task<ILocator?> PanelControlledByAsync(ILocator control)
    {
        var panelId = await control.GetAttributeAsync(AriaControlsAttribute);
        return string.IsNullOrWhiteSpace(panelId)
            ? null
            : Page.Locator($"[id='{panelId}']");
    }

    private static async Task<bool> IsExpandedAsync(ILocator menu) =>
        string.Equals(await menu.GetAttributeAsync(AriaHiddenAttribute), VisibleMenuValue,
            StringComparison.OrdinalIgnoreCase)
        && await menu.IsVisibleAsync();
}
