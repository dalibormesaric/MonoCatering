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
using Mono.Data;
using Mono.Model;
using Mono.Tests.Admin.Fake;

namespace Mono.Tests.Admin.Controllers
{
    public class CategorySizeControllerTest
    {
        [Fact]
        public void Index()
        {
            //ControllerHelper.newSearchPageNumber

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<CategorySize, bool>>>(), It.IsAny<Func<IQueryable<CategorySize>, IOrderedQueryable<CategorySize>>>(), It.IsAny<String>())).Returns(CategorySizeFake.categorySizes);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);

            var result = categorySizeController.Index("", "", "", 1) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<CategorySize>;

            //ViewBag.

            Assert.Equal("Index", result.ViewName);
            Assert.Equal(CategorySizeFake.categoriesPagedList, model);
        }

        [Fact]
        public void Details_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(It.IsAny<int>())).Returns(CategorySizeFake.categorySizeNull);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Details(5) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Details()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(CategorySizeFake.categorySize);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Details(6) as ViewResult;
            var model = result.ViewData.Model as CategorySize;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(CategorySizeFake.categorySize, model);
        }

        [Fact]
        public void Create_Get()
        {
            var categorySizeController = new CategorySizeController(null);
            var result = categorySizeController.Create() as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();    
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Insert(CategorySizeFake.categorySize));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Create(CategorySizeFake.categorySize) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {          
            var mockIUnitOfWork = new Mock<IUnitOfWork>();

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            categorySizeController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = categorySizeController.Create(CategorySizeFake.categorySize) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {           
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Insert(CategorySizeFake.categorySize));

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Create(CategorySizeFake.categorySize) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(It.IsAny<int>())).Returns(CategorySizeFake.categorySizeNull);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Edit(5) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(CategorySizeFake.categorySize);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as CategorySize;

            Assert.Equal("Edit", result.ViewName);
            Assert.Equal(CategorySizeFake.categorySize, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Update(CategorySizeFake.categorySize));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Edit(CategorySizeFake.categorySize) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Get(null, null, "")).Returns(CategorySizeFake.categorySizes);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            categorySizeController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = categorySizeController.Edit(CategorySizeFake.categorySize) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Update(CategorySizeFake.categorySize));         

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Edit(CategorySizeFake.categorySize) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(It.IsAny<int>())).Returns(CategorySizeFake.categorySizeNull);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Delete(5) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Delete_Get_Delete_Get_ErrorMessage()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(CategorySizeFake.categorySize);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as CategorySize;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(CategorySizeFake.categorySize, model);
        }

        [Fact]
        public void Delete_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(CategorySizeFake.categorySize);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as CategorySize;

            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(CategorySizeFake.categorySize, model);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Delete(6));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Delete(6));

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Type_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<CategorySize,bool>>>(), null, "")).Returns(new List<CategorySize> { CategorySizeFake.categorySizeNull } );

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Delete(5) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Type()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<CategorySize, bool>>>(), null, "")).Returns(new List<CategorySize> { CategorySizeFake.categorySize } );
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.SizeValuesString(It.IsAny<int>()));

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Type(6) as ViewResult;

            Assert.Equal("Type", result.ViewName); 
        }
    }
}
