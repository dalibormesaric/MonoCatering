using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using mono.Areas.Admin.Controllers;
using System.Web.Mvc;
using System.Net;
using System.Data;

namespace mono.Tests.Controllers.Admin
{
    public class CategoryControllerTest
    {
        private Models.Category category;
        private Models.Category category1, category2, category3, category4, category5, category6;
        private IQueryable<Models.Category> categories, categoriesOrdered;

        public CategoryControllerTest()
        {
            category = new Models.Category { Name = "category", SizeType = 0 };

            category1 = new Models.Category { Name = "category1", SizeType = 0  };
            category2 = new Models.Category { Name = "cdfgegry2", SizeType = 0, ParentCategory = category1 };
            category3 = new Models.Category { Name = "casdfgry3", SizeType = 0 };
            category4 = new Models.Category { Name = "category4", SizeType = 0, ParentCategory = category1 };
            category5 = new Models.Category { Name = "category5", SizeType = 0 };
            category6 = new Models.Category { Name = "category6", SizeType = 0 };

            categories = new List<Models.Category> { category1, category2, category3, category4, category5, category6 }.AsQueryable();

            categoriesOrdered = categories.OrderBy(c => c.Name);
        }

        public void IDNull(string action)
        {
            var categoryController = new CategoryController(new Mock<mono.DAL.UnitOfWork>().Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Details":
                    result = categoryController.Details(null) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    int? value = null;
                    result = categoryController.Edit(value) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = categoryController.Delete(null) as HttpStatusCodeResult;
                    break;
                case "DeleteTrue":
                    result = categoryController.Delete(null, true) as HttpStatusCodeResult;
                    break;
                default: //Ingredient
                    result = categoryController.Ingredients(null) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        public void CategoryNull(int id, string action)
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            Models.Category category = null;
            mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(id)).Returns(category);

            var categoryController = new CategoryController(mockUnitOfWork.Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Details":
                    result = categoryController.Details(id) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    result = categoryController.Edit(id) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = categoryController.Delete(id) as HttpStatusCodeResult;
                    break;
                case "DeleteTrue":
                    result = categoryController.Delete(id, true) as HttpStatusCodeResult;
                    break;
                default: //Ingredient
                    result = categoryController.Ingredients(id) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        public void ID(int id, string action)
        {
            IDNull(action);
            CategoryNull(id, action);
        }

        [Fact]
        public void Index_SortingAsc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, null, "")).Returns(categories);

            var CategoryController = new CategoryController(mockUnitOfWork.Object);

            var result = CategoryController.Index(null, "category", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Category>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(category6.Name, model.ElementAt(0).Name);
        }

        [Fact]
        public void Index_SortingDesc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, null, "")).Returns(categories);

            var CategoryController = new CategoryController(mockUnitOfWork.Object);

            var result = CategoryController.Index("Name_desc", "category", null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.Category>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(category1.Name, model.ElementAt(0).Name);
        }

        [Fact]
        public void Details()
        {
            ID(6, "Details");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(6)).Returns(category);

            var CategoryController = new CategoryController(mockUnitOfWork.Object);
            var result = CategoryController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Models.Category;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(category, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.SizeValuesSelectList()).Returns(new List<mono.DAL.UnitOfWork.TypeSelectList>());

            var CategoryController = new CategoryController(mockUnitOfWork.Object);
            var result = CategoryController.Create() as ViewResult;

            Assert.Equal(categoriesOrdered, (result.ViewBag.ParentCategoryID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();    
            mockUnitOfWork.Setup(m => m.CategoryRepository.Insert(category));
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();
            mockUnitOfWork.Setup(m => m.SizeValuesSelectList()).Returns(new List<mono.DAL.UnitOfWork.TypeSelectList>());

            var CategoryController = new CategoryController(mockUnitOfWork.Object);
            var result = CategoryController.Create(category) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(categoriesOrdered, (result.ViewBag.ParentCategoryID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {          
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.SizeValuesSelectList()).Returns(new List<mono.DAL.UnitOfWork.TypeSelectList>());

            var CategoryController = new CategoryController(mockUnitOfWork.Object);
            CategoryController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = CategoryController.Create(category) as ViewResult;

            Assert.Equal(categoriesOrdered, (result.ViewBag.ParentCategoryID as SelectList).Items);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {           
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Insert(category));

            var CategoryController = new CategoryController(mockUnitOfWork.Object);

            var result = CategoryController.Create(category) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get()
        {
            ID(6, "Edit");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(6)).Returns(category);
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.SizeValuesSelectList()).Returns(new List<mono.DAL.UnitOfWork.TypeSelectList>());

            var CategoryController = new CategoryController(mockUnitOfWork.Object);

            var result = CategoryController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Models.Category;

            Assert.Equal(categoriesOrdered, (result.ViewBag.ParentCategoryID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);           
            Assert.Equal(category, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Update(category));
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();
            mockUnitOfWork.Setup(m => m.SizeValuesSelectList()).Returns(new List<mono.DAL.UnitOfWork.TypeSelectList>());

            var CategoryController = new CategoryController(mockUnitOfWork.Object);
            var result = CategoryController.Edit(category) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal(categoriesOrdered, (result.ViewBag.ParentCategoryID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, null, "")).Returns(categories);
            mockUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Models.Category>, IOrderedQueryable<Models.Category>>>(), "")).Returns(categoriesOrdered);
            mockUnitOfWork.Setup(m => m.SizeValuesSelectList()).Returns(new List<mono.DAL.UnitOfWork.TypeSelectList>());

            var CategoryController = new CategoryController(mockUnitOfWork.Object);
            CategoryController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = CategoryController.Edit(category) as ViewResult;

            Assert.Equal(categoriesOrdered, (result.ViewBag.ParentCategoryID as SelectList).Items);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Update(category));         

            var CategoryController = new CategoryController(mockUnitOfWork.Object);

            var result = CategoryController.Edit(category) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get()
        {
            ID(6, "DeleteFalse");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(6)).Returns(category);

            var CategoryController = new CategoryController(mockUnitOfWork.Object);

            var result = CategoryController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Models.Category;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(category, model);
        }

        [Fact]
        public void Delete_GetError()
        {
            ID(6, "DeleteTrue");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(6)).Returns(category);

            var CategoryController = new CategoryController(mockUnitOfWork.Object);

            var result = CategoryController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Models.Category;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(category, model);
            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Delete(6));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var CategoryController = new CategoryController(mockUnitOfWork.Object);
            var result = CategoryController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.Delete(6));

            var CategoryController = new CategoryController(mockUnitOfWork.Object);

            var result = CategoryController.DeleteConfirmed(6) as RedirectToRouteResult;

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
            Models.Category childCategory = new Models.Category { ID = 6, Name ="childCategory", ParentCategoryID = 3 };
            Models.Ingredient childIngredient1 = new Models.Ingredient { Name = "childIngredient1", Category = childCategory };
            Models.Ingredient childIngredient2 = new Models.Ingredient { Name = "achildIngredient2", Category = childCategory };
            childCategory.Ingredients = new List<Models.Ingredient> { childIngredient1, childIngredient2 };

            var allIngreients = new List<Models.Ingredient> { childIngredient2, childIngredient1, parentIngredient1, parentIngredient2 };

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(6)).Returns(childCategory);
            mockUnitOfWork.Setup(m => m.CategoryRepository.GetByID(3)).Returns(parentCategory);

            var CategoryController = new CategoryController(mockUnitOfWork.Object);

            var result = CategoryController.Ingredients(6) as ViewResult;
            var model = result.ViewData.Model as List<Models.Ingredient>;

            Assert.Equal(childCategory.Name, result.ViewBag.Category);
            Assert.Equal("Ingredients", result.ViewName);
            Assert.Equal(allIngreients, model);
        }

    }
}
