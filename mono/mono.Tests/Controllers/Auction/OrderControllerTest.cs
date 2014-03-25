using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using Mono.Areas.Auction.Controllers;
using System.Web.Mvc;
using System.Net;
using System.Data;
using System.Linq.Expressions;
using Mono.Model;
using Mono.Areas.Auction.Models;
using Mono.Data;
using System.Web;
using System.Web.Routing;

namespace Mono.Tests.Controllers.Auction
{
    public class OrderControllerTest
    {

        Order order;

        public OrderControllerTest()
        {
            order = new Order { ID = 6, DateTime = System.DateTime.Now, Status = Status.Active, UserID = "currentUserID", Description = "Description", FoodIngredients = new List<FoodIngredient> () };
        }

        public void IDNull(string action)
        {
            var orderController = new OrderController(new Mock<UnitOfWork>().Object, new Mock<Helper>().Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Details":
                    result = orderController.Details(null) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    int? value = null;
                    result = orderController.Edit(value) as HttpStatusCodeResult;
                    break;
                case "DeactivateFalse":
                    result = orderController.Deactivate(null) as HttpStatusCodeResult;
                    break;
                case "DeactivateTrue":
                    result = orderController.Deactivate(null, true) as HttpStatusCodeResult;
                    break;
                default: //Accept
                    result = orderController.Accept(null) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        public HttpStatusCodeResult Result(OrderController orderController, int id, string action)
        {
            HttpStatusCodeResult result;

            switch (action)
            {
                case "Details":
                    result = orderController.Details(id) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    result = orderController.Edit(id) as HttpStatusCodeResult;
                    break;
                case "DeactivateFalse":
                    result = orderController.Deactivate(id) as HttpStatusCodeResult;
                    break;
                case "DeactivateTrue":
                    result = orderController.Deactivate(id, true) as HttpStatusCodeResult;
                    break;
                default: //"DeactivateConfirmed":
                    result = orderController.DeactivateConfirmed(id) as HttpStatusCodeResult;
                    break;
            }

            return (result);
        }

        public void OrderNull(int id, string action)
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();

            Order order = null;
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(id)).Returns(order);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            Assert.Equal((int)HttpStatusCode.NotFound, Result(orderController, id, action).StatusCode);
        }

        public void WrongUserID(int id, string action)
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();

            Order orderWrong = new Order { ID = 6, DateTime = System.DateTime.Now, Status = Status.Active, UserID = "wrongUserID" };
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(id)).Returns(orderWrong);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)Result(orderController, id, action).StatusCode);
        }

        public void WrongStatus(int id, string action)
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();

            Order orderWrong = new Order { ID = 6, DateTime = System.DateTime.Now, Status = Status.Accepted, UserID = "currentUserID" };
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(id)).Returns(orderWrong);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)Result(orderController, id, action).StatusCode);
        }

        public void ID(int id, string action)
        {
            IDNull(action);
            OrderNull(id, action);
        }

        [Fact]
        public void Index()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.Get(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IOrderedQueryable<Order>>>(), "")).Returns(new List<Order>());

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            var result = orderController.Index() as ViewResult;

            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public void Deactivate()
        {
            ID(6, "DeactivateFalse");
            WrongUserID(6, "DeactivateFalse");
            WrongStatus(6, "DeactivateFalse");

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            var result = orderController.Deactivate(6) as ViewResult;

            Assert.Equal("Deactivate", result.ViewName);
        }

        [Fact]
        public void Deactivate_Error()
        {
            ID(6, "DeactivateTrue");
            WrongUserID(6, "DeactivateTrue");
            WrongStatus(6, "DeactivateTrue");

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            var result = orderController.Deactivate(6, true) as ViewResult;

            Assert.Equal("Deactivate", result.ViewName);
            Assert.Equal("Unable to deactivate order. Try again, and if the problem persists contact your system administrator.", result.ViewBag.ErrorMessage);
        }

        [Fact]
        public void DeactivateConfirmed_Error()
        {
            OrderNull(6, "DeactivateConfirmed");
            WrongUserID(6, "DeactivateConfirmed");
            WrongStatus(6, "DeactivateConfirmed");

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);
            mockUnitOfWork.Setup(m => m.OrderRepository.Update(order));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.DeactivateConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Deactivate", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeactivateConfirmed()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);
            mockUnitOfWork.Setup(m => m.OrderRepository.Update(order));

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.DeactivateConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get()
        {
            ID(6, "Edit");

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            var result = orderController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Order;

            Assert.Equal("Edit", result.ViewName);
            Assert.Equal(order, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);
            mockUnitOfWork.Setup(m => m.OrderRepository.Update(order));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            var result = orderController.Edit(order) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_OrderNull()
        {
            Order order = null;

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);
            mockUnitOfWork.Setup(m => m.OrderRepository.Update(order));

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            var result = orderController.Edit(this.order) as ViewResult;

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_WrongUserID()
        {
            Order orderWrong = new Order { ID = 6, DateTime = System.DateTime.Now, Status = Status.Active, UserID = "wrongUserID" };

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(orderWrong);
            mockUnitOfWork.Setup(m => m.OrderRepository.Update(order));

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            var result = orderController.Edit(order) as ViewResult;

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var orderController = new OrderController(null, null);
            orderController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = orderController.Edit(order) as ViewResult;

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);
            mockUnitOfWork.Setup(m => m.OrderRepository.Update(order));
            mockUnitOfWork.Setup(m => m.Save());

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);

            var result = orderController.Edit(order) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Create_Get()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodIngredientRepository.Get(It.IsAny<Expression<Func<FoodIngredient, bool>>>(), null, "")).Returns(new List<FoodIngredient> { new FoodIngredient() });

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Create() as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_GetBasketEmpty()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodIngredientRepository.Get(It.IsAny<Expression<Func<FoodIngredient, bool>>>(), null, "")).Returns(new List<FoodIngredient> ());

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Create() as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Create_BasketEmpty()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodIngredientRepository.Get(It.IsAny<Expression<Func<FoodIngredient, bool>>>(), null, "")).Returns(new List<FoodIngredient>());

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Create("description") as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodIngredientRepository.Get(It.IsAny<Expression<Func<FoodIngredient, bool>>>(), null, "")).Returns(new List<FoodIngredient> { new FoodIngredient() });
            mockUnitOfWork.Setup(m => m.OrderRepository.Insert(It.IsAny<Order>()));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");
            

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Create("description") as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodIngredientRepository.Get(It.IsAny<Expression<Func<FoodIngredient, bool>>>(), null, "")).Returns(new List<FoodIngredient> { new FoodIngredient() });
            mockUnitOfWork.Setup(m => m.OrderRepository.Insert(It.IsAny<Order>()));

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");


            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Create("description") as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Details()
        {
            ID(6, "Details");
            WrongUserID(6, "Details");

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(It.IsAny<object>())).Returns(order);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Details(6) as ViewResult;
            var model = result.ViewData.Model as List<FoodIngredient>;

            Assert.Equal(order.FoodIngredients, model);
            Assert.Equal("Description", result.ViewBag.Description);
            Assert.Equal("Details", result.ViewName);
        }

        [Fact]
        public void Offers()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.Get(null, null, "")).Returns(new List<Offer>());

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Offers(6) as ViewResult;

            Assert.Equal("Offers", result.ViewName);
        }

        [Fact]
        public void Accept_OfferNull()
        {
            IDNull("Accept");

            Offer offer = null;

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offer);

            var orderController = new OrderController(mockUnitOfWork.Object, null);
            var result = orderController.Accept(6) as HttpStatusCodeResult;;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Accept_WrongUserID()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(new Offer { Order = new Order { UserID = "wrongUserID" } });

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Accept(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void Accept_WrongStatus()
        {
            string currentUserID = "currentUserID";

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(new Offer { Order = new Order { UserID = currentUserID, Status = Status.Expired } });

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Accept(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void Accept_ErrorMessage()
        {
            string currentUserID = "currentUserID";
            Offer offer = new Offer { Order = new Order { UserID = currentUserID, Status = Status.Active } };

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offer);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Accept(6, true) as ViewResult;
            var model = result.ViewData.Model as Offer;

            Assert.Equal("Accept failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
            Assert.Equal("Accept", result.ViewName);
            Assert.Equal(offer, model);
        }

        [Fact]
        public void Accept()
        {
            string currentUserID = "currentUserID";
            Offer offer = new Offer { Order = new Order { UserID = currentUserID, Status = Status.Active } };

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offer);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.Accept(6) as ViewResult;
            var model = result.ViewData.Model as Offer;

            Assert.Equal("Accept", result.ViewName);
            Assert.Equal(offer, model);
        }

        [Fact]
        public void AcceptConfirmed_OfferNull()
        {
            Offer offer = null;

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offer);

            var orderController = new OrderController(mockUnitOfWork.Object, null);
            var result = orderController.AcceptConfirmed(6) as HttpStatusCodeResult; ;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void AcceptConfirmed_WrongUserID()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(new Offer { Order = new Order { UserID = "wrongUserID" } });

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns("currentUserID");

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.AcceptConfirmed(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void AcceptConfirmed_WrongStatus()
        {
            string currentUserID = "currentUserID";

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(new Offer { Order = new Order { UserID = currentUserID, Status = Status.Expired } });

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.AcceptConfirmed(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void AcceptConfirmed_DataException()
        {
            string currentUserID = "currentUserID";

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(new Offer { Order = new Order { UserID = currentUserID, Status = Status.Active } });
            mockUnitOfWork.Setup(m => m.OfferRepository.Update(It.IsAny<Offer>()));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.AcceptConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Accept", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void AcceptConfirmed_Valid()
        {
            string currentUserID = "currentUserID";

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(new Offer { Order = new Order { UserID = currentUserID, Status = Status.Active } });
            mockUnitOfWork.Setup(m => m.OfferRepository.Update(It.IsAny<Offer>()));

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var orderController = new OrderController(mockUnitOfWork.Object, mockHelper.Object);
            var result = orderController.AcceptConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }
    }
}
