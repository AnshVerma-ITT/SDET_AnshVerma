namespace HRIntimeAutomation.TestData;

public sealed record ExpectedField(string Label, string Value);

public static class ProfileTestData
{
    public const string EmployeeName = "Dhruv Gupta";
    public const string AvatarInitials = "DG";
    public const string CurrentSchemeName = "Default";
    public const string CurrentSchemeStatus = "Present";
    public const string SaturdayWeekOffPattern = @"Saturday\s+Week-off";
    public const string SundayWeekOffPattern = @"Sunday\s+Week-off";

    public static readonly IReadOnlyList<ExpectedField> PersonalInformation =
    [
        new("Email", "dhruv.g@intimetec.in..."),
        new("Gender", "Male"),
        new("Father's Name", "Jai Prakash Gupta"),
        new("Nationality", "India"),
        new("Employee ID", "ITTV/EMP/2668"),
        new("Date Of Birth", "02-Nov-2000"),
        new("Mobile No.", "7221949173"),
        new("Location", "Jaipur")
    ];

    public static readonly IReadOnlyList<ExpectedField> JobInformation =
    [
        new("Job Title", "Software Engineer"),
        new("Department", "Software Development"),
        new("Employment Status", "Active"),
        new("Trainee Joined Date", "2022-02-01"),
        new("Employee Joined Date", "01-Jul-2022")
    ];
}
