using PlaywrightApiFramework.ReqRes.Models;

namespace PlaywrightApiFramework.Tests.DataProviders;

public class UserScenarioData
{
    public int PageOne { get; set; }
    public int PageTwo { get; set; }
    public int ExistingUserId { get; set; }
    public int SecondExistingUserId { get; set; }
    public int MissingUserId { get; set; }
    public UserRequest UpdateUser { get; set; } = new();
    public UserRequest PatchUser { get; set; } = new();
    public UserRequest JsonContentUser { get; set; } = new();
    public UserRequest PersistenceUser { get; set; } = new();
    public string XmlBody { get; set; } = "";
    public Dictionary<string, string> FormDataUser { get; set; } = new();
    public string RawTextBody { get; set; } = "";
    public string RegisterEmailWithoutPassword { get; set; } = "";
}
