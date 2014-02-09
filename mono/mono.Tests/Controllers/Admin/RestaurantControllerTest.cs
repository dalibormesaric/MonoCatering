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

namespace mono.Tests.Controllers.Admin
{
    public class RestaurantControllerTest
    {
        private Models.Restaurant restaurant;
        private Models.Restaurant restaurant1, restaurant2, restaurant3, restaurant4, restaurant5, restaurant6;
        private IQueryable<Models.Restaurant> restaurants;

        public RestaurantControllerTest()
        {
            restaurant = new Models.Restaurant();

            restaurant1 = new Models.Restaurant { Name = "restoran1", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant2 = new Models.Restaurant { Name = "resafsds2", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant3 = new Models.Restaurant { Name = "resafsds3", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant4 = new Models.Restaurant { Name = "restoran4", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant5 = new Models.Restaurant { Name = "restoran5", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
            restaurant6 = new Models.Restaurant { Name = "restoran6", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };

            restaurants = new List<Models.Restaurant> { restaurant1, restaurant2, restaurant3, restaurant4, restaurant5, restaurant6 }.AsQueryable();
        }

        public void IDNull(string action)
        {
            var restaurantController = new RestaurantController(new Mock<mono.DAL.UnitOfWork>().Object);

            HttpStatusCodeResult result;

            switch(action)
            {
                case "Details":
                    result = restaurantController.Details(null) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    int? value = null;
                    result = restaurantController.Edit(value) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = restaurantController.Delete(null) as HttpStatusCodeResult;
                    break;
                case "DeleteTrue":
                    result = restaurantController.Delete(null, true) as HttpStatusCodeResult;
                    break;
                default: //Employers
                    result = restaurantController.Employers(null) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        public void RestaurantNull(int id, string action)
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            Models.Restaurant restaurant = null;
            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(id)).Returns(restaurant);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Details":
                    result = restaurantController.Details(id) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    result = restaurantController.Edit(id) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = restaurantController.Delete(id) as HttpStatusCodeResult;
                    break;
                case "DeleteTrue":
                    result = restaurantController.Delete(id, true) as HttpStatusCodeResult;
                    break;
                default: //Employers
                    result = restaurantController.Employers(id) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        public void ID(int id, string action)
        {
            IDNull(action);
            RestaurantNull(id, action);
        }

        [Fact]
       public void Index_SortingAsc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Get(null, null, "")).Returns(restaurants);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Index(null, "restoran", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Restaurant>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(restaurant6.Name, model.ElementAt(0).Name);
        }

        [Fact]
       public void Index_SortingDesc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Get(null, null, "")).Returns(restaurants);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Index("Name_desc", "restoran", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Restaurant>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(restaurant1.Name, model.ElementAt(0).Name);
        }

        [Fact]
       public void Employers()
        {
            ID(6, "Employers");

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
       public void Details()
        {
            ID(6, "Details");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(restaurant);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            var result = restaurantController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Models.Restaurant;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(restaurant, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            var result = restaurantController.Create() as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
       public void Create_DataException()
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
       public void Create_InvalidModel()
        {          
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            
            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            restaurantController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = restaurantController.Create(restaurant) as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
       public void Create_Valid()
        {           
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Insert(restaurant));

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Create(restaurant) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
       public void Edit_Get()
        {
            ID(6, "Edit");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(restaurant);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Models.Restaurant;

            Assert.Equal("Edit", result.ViewName);           
            Assert.Equal(restaurant, model);
        }

        [Fact]
       public void Edit_DataException()
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
       public void Edit_InvalidModel()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);
            restaurantController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = restaurantController.Edit(restaurant) as ViewResult;

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
       public void Edit_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Update(restaurant));         

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Edit(restaurant) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
       public void Delete_Get()
        {
            ID(6, "DeleteFalse");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(restaurant);

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Models.Restaurant;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(restaurant, model);
        }

        [Fact]
       public void Delete_GetError()
        {
            ID(6, "DeleteTrue");

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
       public void DeleteConfirmed_DataException()
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
       public void DeleteConfirmed_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Delete(6));

            var restaurantController = new RestaurantController(mockUnitOfWork.Object);

            var result = restaurantController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
