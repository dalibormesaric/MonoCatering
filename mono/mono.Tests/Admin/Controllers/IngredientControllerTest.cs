using System;
using Xunit;
using Moq;
using System.Web.Mvc;
using Mono.Areas.Admin.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data;
using System.Linq.Expressions;
using Mono.Data;
using Mono.Model;
using Mono.Tests.Admin.Fake;

namespace Mono.Tests.Admin.Controllers
{

    public class IngredientControllerTest
    {
        [Fact]
        public void Index()
        {
            //ControllerHelper.newSearchPageNumber

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.Get(It.IsAny<Expression<Func<Ingredient, bool>>>(), It.IsAny<Func<IQueryable<Ingredient>, IOrderedQueryable<Ingredient>>>(), It.IsAny<String>())).Returns(IngredientFake.ingredients);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Index("", "", "", 1) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Ingredient>;

            //ViewBag.

            Assert.Equal("Index", result.ViewName);
            Assert.Equal(IngredientFake.ingredientsPagedList, model);
        }

        [Fact]
        public void Details_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.GetByID(It.IsAny<int>())).Returns(IngredientFake.ingredientNull);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Details(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Details()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.GetByID(6)).Returns(IngredientFake.ingredient);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Ingredient;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(IngredientFake.ingredient, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(IngredientFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Food>, IOrderedQueryable<Food>>>(), "")).Returns(IngredientFake.foodsOrdered);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Create() as ViewResult;

            Assert.Equal(IngredientFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(IngredientFake.foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.Insert(IngredientFake.ingredient));
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(IngredientFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Food>, IOrderedQueryable<Food>>>(), "")).Returns(IngredientFake.foodsOrdered);
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Create(IngredientFake.ingredient) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(IngredientFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(IngredientFake.foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {          
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(IngredientFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Food>, IOrderedQueryable<Food>>>(), "")).Returns(IngredientFake.foodsOrdered);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            ingredientController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = ingredientController.Create(IngredientFake.ingredient) as ViewResult;

            Assert.Equal(IngredientFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(IngredientFake.foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {           
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.Insert(IngredientFake.ingredient));

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Create(IngredientFake.ingredient) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.GetByID(It.IsAny<int>())).Returns(IngredientFake.ingredientNull);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Edit(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.GetByID(6)).Returns(IngredientFake.ingredient);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(IngredientFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Food>, IOrderedQueryable<Food>>>(), "")).Returns(IngredientFake.foodsOrdered);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Ingredient;

            Assert.Equal(IngredientFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(IngredientFake.foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
            Assert.Equal(IngredientFake.ingredient, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.Update(IngredientFake.ingredient));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(IngredientFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Food>, IOrderedQueryable<Food>>>(), "")).Returns(IngredientFake.foodsOrdered);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Edit(IngredientFake.ingredient) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(IngredientFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(IngredientFake.foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.Get(null, null, "")).Returns(IngredientFake.ingredients);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(IngredientFake.categoriesOrdered);
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Food>, IOrderedQueryable<Food>>>(), "")).Returns(IngredientFake.foodsOrdered);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            ingredientController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = ingredientController.Edit(IngredientFake.ingredient) as ViewResult;

            Assert.Equal(IngredientFake.categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(IngredientFake.foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.Update(IngredientFake.ingredient));         

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Edit(IngredientFake.ingredient) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.GetByID(It.IsAny<int>())).Returns(IngredientFake.ingredientNull);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Delete(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Delete_Get_ErrorMessage()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.GetByID(6)).Returns(IngredientFake.ingredient);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Ingredient;

            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(IngredientFake.ingredient, model);
        }

        [Fact]
        public void Delete_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.GetByID(6)).Returns(IngredientFake.ingredient);

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Ingredient;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(IngredientFake.ingredient, model);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.Delete(6));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.IngredientRepository.Delete(6));

            var ingredientController = new IngredientController(mockIUnitOfWork.Object);
            var result = ingredientController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
