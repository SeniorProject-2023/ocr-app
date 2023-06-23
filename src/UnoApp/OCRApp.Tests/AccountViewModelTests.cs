using System;
using System.Threading.Tasks;
using NUnit.Framework;
using OCRApp.Presentation;
using OCRApp.Tests.Infrastructure;

namespace OCRApp.Tests;

public class AccountViewModelTests
{
    [Test]
    public async Task TestLoginSuccessShouldUpdateLoggedInUsername()
    {
        var mockService = new MockOCRService()
        {
            ExpectedLoginOutcome = ExpectedLoginOutcome.Success,
        };

        var vm = new AccountViewModel(mockService);
        Assert.IsNull(vm.LoggedInUsername);
        var loginResult = await vm.LoginAsync("youssef", "123456");
        Assert.IsTrue(loginResult);
        Assert.AreEqual("youssef", vm.LoggedInUsername);
    }

    [Test]
    public async Task TestLoginFailureShouldNotUpdateLoggedInUsername()
    {
        var mockService = new MockOCRService()
        {
            ExpectedLoginOutcome = ExpectedLoginOutcome.Failure,
        };

        var vm = new AccountViewModel(mockService);
        Assert.IsNull(vm.LoggedInUsername);
        var loginResult = await vm.LoginAsync("youssef", "123456");
        Assert.IsFalse(loginResult);
        Assert.IsNull(vm.LoggedInUsername);
    }

    [Test]
    public void TestLoginCrash()
    {
        var mockService = new MockOCRService()
        {
            ExpectedLoginOutcome = ExpectedLoginOutcome.Exception,
        };

        var vm = new AccountViewModel(mockService);
        Assert.IsNull(vm.LoggedInUsername);
        // TODO: Instead of that behavior, we could have an Error property on the view model which surfaces exceptions.
        //       This could be better than surfacing the exception to the view model callers.
        Assert.ThrowsAsync<Exception>(() => vm.LoginAsync("youssef", "123456"));
        Assert.IsNull(vm.LoggedInUsername);
    }
}