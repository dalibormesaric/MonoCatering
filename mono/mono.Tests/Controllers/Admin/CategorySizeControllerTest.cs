using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using mono.Areas.Admin.Controllers;
using System.Web.Mvc;
using System.Net;
using System.Data;
using System.Linq.Expressions;

namespace mono.Tests.Controllers.Admin
{
    public class CategorySizeSizeControllerTest
    {
        private Models.CategorySize categorySize;
        private Models.CategorySize categorySize1, categorySize2, categorySize3, categorySize4, categorySize5, categorySize6;
        private IQueryable<Models.CategorySize> categorySizes, categorySizesOrdered;

        public CategorySizeSizeControllerTest()
        {
            categorySize = new Models.CategorySize { Value = "categorySize", Type = 0 };

            categorySize1 = new Models.CategorySize { Value = "categorySize1", Type = 0 };
            categorySize2 = new Models.CategorySize { Value = "cdfgegrySize2", Type = 0 };
            categorySize3 = new Models.CategorySize { Value = "casdfgrySize3", Type = 0 };
            categorySize4 = new Models.CategorySize { Value = "categorySize4", Type = 0 };
            categorySize5 = new Models.CategorySize { Value = "categorySize5", Type = 0 };
            categorySize6 = new Models.CategorySize { Value = "categorySize6", Type = 0 };

            categorySizes = new List<Models.CategorySize> { categorySize1, categorySize2, categorySize3, categorySize4, categorySize5, categorySize6 }.AsQueryable();

            categorySizesOrdered = categorySizes.OrderBy(c => c.Value);
        }

        public void IDNull(string action)
        {
            var categorySizeController = new CategorySizeController(new Mock<mono.DAL.UnitOfWork>().Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Details":
                    result = categorySizeController.Details(null) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    int? value = null;
                    result = categorySizeController.Edit(value) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = categorySizeController.Delete(null) as HttpStatusCodeResult;
                    break;
                default: //DeleteTrue
                    result = categorySizeController.Delete(null, true) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
        }

        public void CategorySizeNull(int id, string action)
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            Models.CategorySize categorySize = null;
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(id)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);

            HttpStatusCodeResult result;

            switch (action)
            {
                case "Details":
                    result = categorySizeController.Details(id) as HttpStatusCodeResult;
                    break;
                case "Edit":
                    result = categorySizeController.Edit(id) as HttpStatusCodeResult;
                    break;
                case "DeleteFalse":
                    result = categorySizeController.Delete(id) as HttpStatusCodeResult;
                    break;
                default: //DeleteTrue
                    result = categorySizeController.Delete(id, true) as HttpStatusCodeResult;
                    break;
            }

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        public void ID(int id, string action)
        {
            IDNull(action);
            CategorySizeNull(id, action);
        }

        private static string searchString = "categorySize";
        private Expression<Func<Models.CategorySize, bool>> filter = (s =>
            s.Type.ToString().Contains(searchString.ToUpper()) ||
            s.Value.ToUpper().Contains(searchString.ToUpper())
        );
        private Func<IQueryable<Models.CategorySize>, IOrderedQueryable<Models.CategorySize>> orderBy = (q => q.OrderBy(s => s.Value));
        private Func<IQueryable<Models.CategorySize>, IOrderedQueryable<Models.CategorySize>> orderByDescending = (q => q.OrderByDescending(s => s.Value));

        [Fact]
        public void Index_SortingAsc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<Models.CategorySize, bool>>>(), It.IsAny<Func<IQueryable<Models.CategorySize>, IOrderedQueryable<Models.CategorySize>>>(), It.IsAny<String>())).Returns(orderBy(categorySizes.Where(filter)));

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);

            var result = categorySizeController.Index(null, searchString, null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.CategorySize>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(categorySize6.Value, model.ElementAt(0).Value);
        }

        [Fact]
        public void Index_SortingDesc_Filter_PerPage_Page()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<Models.CategorySize, bool>>>(), It.IsAny<Func<IQueryable<Models.CategorySize>, IOrderedQueryable<Models.CategorySize>>>(), It.IsAny<String>())).Returns(orderByDescending(categorySizes.Where(filter)));

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);

            var result = categorySizeController.Index("Value_desc", searchString, null, 2) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Models.CategorySize>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(1, model.Count());
            Assert.Equal(categorySize1.Value, model.ElementAt(0).Value);
        }

        [Fact]
        public void Details()
        {
            ID(6, "Details");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);
            var result = categorySizeController.Details(6) as ViewResult;
            var model = result.ViewData.Model as Models.CategorySize;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(categorySize, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);
            var result = categorySizeController.Create() as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();    
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Insert(categorySize));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);
            var result = categorySizeController.Create(categorySize) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {          
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);
            categorySizeController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = categorySizeController.Create(categorySize) as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {           
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Insert(categorySize));

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);

            var result = categorySizeController.Create(categorySize) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get()
        {
            ID(6, "Edit");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);

            var result = categorySizeController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as Models.CategorySize;

            Assert.Equal("Edit", result.ViewName);           
            Assert.Equal(categorySize, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Update(categorySize));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);
            var result = categorySizeController.Edit(categorySize) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Get(null, null, "")).Returns(categorySizes);

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);
            categorySizeController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = categorySizeController.Edit(categorySize) as ViewResult;

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Update(categorySize));         

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);

            var result = categorySizeController.Edit(categorySize) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get()
        {
            ID(6, "DeleteFalse");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);

            var result = categorySizeController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as Models.CategorySize;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(categorySize, model);
        }

        [Fact]
        public void Delete_GetError()
        {
            ID(6, "DeleteTrue");

            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);

            var result = categorySizeController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as Models.CategorySize;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(categorySize, model);
            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Delete(6));
            mockUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);
            var result = categorySizeController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal(6, result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Delete(6));

            var categorySizeController = new CategorySizeController(mockUnitOfWork.Object);

            var result = categorySizeController.DeleteConfirmed(6) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Type()
        {
            CategorySizeController categorySizeController;
            Mock<mono.DAL.UnitOfWork> mockUnitOfWork;
            HttpStatusCodeResult result;

            //IDNull
            categorySizeController = new CategorySizeController(new Mock<mono.DAL.UnitOfWork>().Object);
            result = categorySizeController.Type(null) as HttpStatusCodeResult;
            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);

            //CategorySizeNull
            mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            List<Models.CategorySize> categorySizeList = new List<Models.CategorySize>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<Models.CategorySize, bool>>>(), null, "")).Returns(categorySizeList);
            

            categorySizeController = new CategorySizeController(mockUnitOfWork.Object);
            result = categorySizeController.Type(6) as HttpStatusCodeResult;
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);

            //CategorySize
            mockUnitOfWork = new Mock<mono.DAL.UnitOfWork>();
            mockUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<Models.CategorySize, bool>>>(), null, "")).Returns(new List<Models.CategorySize> { categorySize } );
            mockUnitOfWork.Setup(m => m.SizeValuesString(It.IsAny<int>()));

            categorySizeController = new CategorySizeController(mockUnitOfWork.Object);
            var result2 = categorySizeController.Type(6) as ViewResult;
            Assert.Equal("Type", result2.ViewName); 
        }
    }
}
