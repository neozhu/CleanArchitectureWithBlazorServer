using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Constants;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Constants;
public class ButtonTextTests
{
    [Test]
    public void Test()
    {
        Assert.AreEqual("Refresh", ButtonText.REFRESH);
        Assert.AreEqual("Edit", ButtonText.EDIT);
        Assert.AreEqual("Delete", ButtonText.DELETE);
        Assert.AreEqual("Add", ButtonText.ADD);
        Assert.AreEqual("Create", ButtonText.CREATE);
        Assert.AreEqual("Export to Excel", ButtonText.EXPORT);
        Assert.AreEqual("Import from Excel", ButtonText.IMPORT);
        Assert.AreEqual("Actions", ButtonText.ACTIONS);
        Assert.AreEqual("Save", ButtonText.SAVE);
        Assert.AreEqual("Save Changes", ButtonText.SAVECHANGES);
        Assert.AreEqual("Cancel", ButtonText.CANCEL);
        Assert.AreEqual("Close", ButtonText.CLOSE);
        Assert.AreEqual("Search", ButtonText.SEARCH);
        Assert.AreEqual("Clear", ButtonText.CLEAR);
        Assert.AreEqual("Reset", ButtonText.RESET);
        Assert.AreEqual("OK", ButtonText.OK);
        Assert.AreEqual("Confirm", ButtonText.CONFIRM);
        Assert.AreEqual("Yes", ButtonText.YES);
        Assert.AreEqual("No", ButtonText.NO);
        Assert.AreEqual("Next", ButtonText.NEXT);
        Assert.AreEqual("Previous", ButtonText.PREVIOUS);
        Assert.AreEqual("Upload", ButtonText.UPLOAD);
        Assert.AreEqual("Download", ButtonText.DOWNLOAD);
        Assert.AreEqual("Uploading...", ButtonText.UPLOADING);
        Assert.AreEqual("Downloading...", ButtonText.DOWNLOADING);
        Assert.AreEqual("No Allowed", ButtonText.NOALLOWED);
        Assert.AreEqual("Sign in with {0}", ButtonText.SIGNINWITH);
        Assert.AreEqual("Logout", ButtonText.LOGOUT);
        Assert.AreEqual("Sign In", ButtonText.SIGNIN);
        Assert.AreEqual("Microsoft", ButtonText.Microsoft);
        Assert.AreEqual("Facebook", ButtonText.Facebook);
        Assert.AreEqual("Google", ButtonText.Google);

    }
}
