using Mono.Areas.Admin.Controllers;
using System;
using System.Web.Mvc;
using Xunit;

namespace Mono.Tests.Controllers.Admin
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
