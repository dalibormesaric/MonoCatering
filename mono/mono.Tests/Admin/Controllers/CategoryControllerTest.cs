using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using Mono.Areas.Admin.Controllers;
using System.Web.Mvc;
using System.Net;
using System.Data;
using System.Linq.Expressions;
using Mono.Model;
using Mono.Data;

using Mono.Tests.Admin.Fake;

namespace Mono.Tests.Admin.Controllers
{
    public class CategoryControllerTest
    {
        [Fact]
        public void Index()
        {
            //ControllerHelper.newSearchPageNumber

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), It.IsAny<String>())).Returns(CategoryFake.categories);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Index("", "", "", 1, null) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Category>;

            //ControllerHelper.newSearchPageNumber

            Assert.Equal("Index", result.ViewName);
            Assert.Equal(CategoryFake.categoriesPagedList, model);
        }

        [Fact]
        public void SubCategory_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(CategoryFake.categoryNull);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.SubCategory(5, "", "", "", (int?)null) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void SubCategory()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(CategoryFake.categoryWithChilds);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.SubCategory(5, "", "", "", (int?)null) as ViewResult;

            Assert.Equal("SubCategory", result.ViewName);
        }

        [Fact]
        public void Details_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(CategoryFake.categoryNull);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Details(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Details()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(CategoryFake.category);
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesString(It.IsAny<int>())).Returns("");

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Category;

            //ViewBag.SizeValues 

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(CategoryFake.category, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(CategoryFake.categories);
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesSelectList()).Returns(CategoryFake.typeSelectList);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(CategoryFake.photoList);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Create() as ViewResult;

            setViewBagsParametres(result);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Insert(CategoryFake.category));
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(CategoryFake.categories);
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesSelectList()).Returns(CategoryFake.typeSelectList);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(CategoryFake.photoList);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Create(CategoryFake.category) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            setViewBagsParametres(result);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {          
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(CategoryFake.categories);
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesSelectList()).Returns(CategoryFake.typeSelectList);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(CategoryFake.photoList);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            categoryController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = categoryController.Create(CategoryFake.category) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            setViewBagsParametres(result);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {           
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Insert(CategoryFake.category));

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Create(CategoryFake.category) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(CategoryFake.categoryNull);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Edit(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(6)).Returns(CategoryFake.category);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(CategoryFake.categories);
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesSelectList()).Returns(CategoryFake.typeSelectList);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(CategoryFake.photoList);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Category;

            setViewBagsParametres(result);
            Assert.Equal("Edit", result.ViewName);
            Assert.Equal(CategoryFake.category, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Update(CategoryFake.category));
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(CategoryFake.categories);
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesSelectList()).Returns(CategoryFake.typeSelectList);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(CategoryFake.photoList);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Edit(CategoryFake.category) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            setViewBagsParametres(result);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, null, "")).Returns(CategoryFake.categories);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(null, It.IsAny<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(), "")).Returns(CategoryFake.categories);
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesSelectList()).Returns(CategoryFake.typeSelectList);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(null, null, "")).Returns(CategoryFake.photoList);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            categoryController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = categoryController.Edit(CategoryFake.category) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            setViewBagsParametres(result);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Update(CategoryFake.category));         

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Edit(CategoryFake.category) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(CategoryFake.categoryNull);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Delete(5) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Delete_Get_ErrorMessage()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(CategoryFake.category);
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesString(It.IsAny<int>())).Returns("");

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Category;

            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(CategoryFake.category, model);
        }

        [Fact]
        public void Delete_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(CategoryFake.category);
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesString(It.IsAny<int>())).Returns("");

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Category;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(CategoryFake.category, model);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Delete(6));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Delete(6));

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Ingredients_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(It.IsAny<int>())).Returns(CategoryFake.categoryNull);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);
            var result = categoryController.Ingredients(6) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Ingredients()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(6)).Returns(CategoryFake.childCategory);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.GetByID(3)).Returns(CategoryFake.parentCategory);

            var categoryController = new CategoryController(mockIUnitOfWork.Object);

            var result = categoryController.Ingredients(6) as ViewResult;
            var model = result.ViewData.Model as List<Ingredient>;

            Assert.Equal(CategoryFake.childCategory.Name, result.ViewBag.Category);
            Assert.Equal("Ingredients", result.ViewName);
            Assert.Equal(CategoryFake.allIngredients, model);
        }

        private void setViewBagsParametres(ViewResult result)
        {
            Assert.Equal(CategoryFake.categories, (result.ViewBag.ParentCategoryID as SelectList).Items);
            Assert.Equal(CategoryFake.typeSelectList, (result.ViewBag.SizeType as SelectList).Items);
            Assert.Equal(CategoryFake.photoList.Select(p => p.FileName), (result.ViewBag.Photos as SelectList).Items);
        }

    }
}
