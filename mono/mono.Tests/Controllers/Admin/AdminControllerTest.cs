using mono.Areas.Admin.Controllers;
using System;
using System.Web.Mvc;
using Xunit;

namespace mono.Tests.Controllers.Admin
{
    public class AdminControllerTest
    {
        [Fact]
        public void Index()
        {
            var adminController = new AdminController();

            var result = adminController.Index() as ViewResult;

            Assert.Equal("Index", result.ViewName);
        }
    }
}
