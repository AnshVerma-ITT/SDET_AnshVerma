# HRINTIME locator evidence guide

Screenshots show visual state, but the automation also needs the relevant DOM. Do not send credentials, cookies, tokens, or confidential employee values.

For each requested control:

1. Open HRINTIME on the VPN laptop.
2. Press `F12` and use the element picker.
3. Right-click the selected element in the Elements panel.
4. Choose **Copy → Copy outerHTML**.
5. Provide the page name, visible label, current URL, screenshot, and copied HTML.

Prefer HTML containing `id`, `name`, `role`, `aria-label`, `data-testid`, `href`, or a label association. For repeating controls, include the parent container and one child item.

## Evidence still required

| Page/area | Required elements or containers |
| --- | --- |
| Login | Invalid-error container and a unique Dashboard heading/container after successful login |
| Navigation | Navigation root, Organization trigger/submenu, Leave & Attendance trigger/submenu, and each child link |
| Dashboard | Dashboard tab, calendar container, current-day cell, and hover tooltip/popover |
| My Profile | My Profile link, Personal Information container/rows, Job & Skills tab, Job tab/section, Work Scheme Record, Scheme Details, Saturday and Sunday rows |
| Resignation | Employment tab, Resignation tab, Date of Apply value and Last Working Date value |
| Leave Application | Link, Apply Leave button, Leave Type control/options, calendar/dialog/day cell, description field, Submit, success prompt, results table and row |
| Attendance | Link, start/end date controls, calendar day, status control/Week Off option, search/apply control, table and row |
| Leave Correction | Link, Apply Correction, correction type/Work from Home option, date range controls, half-day chip, description, Submit, prompt and record row |
| Employee Directory | Link, Job Title control/Director option, Table View, results container/row, Next, Previous and current-page control |
| Footer | Footer container and the four anchor elements including each `href` |
| Logout | Profile icon/menu, Logout control and the login-page state after logout |

## Business evidence also needed

- Exact invalid-login and successful submission text.
- Expected destination URL or unique heading for every navigation link.
- HRINTIME date format and application timezone.
- Whether resignation uses calendar months and whether weekends/holidays adjust the result.
- Safe dates and cleanup rules for leave and correction submissions.
- Whether Saturday/Sunday are always week-offs for the test account.
