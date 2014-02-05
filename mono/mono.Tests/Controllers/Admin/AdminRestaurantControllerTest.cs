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
using System.Net;
using System.Linq.Expressions;

namespace mono.Tests.Controllers
{
    public class AdminRestaurantControllerTest
    {
        private Models.Restaurant restaurant;
        private Models.Restaurant restaurant1, restaurant2, restaurant3, restaurant4, restaurant5, restaurant6;
        private IQueryable<Models.Restaurant> restaurants;

        public AdminRestaurantControllerTest()
        {
            restaurant = new Models.Restaurant();

            restaurant1 = new Models.Restaurant { Name = "restoranMock1", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant2 = new Models.Restaurant { Name = "resafsdsMock2", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant3 = new Models.Restaurant { Name = "resafsdsMock3", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant4 = new Models.Restaurant { Name = "restoranMock4", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant5 = new Models.Restaurant { Name = "restoranMock5", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant6 = new Models.Restaurant { Name = "restoranMock6", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };

            restaurants = new List<Models.Restaurant> { restaurant1, restaurant2, restaurant3, restaurant4, restaurant5, restaurant6 }.AsQueryable();
        }

        public void RestaurantNull(int id)
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            Models.Restaurant restaurant = null;
            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(id)).Returns(restaurant);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Employers(id) as HttpStatusCodeResult;
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        public void ID(int id)
        {
            IDNull();
            RestaurantNull(id);
        }

        [Fact]
        public void RetaurantIndex_SortingAsc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Get(null, null, "")).Returns(restaurants);

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
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Get(null, null, "")).Returns(restaurants);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Index("Name_desc", "restoranMock", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Restaurant>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(restaurant1.Name, model.ElementAt(0).Name);
        }

        public void IDNull()
        {
            var restaurantController = new RestaurantController(new Mock<mono.DAL.UnitOfWork>().Object);

            var result = restaurantController.Employers(null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void RestaurantEmployers()
        {
            ID(6);

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            var restaurant = new Models.Restaurant(){Name = "restaurant"};
            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(restaurant);

            var user1 = new Models.MyUser { FirstName = "zzzz", LastName = "z" };
            var user2 = new Models.MyUser { FirstName = "aaaa", LastName = "a" };
            var users = new List<Models.MyUser>() { user1, user2 }.AsQueryable();
            mockUnitOfWork.Setup(m => m.UserRepository.Get(It.IsAny<Expression<Func<Models.MyUser, bool>>>(), It.IsAny<Func<IQueryable<Models.MyUser>, IOrderedQueryable<Models.MyUser>>>(), "")).Returns(users.OrderBy(u => u.LastName + " " + u.FirstName));

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Employers(6) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.MyUser>;

            Assert.Equal("Employers", result.ViewName);

            Assert.Equal(restaurant.Name, result.ViewBag.Restaurant);

            Assert.Equal(users.Count(), model.Count());
            Assert.Equal(user2.LastName + " " + user2.FirstName, model.ElementAt(0).LastName + " " + model.ElementAt(0).FirstName);
            Assert.Equal(user1.LastName + " " + user1.FirstName, model.ElementAt(1).LastName + " " + model.ElementAt(1).FirstName);
        }

        [Fact]
        public void RestaurantDetails()
        {
            ID(6);

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(restaurant);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            var result = restaurantController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Models.Restaurant;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(restaurant, model);
        }

        [Fact]
        public void RestaurantCreate_Get()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            var result = restaurantController.Create() as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void RestaurantCreate_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();    

            mockUnitOfWork.Setup(m => m.RestaurantRepository.Insert(restaurant));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            var result = restaurantController.Create(restaurant) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void RestaurantCreate_InvalidModel()
        {          
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            
            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            restaurantController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = restaurantController.Create(restaurant) as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void RestaurantCreate_Valid()
        {           
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Insert(restaurant));

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Create(restaurant) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void RestaurantEdit_Get()
        {
            ID(6);

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(restaurant);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Models.Restaurant;

            Assert.Equal("Edit", result.ViewName);           
            Assert.Equal(restaurant, model);
        }

        [Fact]
        public void RestaurantEdit_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            mockUnitOfWork.Setup(m => m.RestaurantRepository.Update(restaurant));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            var result = restaurantController.Edit(restaurant) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void RestaurantEdit_InvalidModel()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            restaurantController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = restaurantController.Edit(restaurant) as ViewResult;

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void RestaurantEdit_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Update(restaurant));         

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Edit(restaurant) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void RestaurantDelete_Get()
        {
            ID(6);

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(restaurant);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Models.Restaurant;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(restaurant, model);
        }

        [Fact]
        public void RestaurantDelete_GetError()
        {
            ID(6);

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(restaurant);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Models.Restaurant;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(restaurant, model);
            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
        }

        [Fact]
        public void RestaurantDeleteConfirmed_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            mockUnitOfWork.Setup(m => m.RestaurantRepository.Delete(6));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            var result = restaurantController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void RestaurantDeleteConfirmed_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Delete(6));

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
