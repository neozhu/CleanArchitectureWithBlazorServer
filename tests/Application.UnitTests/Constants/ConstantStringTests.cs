using CleanArchitecture.Blazor.Infrastructure.Constants;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Constants;

public class ConstantStringTests
{
    [Test]
    public void Test()
    {
        Assert.Equals("Refresh", ConstantString.Refresh);
        Assert.Equals("Edit", ConstantString.Edit);
        Assert.Equals("Delete", ConstantString.Delete);
        Assert.Equals("Add", ConstantString.Add);
        Assert.Equals("New", ConstantString.New);
        Assert.Equals("Export to Excel", ConstantString.Export);
        Assert.Equals("Import from Excel", ConstantString.Import);
        Assert.Equals("Actions", ConstantString.Actions);
        Assert.Equals("Save", ConstantString.Save);
        Assert.Equals("Save Changes", ConstantString.SaveChanges);
        Assert.Equals("Cancel", ConstantString.Cancel);
        Assert.Equals("Close", ConstantString.Close);
        Assert.Equals("Search", ConstantString.Search);
        Assert.Equals("Clear", ConstantString.Clear);
        Assert.Equals("Reset", ConstantString.Reset);
        Assert.Equals("OK", ConstantString.Ok);
        Assert.Equals("Confirm", ConstantString.Confirm);
        Assert.Equals("Yes", ConstantString.Yes);
        Assert.Equals("No", ConstantString.No);
        Assert.Equals("Next", ConstantString.Next);
        Assert.Equals("Previous", ConstantString.Previous);
        Assert.Equals("Upload", ConstantString.Upload);
        Assert.Equals("Download", ConstantString.Download);
        Assert.Equals("Uploading...", ConstantString.Uploading);
        Assert.Equals("Downloading...", ConstantString.Downloading);
        Assert.Equals("No Allowed", ConstantString.NoAllowed);
        Assert.Equals("Sign in with {0}", ConstantString.SigninWith);
        Assert.Equals("Logout", ConstantString.Logout);
        Assert.Equals("Sign In", ConstantString.Signin);
        Assert.Equals("Microsoft", ConstantString.Microsoft);
        Assert.Equals("Facebook", ConstantString.Facebook);
        Assert.Equals("Google", ConstantString.Google);
    }
}