﻿using System;
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

namespace Mono.Tests.Auction.Controllers
{
    public class BasketControllerTest
    {
        private FoodIngredient foodIngredient;

        public BasketControllerTest()
        {
            foodIngredient = new FoodIngredient { ID = 2 };
        }

        private MockObject mockObject = new MockObject();

        public void IDNull(string action)
        {
            var basketController = new BasketController(new Mock<IUnitOfWork>().Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "DeleteFalse":
                    result = basketController.Delete(null) as HttpStatusCodeResult;
                    break;
                default: // "DeleteTrue":
                    result = basketController.Delete(null, true) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        public void FoodIngredientNull(int id, string action)
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();

            FoodIngredient foodIngredient = null;
            mockIUnitOfWork.Setup(m => m.FoodIngredientRepository.GetByID(id)).Returns(foodIngredient);

            var basketController = new BasketController(mockIUnitOfWork.Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "DeleteFalse":
                    result = basketController.Delete(id) as HttpStatusCodeResult;
                    break;
                default: //"DeleteTrue":
                    result = basketController.Delete(id, true) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        public void ID(int id, string action)
        {
            IDNull(action);
            FoodIngredientNull(id, action);
        }

        [Fact]
        public void Index()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodIngredientRepository.Get(It.IsAny<Expression<Func<FoodIngredient, bool>>>(), null, null)).Returns(new List<FoodIngredient>());
            
            var basketController = new BasketController(mockIUnitOfWork.Object);
            basketController.ControllerContext = new ControllerContext(mockObject.currentUser(), new RouteData(), basketController);

            var result = basketController.Index() as ViewResult;

            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public void Delete_Get()
        {
            ID(6, "DeleteFalse");

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodIngredientRepository.GetByID(6)).Returns(foodIngredient);

            var basketController = new BasketController(mockIUnitOfWork.Object);

            var result = basketController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as FoodIngredient;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(foodIngredient, model);
        }

        [Fact]
        public void Delete_GetError()
        {
            ID(6, "DeleteTrue");

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodIngredientRepository.GetByID(6)).Returns(foodIngredient);

            var basketController = new BasketController(mockIUnitOfWork.Object);

            var result = basketController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as FoodIngredient;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(foodIngredient, model);
            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodIngredientRepository.Delete(6));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var basketController = new BasketController(mockIUnitOfWork.Object);
            var result = basketController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodIngredientRepository.Delete(6));

            var basketController = new BasketController(mockIUnitOfWork.Object);

            var result = basketController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
