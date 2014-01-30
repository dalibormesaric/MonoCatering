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

using System.Data;

namespace mono.Tests.Controllers
{
    public class AdminRestaurantControllerTest
    {
        [Fact]
        public void RetaurantIndex_SortingAsc_Filter_PerPage_Page()
        {
            var restaurant1 = new Models.Restaurant { Name = "restoranMock1", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant2 = new Models.Restaurant { Name = "resafsdsMock2", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant3 = new Models.Restaurant { Name = "resafsdsMock3", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant4 = new Models.Restaurant { Name = "restoranMock4", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant5 = new Models.Restaurant { Name = "restoranMock5", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant6 = new Models.Restaurant { Name = "restoranMock6", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };

            var data = new List<Models.Restaurant> { restaurant1, restaurant2, restaurant3, restaurant4, restaurant5, restaurant6 }.AsQueryable();

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Get(null, null, "")).Returns(data);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Index(null, "restoranMock", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Restaurant>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(restaurant6.Name, model.ElementAt(0).Name);
        }

        [Fact]
        public void RetaurantIndex_SortingDesc_Filter_PerPage_Page()
        {
            var restaurant1 = new Models.Restaurant { Name = "restoranMock1", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant2 = new Models.Restaurant { Name = "resafsdsMock2", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant3 = new Models.Restaurant { Name = "resafsdsMock3", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant4 = new Models.Restaurant { Name = "restoranMock4", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant5 = new Models.Restaurant { Name = "restoranMock5", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            var restaurant6 = new Models.Restaurant { Name = "restoranMock6", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };

            var data = new List<Models.Restaurant> { restaurant1, restaurant2, restaurant3, restaurant4, restaurant5, restaurant6 }.AsQueryable();

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Get(null, null, "")).Returns(data);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Index("Name_desc", "restoranMock", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Restaurant>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(restaurant1.Name, model.ElementAt(0).Name);
        }

        [Fact]
        public void RestaurantCreate_DataException()
        {
            Models.Restaurant restaurant = new Models.Restaurant();

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();     
            var mockRestaurantRepository = new Mock<mono.DAL.GenericRepository<mono.Models.Restaurant>>();

            mockUnitOfWork.Setup(m => m.RestaurantRepository).Returns(mockRestaurantRepository.Object);
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            var result = restaurantController.Create(restaurant) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void RestaurantCreate_InvalidModel()
        {
            Models.Restaurant restaurant = new Models.Restaurant();
            
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            var mockRestaurantRepository = new Mock<mono.DAL.GenericRepository<mono.Models.Restaurant>>();

            mockUnitOfWork.Setup(m => m.RestaurantRepository).Returns(mockRestaurantRepository.Object);
            
            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            restaurantController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = restaurantController.Create(restaurant) as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void RestaurantCreate_Valid()
        {
            Models.Restaurant restaurant = new Models.Restaurant();
            
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            var mockRestaurantRepository = new Mock<mono.DAL.GenericRepository<mono.Models.Restaurant>>();

            mockUnitOfWork.Setup(m => m.RestaurantRepository).Returns(mockRestaurantRepository.Object);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Create(restaurant) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }
    }
}
