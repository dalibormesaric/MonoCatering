using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Xunit;
using Mono;
using Mono.Controllers;
using Mono.Areas.Admin.Controllers;
using Moq;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

using System.Data;
using System.Net;
using System.Linq.Expressions;

using Mono.Data;
using Mono.Model;
using Mono.Tests.Admin.Fake;

namespace Mono.Tests.Admin.Controllers
{
    public class RestaurantControllerTest
    {
        [Fact]
        public void Index()
        {
            //ControllerHelper.newSearchPageNumber

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.Get(It.IsAny<Expression<Func<Restaurant, bool>>>(), It.IsAny<Func<IQueryable<Restaurant>, IOrderedQueryable<Restaurant>>>(), It.IsAny<String>())).Returns(RestaurantFake.restaurants);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Index("", "", "", 1) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Restaurant>;

            //ViewBag.

            Assert.Equal("Index", result.ViewName);
            Assert.Equal(RestaurantFake.restaurantsPagedList, model);
        }

        [Fact]
        public void Employers_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(It.IsAny<int>())).Returns(RestaurantFake.restaurantNull);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Employers(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Employers()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(RestaurantFake.restaurant);
            mockIUnitOfWork.Setup(m => m.UserRepository.Get(It.IsAny<Expression<Func<MyUser, bool>>>(), It.IsAny<Func<IQueryable<MyUser>, IOrderedQueryable<MyUser>>>(), "")).Returns(RestaurantFake.users);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Employers(6) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<MyUser>;

            Assert.Equal(RestaurantFake.restaurant.Name, result.ViewBag.Restaurant);
            Assert.Equal("Employers", result.ViewName);
            Assert.Equal(RestaurantFake.users, model);
        }

        [Fact]
        public void Details_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(It.IsAny<int>())).Returns(RestaurantFake.restaurantNull);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Details(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Details()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(RestaurantFake.restaurant);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Restaurant;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(RestaurantFake.restaurant, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Create() as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.Insert(RestaurantFake.restaurant));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Create(RestaurantFake.restaurant) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            restaurantController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = restaurantController.Create(RestaurantFake.restaurant) as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.Insert(RestaurantFake.restaurant));

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Create(RestaurantFake.restaurant) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(It.IsAny<int>())).Returns(RestaurantFake.restaurantNull);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Edit(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(RestaurantFake.restaurant);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Restaurant;

            Assert.Equal("Edit", result.ViewName);
            Assert.Equal(RestaurantFake.restaurant, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.Update(RestaurantFake.restaurant));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Edit(RestaurantFake.restaurant) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            restaurantController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = restaurantController.Edit(RestaurantFake.restaurant) as ViewResult;

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.Update(RestaurantFake.restaurant));

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Edit(RestaurantFake.restaurant) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(It.IsAny<int>())).Returns(RestaurantFake.restaurantNull);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Delete(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Delete_Get_ErrorMessage()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(RestaurantFake.restaurant);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Restaurant;

            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(RestaurantFake.restaurant, model);
        }

        [Fact]
        public void Delete_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.GetByID(6)).Returns(RestaurantFake.restaurant);

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Restaurant;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(RestaurantFake.restaurant, model);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.Delete(6));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.Delete(6));

            var restaurantController = new RestaurantController(mockIUnitOfWork.Object);
            var result = restaurantController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
