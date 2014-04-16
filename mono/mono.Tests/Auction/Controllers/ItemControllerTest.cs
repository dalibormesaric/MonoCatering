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

namespace Mono.Tests.Auction.Controllers
{
    public class ItemControllerTest
    {
        private Category category1, category2, category3, category4;
        private Food food11, food12;
        private IQueryable<Category> categories;

        public ItemControllerTest()
        {
            food11 = new Food { Name = "food11" };
            food12 = new Food { Name = "food12" };

            category1 = new Category { ID = 1, Name = "category1", SizeType = 0, ParentCategoryID = null, Food = new List<Food> { food11, food12 }, ChildCategory = new List<Category>() };
            category2 = new Category { ID = 2, Name = "category2", SizeType = 0, ParentCategoryID = null };
            category3 = new Category { ID = 3, Name = "category3", SizeType = 0, ParentCategoryID = category1.ID };
            category4 = new Category { ID = 4, Name = "category4", SizeType = 0, ParentCategoryID = category1.ID };

            category1.ChildCategory.Add(category3);
            category1.ChildCategory.Add(category4);

            categories = new List<Category> { category1, category2, category3, category4 }.AsQueryable();
        }

        private MockObject mockObject = new MockObject();

        [Fact]
        public void Index_IDNull()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), It.IsAny<String>())).Returns(categories.Where(c => c.ParentCategoryID == null));

            var itemController = new ItemController(mockIUnitOfWork.Object);
            itemController.ControllerContext = new ControllerContext(mockObject.isAjaxFalse(), new RouteData(), itemController);
            var result = itemController.Index(null) as ViewResult;
            var model = result.ViewData.Model as ItemViewModel;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(category1.Name, model.ListCategoryFood.ElementAt(0).Name);
            Assert.Equal(category2.Name, model.ListCategoryFood.ElementAt(1).Name);
        }

        [Fact]
        public void Index_IDNullAjax()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), It.IsAny<String>())).Returns(categories.Where(c => c.ParentCategoryID == null));

            var itemController = new ItemController(mockIUnitOfWork.Object);
            itemController.ControllerContext = new ControllerContext(mockObject.isAjaxTrue(), new RouteData(), itemController);

            var result = itemController.Index(null) as PartialViewResult;
            var model = result.ViewData.Model as ItemViewModel;

            Assert.Equal("_Category", result.ViewName);

            Assert.Equal(category1.Name, model.ListCategoryFood.ElementAt(0).Name);
            Assert.Equal(category2.Name, model.ListCategoryFood.ElementAt(1).Name);
        }

        [Fact]
        public void Index_Ajax()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<object>())).Returns(category1);

            var itemController = new ItemController(mockIUnitOfWork.Object);
            itemController.ControllerContext = new ControllerContext(mockObject.isAjaxTrue(), new RouteData(), itemController);

            var result = itemController.Index(1) as PartialViewResult;
            var model = result.ViewData.Model as ItemViewModel;

            Assert.Equal("_Category", result.ViewName);

            Assert.Equal(category3.Name, model.ListCategoryFood.ElementAt(0).Name);
            Assert.Equal(category4.Name, model.ListCategoryFood.ElementAt(1).Name);
            Assert.Equal(food11.Name, model.ListCategoryFood.ElementAt(2).Name);
            Assert.Equal(food12.Name, model.ListCategoryFood.ElementAt(3).Name);
        }

        [Fact]
        public void Index_CategoryNotFound()
        {
            Category category = null;

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<object>())).Returns(category);

            var itemController = new ItemController(mockIUnitOfWork.Object);

            var result = itemController.Index(1) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Index()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<object>())).Returns(category1);

            var itemController = new ItemController(mockIUnitOfWork.Object);
            itemController.ControllerContext = new ControllerContext(mockObject.isAjaxFalse(), new RouteData(), itemController);

            var result = itemController.Index(1) as ViewResult;
            var model = result.ViewData.Model as ItemViewModel;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(category3.Name, model.ListCategoryFood.ElementAt(0).Name);
            Assert.Equal(category4.Name, model.ListCategoryFood.ElementAt(1).Name);
            Assert.Equal(   food11.Name, model.ListCategoryFood.ElementAt(2).Name);
            Assert.Equal(   food12.Name, model.ListCategoryFood.ElementAt(3).Name);
        }

        [Fact]
        public void AddGet_NullID()
        {
            var itemController = new ItemController(null);
            int? ID = null;
            var result = itemController.Add(ID) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void AddGet_FoodNotFound()
        {
            Food food = null;

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<object>())).Returns(food);

            var itemController = new ItemController(mockIUnitOfWork.Object);
            var result = itemController.Add(3) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void AddGet()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<object>())).Returns(food11);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<object>())).Returns(category1);
            mockIUnitOfWork.Setup(m => m.SizeValues(It.IsAny<int>())).Returns(new List<CategorySize>());
            mockIUnitOfWork.Setup(m => m.IngredientsForFood(It.IsAny<Food>())).Returns(new List<Ingredient>());

            var itemController = new ItemController(mockIUnitOfWork.Object);
            var result = itemController.Add(1) as ViewResult;

            Assert.Equal("Add", result.ViewName);
        }

        [Fact]
        public void Add_NullID()
        {
            FoodIngredientViewModel foodIngredientViewModel = new FoodIngredientViewModel();

            var itemController = new ItemController(null);
            var result = itemController.Add(foodIngredientViewModel) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public void Add_FoodNotFound()
        {
            FoodIngredientViewModel foodIngredientViewModel = new FoodIngredientViewModel
            {
                FoodID = 1
            };

            Food food = null;

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<object>())).Returns(food);

            var itemController = new ItemController(mockIUnitOfWork.Object);

            var result = itemController.Add(foodIngredientViewModel) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Add_InvalidModel()
        {
            FoodIngredientViewModel foodIngredientViewModel = new FoodIngredientViewModel
            {
                FoodID = 1
            };

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<object>())).Returns(food11);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<object>())).Returns(category1);
            mockIUnitOfWork.Setup(m => m.SizeValues(It.IsAny<int>())).Returns(new List<CategorySize>());
            mockIUnitOfWork.Setup(m => m.IngredientsForFood(It.IsAny<Food>())).Returns(new List<Ingredient>());

            var itemController = new ItemController(mockIUnitOfWork.Object);
            itemController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = itemController.Add(foodIngredientViewModel) as ViewResult;

            Assert.Equal("Add", result.ViewName);
        }

        [Fact]
        public void Add_DataException()
        {
            FoodIngredientViewModel foodIngredientViewModel = new FoodIngredientViewModel
            {
                FoodID = 1
            };

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<object>())).Returns(food11);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<object>())).Returns(category1);
            mockIUnitOfWork.Setup(m => m.SizeValues(It.IsAny<int>())).Returns(new List<CategorySize>());
            mockIUnitOfWork.Setup(m => m.IngredientsForFood(It.IsAny<Food>())).Returns(new List<Ingredient>());
            mockIUnitOfWork.Setup(m => m.FoodIngredientRepository.Insert(It.IsAny<FoodIngredient>()));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var itemController = new ItemController(mockIUnitOfWork.Object);
            itemController.ControllerContext = new ControllerContext(mockObject.currentUser(), new RouteData(), itemController);
 
            var result = itemController.Add(foodIngredientViewModel) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Add", result.ViewName);
        }

        [Fact]
        public void Add()
        {
            FoodIngredientViewModel foodIngredientViewModel = new FoodIngredientViewModel
            {
                FoodID = 1
            };

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<object>())).Returns(food11);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<object>())).Returns(category1);
            mockIUnitOfWork.Setup(m => m.SizeValues(It.IsAny<int>())).Returns(new List<CategorySize>());
            mockIUnitOfWork.Setup(m => m.IngredientsForFood(It.IsAny<Food>())).Returns(new List<Ingredient>());
            mockIUnitOfWork.Setup(m => m.FoodIngredientRepository.Insert(It.IsAny<FoodIngredient>()));
            mockIUnitOfWork.Setup(m => m.Save());

            var itemController = new ItemController(mockIUnitOfWork.Object);
            itemController.ControllerContext = new ControllerContext(mockObject.currentUser(), new RouteData(), itemController);

            var result = itemController.Add(foodIngredientViewModel) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
