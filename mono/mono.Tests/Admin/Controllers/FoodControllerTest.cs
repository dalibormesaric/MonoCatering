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
using Mono.Tests.Admin.Fake;

namespace Mono.Tests.Admin.Controllers
{
    public class FoodControllerTest
    {
        [Fact]
        public void Index()
        {
            //ControllerHelper.newSearchPageNumber

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(It.IsAny<Expression<Func<Food, bool>>>(), It.IsAny<Func<IQueryable<Food>, IOrderedQueryable<Food>>>(), It.IsAny<String>())).Returns(FoodFake.foods);

            var foodController = new FoodController(mockIUnitOfWork.Object);

            var result = foodController.Index("", "", "", 1, null) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Food>;

            //ViewBag.

            Assert.Equal("Index", result.ViewName);
            Assert.Equal(FoodFake.foodsPagedList, model);
        }

        [Fact]
        public void Category_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns((Category)null);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Category(5, "", "", "", (int?)null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Category()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(FoodFake.categoryWithChilds);
            mockIUnitOfWork.Setup(m => m.FoodRepository.FoodInCategory(It.IsAny<Category>())).Returns(new List<Food>());

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Category(5, "", "", "", (int?)null) as ViewResult;

            Assert.Equal("Category", result.ViewName);
        }

        [Fact]
        public void Details_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<int>())).Returns(FoodFake.foodNull);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Details(5) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Details()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(FoodFake.food);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Food;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(FoodFake.food, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(FoodFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(FoodFake.photoList);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Create() as ViewResult;

            Assert.Equal(FoodFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.Insert(FoodFake.food));
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(FoodFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(FoodFake.photoList);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Create(FoodFake.food) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(FoodFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {          
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(FoodFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(FoodFake.photoList);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            foodController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = foodController.Create(FoodFake.food) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(FoodFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {           
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.Insert(FoodFake.food));

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Create(FoodFake.food) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<int>())).Returns(FoodFake.foodNull);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Edit(5) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(FoodFake.food);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(FoodFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(FoodFake.photoList);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Food;

            Assert.Equal(FoodFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
            Assert.Equal(FoodFake.food, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.Update(FoodFake.food));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(FoodFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(FoodFake.photoList);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Edit(FoodFake.food) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(FoodFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(null, null, "")).Returns(FoodFake.foods);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(FoodFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(FoodFake.photoList);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            foodController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = foodController.Edit(FoodFake.food) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(FoodFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.Update(FoodFake.food));         

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Edit(FoodFake.food) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<int>())).Returns(FoodFake.foodNull);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Delete(5) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Delete_Get_ErrorMessage()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(FoodFake.food);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Food;

            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(FoodFake.food, model);
        }

        [Fact]
        public void Delete_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(FoodFake.food);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Food;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(FoodFake.food, model);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.Delete(6));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.Delete(6));

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Ingredients_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(It.IsAny<int>())).Returns(FoodFake.foodNull);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Ingredients(5) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Ingredients()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(FoodFake.food);
            mockIUnitOfWork.Setup(m => m.IngredientRepository.IngredientsForFood(FoodFake.food)).Returns(FoodFake.allIngreients);

            var foodController = new FoodController(mockIUnitOfWork.Object);
            var result = foodController.Ingredients(6) as ViewResult;
            var model = result.ViewData.Model as List<Ingredient>;

            Assert.Equal(FoodFake.food.Name, result.ViewBag.Food);
            Assert.Equal("Ingredients", result.ViewName);
            Assert.Equal(FoodFake.allIngreients, model);
        }
    }
}
