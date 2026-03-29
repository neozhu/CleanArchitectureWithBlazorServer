using CleanArchitecture.Blazor.Infrastructure.Constants;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Constants;

public class ConstantStringTests
{
    [Test]
    public void Test()
    {
        Assert.AreEqual(ConstantString.Localize("Refresh"), ConstantString.Refresh);
        Assert.AreEqual(ConstantString.Localize("Edit"), ConstantString.Edit);
        Assert.AreEqual(ConstantString.Localize("Delete"), ConstantString.Delete);
        Assert.AreEqual(ConstantString.Localize("Add"), ConstantString.Add);
        Assert.AreEqual(ConstantString.Localize("New"), ConstantString.New);
        Assert.AreEqual(ConstantString.Localize("Export to Excel"), ConstantString.Export);
        Assert.AreEqual(ConstantString.Localize("Import from Excel"), ConstantString.Import);
        Assert.AreEqual(ConstantString.Localize("Actions"), ConstantString.Actions);
        Assert.AreEqual(ConstantString.Localize("Save"), ConstantString.Save);
        Assert.AreEqual(ConstantString.Localize("Save Changes"), ConstantString.SaveChanges);
        Assert.AreEqual(ConstantString.Localize("Cancel"), ConstantString.Cancel);
        Assert.AreEqual(ConstantString.Localize("Close"), ConstantString.Close);
        Assert.AreEqual(ConstantString.Localize("Search"), ConstantString.Search);
        Assert.AreEqual(ConstantString.Localize("Clear"), ConstantString.Clear);
        Assert.AreEqual(ConstantString.Localize("Reset"), ConstantString.Reset);
        Assert.AreEqual(ConstantString.Localize("OK"), ConstantString.Ok);
        Assert.AreEqual(ConstantString.Localize("Confirm"), ConstantString.Confirm);
        Assert.AreEqual(ConstantString.Localize("Yes"), ConstantString.Yes);
        Assert.AreEqual(ConstantString.Localize("No"), ConstantString.No);
        Assert.AreEqual(ConstantString.Localize("Next"), ConstantString.Next);
        Assert.AreEqual(ConstantString.Localize("Previous"), ConstantString.Previous);
        Assert.AreEqual(ConstantString.Localize("Upload"), ConstantString.Upload);
        Assert.AreEqual(ConstantString.Localize("Download"), ConstantString.Download);
        Assert.AreEqual(ConstantString.Localize("Uploading..."), ConstantString.Uploading);
        Assert.AreEqual(ConstantString.Localize("Downloading..."), ConstantString.Downloading);
        Assert.AreEqual(ConstantString.Localize("No Allowed"), ConstantString.NoAllowed);
        Assert.AreEqual(ConstantString.Localize("Sign in with {0}"), ConstantString.SigninWith);
        Assert.AreEqual(ConstantString.Localize("Logout"), ConstantString.Logout);
        Assert.AreEqual(ConstantString.Localize("Sign In"), ConstantString.Signin);
        Assert.AreEqual(ConstantString.Localize("Microsoft"), ConstantString.Microsoft);
        Assert.AreEqual(ConstantString.Localize("Facebook"), ConstantString.Facebook);
        Assert.AreEqual(ConstantString.Localize("Google"), ConstantString.Google);
    }
}
