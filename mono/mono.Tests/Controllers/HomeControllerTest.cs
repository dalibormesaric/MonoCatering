using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Xunit;
using mono;
using mono.Controllers;

namespace mono.Tests.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            //Assert.IsNotNull(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            //Assert.IsNotNull(result);
            Assert.NotNull(result);
        }
    }
}
