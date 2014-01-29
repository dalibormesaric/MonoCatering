using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Xunit;
using mono;
using mono.Controllers;
using mono.Areas.Admin.Controllers;
using Moq;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace mono.Tests.Controllers
{
    public class AdminCategoryControllerTest
    {
        [Fact]
        public void CreateCategory()
        {
            /*
            var fakeContext = new Mock<mono.DAL.MonoDbContext>();
            var fakeUnitOfWork = new mono.DAL.UnitOfWork(fakeContext.Object);

            Models.Category category = new Models.Category{ Name = "HranaFake" };

            CategoryController categoryController = new CategoryController(fakeUnitOfWork);

            var result = (RedirectToRouteResult)categoryController.Create(category);

            Assert.Equal(result.RouteValues["action"], "Index");
            fakeContext.Verify(r => r.Categories.Add(category));
            */

            //Assert.Equal(2, 2);

            var mockSet = new Mock<DbSet<Models.Restaurant>>();
            var mockContext = new Mock<mono.DAL.MonoDbContext>();

            mockContext.Setup(m => m.Restaurants).Returns(mockSet.Object);

            var unitOfWork = new mono.DAL.UnitOfWork(mockContext.Object);

            var restaurantController = new RestaurantController(unitOfWork);

            Models.Restaurant restaurant = new Models.Restaurant { Name ="res1", Address = "add1", Description = "opis1", OIB = "2332", Phone = "4322 424" };

            restaurantController.Create(restaurant);

            mockSet.Verify(m => m.Add(It.IsAny<Models.Restaurant>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once()); 

        }
    }
}
