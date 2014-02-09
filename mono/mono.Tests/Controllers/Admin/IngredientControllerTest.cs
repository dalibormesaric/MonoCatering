using System;
using Xunit;
using Moq;
using System.Web.Mvc;
using mono.Areas.Admin.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data;

namespace mono.Tests.Controllers.Admin
{

    public class IngredientControllerTest
    {
        private Models.Ingredient ingredient;
        private Models.Ingredient ingredient1, ingredient2, ingredient3, ingredient4, ingredient5, ingredient6;
        private IQueryable<Models.Ingredient> ingredients;

        Models.Category category1, category2;
        IQueryable<Models.Category> categoriesOrdered;

        Models.Food food1, food2;
        IQueryable<Models.Food> foodsOrdered;

        public IngredientControllerTest()
        {
            ingredient = new Models.Ingredient { CategoryID = 6 };

            Models.Category category = new Models.Category { Name = "category" };
            Models.Food food = new Models.Food { Name = "food" };

            ingredient1 = new Models.Ingredient { Name = "ingredient1", Category = category, Food = food };
            ingredient2 = new Models.Ingredient { Name = "ingrsfgent2", Category = category, Food = food };
            ingredient3 = new Models.Ingredient { Name = "afasgdient3", Category = category, Food = food };
            ingredient4 = new Models.Ingredient { Name = "ingredient4", Category = category, Food = food };
            ingredient5 = new Models.Ingredient { Name = "ingredient5", Category = category, Food = food };
            ingredient6 = new Models.Ingredient { Name = "ingredient6", Category = category, Food = food };

            ingredients = new List<Models.Ingredient> { ingredient1, ingredient2, ingredient3, ingredient4, ingredient5, ingredient6 }.AsQueryable();

            category1 = new Models.Category { ID = 6, Name = "category1" };
            category2 = new Models.Category { ID = 3, Name = "category2" };

            categoriesOrdered = new List<Models.Category> { category1, category2 }.AsQueryable();

            food1 = new Models.Food { ID = 6, Name = "food1" };
            food2 = new Models.Food { ID = 3, Name = "food2" };

            foodsOrdered = new List<Models.Food> { food1, food2 }.AsQueryable();
        }

        public void IDNull(string action)
        {
            var ingredientController = new IngredientController(new Mock<mono.DAL.UnitOfWork>().Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Details":
                    result = ingredientController.Details(null) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    int? value = null;
                    result = ingredientController.Edit(value) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = ingredientController.Delete(null) as HttpStatusCodeResult;
                    break;
                default: // "DeleteTrue":
                    result = ingredientController.Delete(null, true) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        public void Ingredient1Null(int id, string action)
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            Models.Ingredient ingredient = null;
            mockUnitOfWork.Setup(m => m.IngredientRepository.GetByID(id)).Returns(ingredient);

            var ingredientController = new IngredientController(mockUnitOfWork.Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Details":
                    result = ingredientController.Details(id) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    result = ingredientController.Edit(id) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = ingredientController.Delete(id) as HttpStatusCodeResult;
                    break;
                default: // "DeleteTrue":
                    result = ingredientController.Delete(id, true) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        public void ID(int id, string action)
        {
            IDNull(action);
            Ingredient1Null(id, action);
        }

        [Fact]
        public void Index_SortingAsc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.Get(null, null, "")).Returns(ingredients);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);

            var result = IngredientController.Index(null, "ingredient", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Ingredient>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(ingredient6.Name, model.ElementAt(0).Name);
        }

        [Fact]
        public void Index_SortingDesc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.Get(null, null, "")).Returns(ingredients);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);

            var result = IngredientController.Index("Name_desc", "ingredient", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Ingredient>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(ingredient1.Name, model.ElementAt(0).Name);
        }

        [Fact]
        public void Details()
        {
            ID(6, "Details");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.GetByID(6)).Returns(ingredient);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);
            var result = IngredientController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Models.Ingredient;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(ingredient, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Models.Food>, IOrderedQueryable<Models.Food>>>(), "")).Returns(foodsOrdered);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);
            var result = IngredientController.Create() as ViewResult;

            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();    
            mockUnitOfWork.Setup(m => m.IngredientRepository.Insert(ingredient));
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Models.Food>, IOrderedQueryable<Models.Food>>>(), "")).Returns(foodsOrdered);
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var IngredientController = new IngredientController(mockUnitOfWork.Object);
            var result = IngredientController.Create(ingredient) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {          
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Models.Food>, IOrderedQueryable<Models.Food>>>(), "")).Returns(foodsOrdered);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);
            IngredientController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = IngredientController.Create(ingredient) as ViewResult;

            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {           
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.Insert(ingredient));

            var IngredientController = new IngredientController(mockUnitOfWork.Object);

            var result = IngredientController.Create(ingredient) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get()
        {
            ID(6, "Edit");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.GetByID(6)).Returns(ingredient);
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Models.Food>, IOrderedQueryable<Models.Food>>>(), "")).Returns(foodsOrdered);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);

            var result = IngredientController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Models.Ingredient;


            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);           
            Assert.Equal(ingredient, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.Update(ingredient));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Models.Food>, IOrderedQueryable<Models.Food>>>(), "")).Returns(foodsOrdered);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);
            var result = IngredientController.Edit(ingredient) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.Get(null, null, "")).Returns(ingredients);
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.FoodRepository.Get(null, It.IsAny<Func<IQueryable<Models.Food>, IOrderedQueryable<Models.Food>>>(), "")).Returns(foodsOrdered);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);
            IngredientController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = IngredientController.Edit(ingredient) as ViewResult;

            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal(foodsOrdered, (result.ViewBag.FoodID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.Update(ingredient));         

            var IngredientController = new IngredientController(mockUnitOfWork.Object);

            var result = IngredientController.Edit(ingredient) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get()
        {
            ID(6, "DeleteFalse");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.GetByID(6)).Returns(ingredient);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);

            var result = IngredientController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Models.Ingredient;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(ingredient, model);
        }

        [Fact]
        public void Delete_GetError()
        {
            ID(6, "DeleteTrue");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.GetByID(6)).Returns(ingredient);

            var IngredientController = new IngredientController(mockUnitOfWork.Object);

            var result = IngredientController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Models.Ingredient;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(ingredient, model);
            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.Delete(6));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var IngredientController = new IngredientController(mockUnitOfWork.Object);
            var result = IngredientController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.IngredientRepository.Delete(6));

            var IngredientController = new IngredientController(mockUnitOfWork.Object);

            var result = IngredientController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
