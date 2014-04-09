using System;
using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;
using Mono.Areas.Admin.Controllers;
using System.Web.Mvc;
using System.Net;
using System.Data;
using System.Linq.Expressions;
using Mono.Data;
using Mono.Model;
using Mono.Models;
using Mono.Tests.Admin.Fake;

namespace Mono.Tests.Admin.Controllers
{
    public class UserControllerTest
    {
        [Fact]
        public void Index()
        {
            //ControllerHelper.newSearchPageNumber

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.UserRepository.Get(It.IsAny<Expression<Func<MyUser, bool>>>(), It.IsAny<Func<IQueryable<MyUser>, IOrderedQueryable<MyUser>>>(), It.IsAny<String>())).Returns(UserFake.users);

            var UserController = new UserController(mockIUnitOfWork.Object);

            var result = UserController.Index("", "", "", 1) as ViewResult;
           
            //ControllerHelper.newSearchPageNumber
            
            //var model = result.ViewData.Model as IEnumerable<AdminUserViewModel>;
            //Mapper.CreateMap<MyUser, AdminUserViewModel>().ForMember(dest => dest.Restaurant, conf => conf.MapFrom(ol => ol.Restaurant.Name));
            //IEnumerable<AdminUserViewModel> usersAUVM = Mapper.Map<IEnumerable<MyUser>, IEnumerable<AdminUserViewModel>>(UserFake.users.ToList());
            //IEnumerable<AdminUserViewModel> usersPagedList = usersAUVM.ToPagedList(1, Global.PageSize);
            //Assert.Equal(usersPagedList, model);

            Assert.Equal("Index", result.ViewName);
        }

        public void Edit_Get_IDNull()
        {
            var userController = new UserController(null);
            HttpStatusCodeResult result = userController.Edit((string)null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        public void Edit_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.UserRepository.GetByID("5")).Returns(UserFake.userNull);

            var userController = new UserController(mockIUnitOfWork.Object);
            HttpStatusCodeResult result = userController.Edit("5") as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.UserRepository.GetByID("6")).Returns(UserFake.user);
            mockIUnitOfWork.Setup(m => m.RestaurantRepository.Get(null, It.IsAny<Func<IQueryable<Restaurant>, IOrderedQueryable<Restaurant>>>(), "")).Returns(UserFake.restaurantsOrdered);

            var UserController = new UserController(mockIUnitOfWork.Object);

            var result = UserController.Edit("6") as ViewResult;
            var model = result.ViewData.Model as MyUser;

            Assert.Equal(UserFake.restaurantsOrdered, (result.ViewBag.RestaurantID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
            Assert.Equal(UserFake.user, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.UserRepository.GetByID("6")).Returns(UserFake.user);
            mockIUnitOfWork.Setup(m => m.UserRepository.Update(UserFake.user));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var UserController = new UserController(mockIUnitOfWork.Object);
            var result = UserController.Edit("6", 6) as RedirectToRouteResult;

            Assert.Equal("Edit", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.UserRepository.Update(UserFake.user));         

            var UserController = new UserController(mockIUnitOfWork.Object);

            var result = UserController.Edit("invalidID", 6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
