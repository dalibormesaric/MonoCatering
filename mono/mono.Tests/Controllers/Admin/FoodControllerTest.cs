using System;
using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;
using mono.Areas.Admin.Controllers;
using System.Web.Mvc;
using System.Net;
using System.Data;

namespace mono.Tests.Controllers.Admin
{
    public class FoodControllerTest
    {
        private Models.Food food;
        private Models.Food food1, food2, food3, food4, food5, food6;
        private IQueryable<Models.Food> foods;

        Models.Category category1, category2;
        IQueryable<Models.Category> categoriesOrdered;

        Models.Ingredient foodIngredient1;

        public FoodControllerTest()
        {
            food = new Models.Food { CategoryID = 6 };
            foodIngredient1 = new Models.Ingredient { Name = "foodIngredient1" };
            food.Ingredients = new List<Models.Ingredient> { foodIngredient1, };

            Models.Category category = new Models.Category { Name = "category" };

            food1 = new Models.Food { Name = "food1", Category = category };
            food2 = new Models.Food { Name = "cdfgegry2", Category = category };
            food3 = new Models.Food { Name = "casdfgry3", Category = category };
            food4 = new Models.Food { Name = "food4", Category = category };
            food5 = new Models.Food { Name = "food5", Category = category };
            food6 = new Models.Food { Name = "food6", Category = category };

            foods = new List<Models.Food> { food1, food2, food3, food4, food5, food6 }.AsQueryable();

            category1 = new Models.Category { ID = 6, Name = "category1" };
            category2 = new Models.Category { ID = 3, Name = "category2" };

            categoriesOrdered = new List<Models.Category> { category1, category2 }.AsQueryable();
        }

        public void IDNull(string action)
        {
            var foodController = new FoodController(new Mock<mono.DAL.UnitOfWork>().Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Category":
                    result = foodController.Category(null) as HttpStatusCodeResult;
                    break;
                case "Details":
                    result = foodController.Details(null) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    int? value = null;
                    result = foodController.Edit(value) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = foodController.Delete(null) as HttpStatusCodeResult;
                    break;
                case "DeleteTrue":
                    result = foodController.Delete(null, true) as HttpStatusCodeResult;
                    break;
                default: //Ingredient
                    result = foodController.Ingredients(null) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        public void FoodNull(int id, string action)
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            Models.Food food = null;
            Models.Category category = null;
            mockUnitOfWork.Setup(m => m.FoodRepository.GetByID(id)).Returns(food);
            mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(id)).Returns(category);

            var foodController = new FoodController(mockUnitOfWork.Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Category":
                    result = foodController.Category(id) as HttpStatusCodeResult;
                    break;
                case "Details":
                    result = foodController.Details(id) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    result = foodController.Edit(id) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = foodController.Delete(id) as HttpStatusCodeResult;
                    break;
                case "DeleteTrue":
                    result = foodController.Delete(id, true) as HttpStatusCodeResult;
                    break;
                default: //Ingredient
                    result = foodController.Ingredients(id) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        public void ID(int id, string action)
        {
            IDNull(action);
            FoodNull(id, action);
        }

        [Fact]
        public void Index_SortingAsc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.Get(null, null, "")).Returns(foods);

            var FoodController = new FoodController(mockUnitOfWork.Object);

            var result = FoodController.Index(null, "food", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Food>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(food6.Name, model.ElementAt(0).Name);
        }

        [Fact]
        public void Index_SortingDesc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.Get(null, null, "")).Returns(foods);

            var FoodController = new FoodController(mockUnitOfWork.Object);

            var result = FoodController.Index("Name_desc", "food", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Food>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(food1.Name, model.ElementAt(0).Name);
        }

        [Fact]
        public void Category()
        {
            ID(6, "Category");

            var parentFoods = new List<Models.Food> { food1, food2 };
            var child1Foods = new List<Models.Food> { food3, food4, food5 };
            var child2Foods = new List<Models.Food> { food6 };

            Models.Category child1Category = new Models.Category { ID = 3, Name = "child1Category", Food = child1Foods, ChildCategory = new List<Models.Category> () };
            Models.Category child2Category = new Models.Category { ID = 2, Name = "child2Category", Food = child2Foods, ChildCategory = new List<Models.Category> () };
            Models.Category parentCategory = new Models.Category { ID = 6, Name = "parentCategory", Food = parentFoods, ChildCategory = new List<Models.Category> { child1Category, child2Category } };
           
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(6)).Returns(parentCategory);

            var FoodController = new FoodController(mockUnitOfWork.Object);
            var result = FoodController.Category(6) as ViewResult;
            var model = result.ViewData.Model as List<Models.Food>;

            Assert.Equal(parentCategory.Name, result.ViewBag.Category);
            Assert.Equal("Category", result.ViewName);
            Assert.Equal(foods.OrderBy(f => f.Name).ToList(), model);
        }

        [Fact]
        public void Details()
        {
            ID(6, "Details");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(food);

            var FoodController = new FoodController(mockUnitOfWork.Object);
            var result = FoodController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Models.Food;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(food, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);

            var FoodController = new FoodController(mockUnitOfWork.Object);
            var result = FoodController.Create() as ViewResult;

            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();    
            mockUnitOfWork.Setup(m => m.FoodRepository.Insert(food));
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var FoodController = new FoodController(mockUnitOfWork.Object);
            var result = FoodController.Create(food) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {          
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);

            var FoodController = new FoodController(mockUnitOfWork.Object);
            FoodController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = FoodController.Create(food) as ViewResult;

            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {           
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.Insert(food));

            var FoodController = new FoodController(mockUnitOfWork.Object);

            var result = FoodController.Create(food) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get()
        {
            ID(6, "Edit");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(food);
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);

            var FoodController = new FoodController(mockUnitOfWork.Object);

            var result = FoodController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Models.Food;


            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);           
            Assert.Equal(food, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.Update(food));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);

            var FoodController = new FoodController(mockUnitOfWork.Object);
            var result = FoodController.Edit(food) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.Get(null, null, "")).Returns(foods);
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);

            var FoodController = new FoodController(mockUnitOfWork.Object);
            FoodController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = FoodController.Edit(food) as ViewResult;

            Assert.Equal(categoriesOrdered, (result.ViewBag.CategoryID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.Update(food));         

            var FoodController = new FoodController(mockUnitOfWork.Object);

            var result = FoodController.Edit(food) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get()
        {
            ID(6, "DeleteFalse");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(food);

            var FoodController = new FoodController(mockUnitOfWork.Object);

            var result = FoodController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Models.Food;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(food, model);
        }

        [Fact]
        public void Delete_GetError()
        {
            ID(6, "DeleteTrue");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(food);

            var FoodController = new FoodController(mockUnitOfWork.Object);

            var result = FoodController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Models.Food;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(food, model);
            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.Delete(6));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var FoodController = new FoodController(mockUnitOfWork.Object);
            var result = FoodController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.Delete(6));

            var FoodController = new FoodController(mockUnitOfWork.Object);

            var result = FoodController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Ingredients()
        {
            ID(6, "Ingredients");

            Models.Category parentCategory = new Models.Category { ID = 3, Name = "parentCategory" };
            Models.Ingredient parentIngredient1 = new Models.Ingredient { Name = "parentIngredient1", Category = parentCategory };
            Models.Ingredient parentIngredient2 = new Models.Ingredient { Name = "parentIngredient2", Category = parentCategory };
            parentCategory.Ingredients = new List<Models.Ingredient> { parentIngredient1, parentIngredient2 };
            Models.Category childCategory = new Models.Category { ID = 6, Name = "childCategory", ParentCategoryID = 3 };
            Models.Ingredient childIngredient1 = new Models.Ingredient { Name = "childIngredient1", Category = childCategory };
            Models.Ingredient childIngredient2 = new Models.Ingredient { Name = "achildIngredient2", Category = childCategory };
            childCategory.Ingredients = new List<Models.Ingredient> { childIngredient1, childIngredient2 };

            var allIngreients = new List<Models.Ingredient> { childIngredient2, childIngredient1, food.Ingredients.ElementAt(0), parentIngredient1, parentIngredient2 };

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.FoodRepository.GetByID(6)).Returns(food);
            //mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(6)).Returns(childCategory);
            //mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(3)).Returns(parentCategory);
            mockUnitOfWork.Setup(m => m.IngredientsForFood(food)).Returns(allIngreients);

            var FoodController = new FoodController(mockUnitOfWork.Object);

            var result = FoodController.Ingredients(6) as ViewResult;
            var model = result.ViewData.Model as List<Models.Ingredient>;

            Assert.Equal(food.Name, result.ViewBag.Food);
            Assert.Equal("Ingredients", result.ViewName);
            Assert.Equal(allIngreients, model);
        }
    }
}
