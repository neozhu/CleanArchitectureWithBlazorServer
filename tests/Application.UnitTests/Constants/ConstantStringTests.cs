using CleanArchitecture.Blazor.Application.Constants;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Constants;
public class ConstantStringTests
{
    [Test]
    public void Test()
    {
        Assert.AreEqual("Refresh", ConstantString.Refresh);
        Assert.AreEqual("Edit", ConstantString.Edit);
        Assert.AreEqual("Delete", ConstantString.Delete);
        Assert.AreEqual("Add", ConstantString.Add);
        Assert.AreEqual("Create", ConstantString.New);
        Assert.AreEqual("Export to Excel", ConstantString.Export);
        Assert.AreEqual("Import from Excel", ConstantString.Import);
        Assert.AreEqual("Actions", ConstantString.Actions);
        Assert.AreEqual("Save", ConstantString.Save);
        Assert.AreEqual("Save Changes", ConstantString.SaveChanges);
        Assert.AreEqual("Cancel", ConstantString.Cancel);
        Assert.AreEqual("Close", ConstantString.Close);
        Assert.AreEqual("Search", ConstantString.Search);
        Assert.AreEqual("Clear", ConstantString.Clear);
        Assert.AreEqual("Reset", ConstantString.Reset);
        Assert.AreEqual("OK", ConstantString.Ok);
        Assert.AreEqual("Confirm", ConstantString.Confirm);
        Assert.AreEqual("Yes", ConstantString.Yes);
        Assert.AreEqual("No", ConstantString.No);
        Assert.AreEqual("Next", ConstantString.Next);
        Assert.AreEqual("Previous", ConstantString.Previous);
        Assert.AreEqual("Upload", ConstantString.Upload);
        Assert.AreEqual("Download", ConstantString.Download);
        Assert.AreEqual("Uploading...", ConstantString.Uploading);
        Assert.AreEqual("Downloading...", ConstantString.Downloading);
        Assert.AreEqual("No Allowed", ConstantString.NoAllowed);
        Assert.AreEqual("Sign in with {0}", ConstantString.SigninWith);
        Assert.AreEqual("Logout", ConstantString.Logout);
        Assert.AreEqual("Sign In", ConstantString.Signin);
        Assert.AreEqual("Microsoft", ConstantString.Microsoft);
        Assert.AreEqual("Facebook", ConstantString.Facebook);
        Assert.AreEqual("Google", ConstantString.Google);

    }
}
