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
    public class OfferControllerTest
    {
        private Offer offer, offerWrongStatus, offerAccepted, offerWrongRestaurant;
        private string currentUserID;
        private List<Offer> offers;

        private Order order, orderWrongStatus;
        private List<Order> orders;

        MyUser user;

        public OfferControllerTest()
        {
            currentUserID = "currentUserID";

            offer = new Offer { OrderID = 3, RestaurantID = 7 };
            offerWrongStatus = new Offer { OrderID = 3, Order = new Order { Status = Status.Expired } };
            offerWrongRestaurant = new Offer { RestaurantID = 17 };
            offerAccepted = new Offer { AcceptedOrderID = 3, Order = new Order { UserID = currentUserID } };

            offers = new List<Offer>();

            order = new Order { Status = Status.Active };
            orderWrongStatus = new Order { Status = Status.Expired };

            orders = new List<Order>();

            user = new MyUser { RestaurantID = 7 };
        }

        [Fact]
        public void Index()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(new MyUser());
            mockUnitOfWork.Setup(m => m.OfferRepository.Get(It.IsAny<Expression<Func<Offer, bool>>>(), It.IsAny<Func<IQueryable<Offer>, IOrderedQueryable<Offer>>>(), "")).Returns(offers);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);

            var result = offerController.Index() as ViewResult;

            Assert.Equal("Index", result.ViewName);
            Assert.Equal(offers, result.ViewData.Model as List<Offer>);
        }

        [Fact]
        public void Orders()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.Get(It.IsAny<Expression<Func<Order, bool>>>(), null, "")).Returns(orders);

            var offerController = new OfferController(mockUnitOfWork.Object, null);
            var result = offerController.Orders() as ViewResult;

            Assert.Equal(orders, result.ViewData.Model as List<Order>);
            Assert.Equal("Orders", result.ViewName);
        }

        [Fact]
        public void Details_IDNull()
        {
            var offerController = new OfferController(null, null);
            HttpStatusCodeResult result = offerController.Details(null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void Details_OrderNull()
        {
            Order order = null;

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);

            var offerController = new OfferController(mockUnitOfWork.Object, null);
            HttpStatusCodeResult result = offerController.Details(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Details()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);

            var offerController = new OfferController(mockUnitOfWork.Object, null);
            var result = offerController.Details(6) as ViewResult;

            Assert.Equal(order, result.ViewData.Model as Order);
            Assert.Equal("Details", result.ViewName);
        }

        [Fact]
        public void MakeOfferGet_IDNull()
        {
            int? id = null;
            var offerController = new OfferController(null, null);
            HttpStatusCodeResult result = offerController.MakeOffer(id) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void MakeOfferGet_OrderNull()
        {
            Order order = null;

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);

            var offerController = new OfferController(mockUnitOfWork.Object, null);
            HttpStatusCodeResult result = offerController.MakeOffer(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void MakeOfferGet_WrongStatus()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(orderWrongStatus);

            var offerController = new OfferController(mockUnitOfWork.Object, null);
            var result = offerController.MakeOffer(6) as RedirectToRouteResult;

            Assert.Equal("Orders", result.RouteValues["action"]);
        }

        [Fact]
        public void MakeOfferGet()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(6)).Returns(order);

            var offerController = new OfferController(mockUnitOfWork.Object, null);
            var result = offerController.MakeOffer(6) as ViewResult;

            Assert.Equal(6, result.ViewBag.OrderID);
            Assert.Equal("MakeOffer", result.ViewName);
        }

        [Fact]
        public void MakeOffer_InvalidModel()
        {
            var offerController = new OfferController(null, null);
            offerController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = offerController.MakeOffer(offer) as ViewResult;

            Assert.Equal(offer.OrderID, result.ViewBag.OrderID);
            Assert.Equal("MakeOffer", result.ViewName);
        }

        [Fact]
        public void MakeOffer_WrongStatus()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(It.IsAny<object>())).Returns(orderWrongStatus);

            var offerController = new OfferController(mockUnitOfWork.Object, null);
            var result = offerController.MakeOffer(offerWrongStatus) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(offerWrongStatus.OrderID, result.ViewBag.OrderID);
            Assert.Equal("MakeOffer", result.ViewName);
        }

        [Fact]
        public void MakeOffer_DataException()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(user);
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(It.IsAny<object>())).Returns(order);
            mockUnitOfWork.Setup(m => m.OfferRepository.Insert(It.IsAny<Offer>()));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            var result = offerController.MakeOffer(offer) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(offer.OrderID, result.ViewBag.OrderID);
            Assert.Equal("MakeOffer", result.ViewName);
        }

        [Fact]
        public void MakeOffer()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(new MyUser { RestaurantID = 3 } );
            mockUnitOfWork.Setup(m => m.OrderRepository.GetByID(It.IsAny<object>())).Returns(order);
            mockUnitOfWork.Setup(m => m.OfferRepository.Insert(It.IsAny<Offer>()));

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            var result = offerController.MakeOffer(offer) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void DeleteGet_IDNull()
        {
            var offerController = new OfferController(null, null);
            HttpStatusCodeResult result = offerController.Delete(null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void DeleteGet_OrderNull()
        {
            Offer offerNull = null;

            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offerNull);

            var offerController = new OfferController(mockUnitOfWork.Object, null);
            HttpStatusCodeResult result = offerController.Delete(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void DeleteGet_WrongUser()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offerWrongRestaurant);
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(user);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            HttpStatusCodeResult result = offerController.Delete(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void DeleteGet_AlreadyAccepted()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offerAccepted);
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(user);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            HttpStatusCodeResult result = offerController.Delete(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

       
        [Fact]
        public void Delete_Get()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offer);
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(user);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            var result = offerController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Offer;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(offer, model);
        }

        [Fact]
        public void Delete_GetError()
        {
             var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offer);
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(user);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            var result = offerController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Offer;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(offer, model);
            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
        }

        [Fact]
        public void DeleteConfirmed_WrongRestaurant()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offerWrongRestaurant);
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(user);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            var result = offerController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_AlreadyAccepted()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offerAccepted);
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(user);

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            var result = offerController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offer);
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(user);
            mockUnitOfWork.Setup(m => m.CategoryRepository.Delete(6));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            var result = offerController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockUnitOfWork = new Mock<UnitOfWork>();
            mockUnitOfWork.Setup(m => m.OfferRepository.GetByID(6)).Returns(offer);
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(It.IsAny<object>())).Returns(user);
            mockUnitOfWork.Setup(m => m.CategoryRepository.Delete(6));

            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(m => m.getCurrentUserID()).Returns(currentUserID);

            var offerController = new OfferController(mockUnitOfWork.Object, mockHelper.Object);
            var result = offerController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
