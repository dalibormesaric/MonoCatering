using System;
using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;
using mono.Areas.Admin.Controllers;
using System.Web.Mvc;
using System.Net;
using System.Data;
using System.Linq.Expressions;

namespace mono.Tests.Controllers.Admin
{
    public class UserControllerTest
    {
        private Models.MyUser user;
        private Models.MyUser user1, user2, user3, user4, user5, user6;
        private IQueryable<Models.MyUser> users;

        Models.Restaurant restaurant1, restaurant2;
        IQueryable<Models.Restaurant> restaurantsOrdered;

        public UserControllerTest()
        {
            user = new Models.MyUser { Id = "6" };

            user1 = new Models.MyUser { UserName = "user1", FirstName = "first1", LastName = "last1" };
            user2 = new Models.MyUser { UserName = "aser2", FirstName = "first2", LastName = "last2" };
            user3 = new Models.MyUser { UserName = "udfg3", FirstName = "first3", LastName = "last3" };
            user4 = new Models.MyUser { UserName = "user4", FirstName = "first4", LastName = "last4" };
            user5 = new Models.MyUser { UserName = "user5", FirstName = "first5", LastName = "last5" };
            user6 = new Models.MyUser { UserName = "user6", FirstName = "first6", LastName = "last6" };

            users = new List<Models.MyUser> { user1, user2, user3, user4, user5, user6 }.AsQueryable();

            restaurant1 = new Models.Restaurant { ID = 6, Name = "restaurant1" };
            restaurant2 = new Models.Restaurant { ID = 3, Name = "restaurant2" };

            restaurantsOrdered = new List<Models.Restaurant> { restaurant1, restaurant2 }.AsQueryable();
        }

        public void IDNull()
        {
            var userController = new UserController(new Mock<mono.DAL.UnitOfWork>().Object);

            HttpStatusCodeResult result = userController.Edit((string)null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        public void UserNull(string id)
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            Models.MyUser user = null;
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID(id)).Returns(user);

            var userController = new UserController(mockUnitOfWork.Object);

            HttpStatusCodeResult result = userController.Edit(id) as HttpStatusCodeResult;
                    
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        public void ID(string id)
        {
            IDNull();
            UserNull(id);
        }

        private static string searchString = "user";
        private Expression<Func<Models.MyUser, bool>> filter = (u =>
            u.UserName.ToUpper().Contains(searchString.ToUpper()) ||
            u.FirstName.ToUpper().Contains(searchString.ToUpper()) ||
            u.LastName.ToUpper().Contains(searchString.ToUpper())
        );
        private Func<IQueryable<Models.MyUser>, IOrderedQueryable<Models.MyUser>> orderBy = (q => q.OrderBy(r => r.UserName));
        private Func<IQueryable<Models.MyUser>, IOrderedQueryable<Models.MyUser>> orderByDescending = (q => q.OrderByDescending(r => r.UserName));

        [Fact]
        public void Index_SortingAsc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.UserRepository.Get(It.IsAny<Expression<Func<Models.MyUser, bool>>>(), It.IsAny<Func<IQueryable<Models.MyUser>, IOrderedQueryable<Models.MyUser>>>(), It.IsAny<String>())).Returns(orderBy(users.Where(filter)));

            var UserController = new UserController(mockUnitOfWork.Object);

            var result = UserController.Index(null, searchString, null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.AdminUserViewModel>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(user6.UserName, model.ElementAt(0).UserName);
        }

        [Fact]
        public void Index_SortingDesc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.UserRepository.Get(It.IsAny<Expression<Func<Models.MyUser, bool>>>(), It.IsAny<Func<IQueryable<Models.MyUser>, IOrderedQueryable<Models.MyUser>>>(), It.IsAny<String>())).Returns(orderByDescending(users.Where(filter)));

            var UserController = new UserController(mockUnitOfWork.Object);

            var result = UserController.Index("Name_desc", searchString, null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.AdminUserViewModel>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(user1.UserName, model.ElementAt(0).UserName);
        }

        [Fact]
        public void Edit_Get()
        {
            ID("6");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID("6")).Returns(user);
            mockUnitOfWork.Setup(m => m.RestaurantRepository.Get(null, It.IsAny<Func<IQueryable<Models.Restaurant>, IOrderedQueryable<Models.Restaurant>>>(), "")).Returns(restaurantsOrdered);

            var UserController = new UserController(mockUnitOfWork.Object);

            var result = UserController.Edit("6") as ViewResult;
            var model = result.ViewData.Model as Models.MyUser;

            Assert.Equal(restaurantsOrdered, (result.ViewBag.RestaurantID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
            Assert.Equal(user, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.UserRepository.GetByID("6")).Returns(user);
            mockUnitOfWork.Setup(m => m.UserRepository.Update(user));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var UserController = new UserController(mockUnitOfWork.Object);
            var result = UserController.Edit("6", 6) as RedirectToRouteResult;

            Assert.Equal("Edit", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.UserRepository.Update(user));         

            var UserController = new UserController(mockUnitOfWork.Object);

            var result = UserController.Edit("6", 6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
