using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Constants;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Constants;
public class ConstantStringTests
{
    [Test]
    public void Test()
    {
        Assert.AreEqual("Refresh", ConstantString.REFRESH);
        Assert.AreEqual("Edit", ConstantString.EDIT);
        Assert.AreEqual("Delete", ConstantString.DELETE);
        Assert.AreEqual("Add", ConstantString.ADD);
        Assert.AreEqual("Create", ConstantString.NEW);
        Assert.AreEqual("Export to Excel", ConstantString.EXPORT);
        Assert.AreEqual("Import from Excel", ConstantString.IMPORT);
        Assert.AreEqual("Actions", ConstantString.ACTIONS);
        Assert.AreEqual("Save", ConstantString.SAVE);
        Assert.AreEqual("Save Changes", ConstantString.SAVECHANGES);
        Assert.AreEqual("Cancel", ConstantString.CANCEL);
        Assert.AreEqual("Close", ConstantString.CLOSE);
        Assert.AreEqual("Search", ConstantString.SEARCH);
        Assert.AreEqual("Clear", ConstantString.CLEAR);
        Assert.AreEqual("Reset", ConstantString.RESET);
        Assert.AreEqual("OK", ConstantString.OK);
        Assert.AreEqual("Confirm", ConstantString.CONFIRM);
        Assert.AreEqual("Yes", ConstantString.YES);
        Assert.AreEqual("No", ConstantString.NO);
        Assert.AreEqual("Next", ConstantString.NEXT);
        Assert.AreEqual("Previous", ConstantString.PREVIOUS);
        Assert.AreEqual("Upload", ConstantString.UPLOAD);
        Assert.AreEqual("Download", ConstantString.DOWNLOAD);
        Assert.AreEqual("Uploading...", ConstantString.UPLOADING);
        Assert.AreEqual("Downloading...", ConstantString.DOWNLOADING);
        Assert.AreEqual("No Allowed", ConstantString.NOALLOWED);
        Assert.AreEqual("Sign in with {0}", ConstantString.SIGNINWITH);
        Assert.AreEqual("Logout", ConstantString.LOGOUT);
        Assert.AreEqual("Sign In", ConstantString.SIGNIN);
        Assert.AreEqual("Microsoft", ConstantString.Microsoft);
        Assert.AreEqual("Facebook", ConstantString.Facebook);
        Assert.AreEqual("Google", ConstantString.Google);

    }
}
