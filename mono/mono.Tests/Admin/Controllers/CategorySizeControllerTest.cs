﻿using System;
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

namespace Mono.Tests.Admin.Controllers
{
    public class CategorySizeSizeControllerTest
    {
        private CategorySize categorySize;
        private CategorySize categorySize1, categorySize2, categorySize3, categorySize4, categorySize5, categorySize6;
        private IQueryable<CategorySize> categorySizes, categorySizesOrdered;

        public CategorySizeSizeControllerTest()
        {
            categorySize = new CategorySize { Value = "categorySize", Type = 0 };

            categorySize1 = new CategorySize { Value = "categorySize1", Type = 0 };
            categorySize2 = new CategorySize { Value = "cdfgegrySize2", Type = 0 };
            categorySize3 = new CategorySize { Value = "casdfgrySize3", Type = 0 };
            categorySize4 = new CategorySize { Value = "categorySize4", Type = 0 };
            categorySize5 = new CategorySize { Value = "categorySize5", Type = 0 };
            categorySize6 = new CategorySize { Value = "categorySize6", Type = 0 };

            categorySizes = new List<CategorySize> { categorySize1, categorySize2, categorySize3, categorySize4, categorySize5, categorySize6 }.AsQueryable();

            categorySizesOrdered = categorySizes.OrderBy(c => c.Value);
        }

        public void IDNull(string action)
        {
            var categorySizeController = new CategorySizeController(new Mock<IUnitOfWork>().Object);

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
            var mockIUnitOfWork = new Mock<IUnitOfWork>();

            CategorySize categorySize = null;
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(id)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);

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
        private Expression<Func<CategorySize, bool>> filter = (s =>
            s.Type.ToString().Contains(searchString.ToUpper()) ||
            s.Value.ToUpper().Contains(searchString.ToUpper())
        );
        private Func<IQueryable<CategorySize>, IOrderedQueryable<CategorySize>> orderBy = (q => q.OrderBy(s => s.Value));
        private Func<IQueryable<CategorySize>, IOrderedQueryable<CategorySize>> orderByDescending = (q => q.OrderByDescending(s => s.Value));

        [Fact]
        public void Index_SortingAsc_Filter_PerPage_Page()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<CategorySize, bool>>>(), It.IsAny<Func<IQueryable<CategorySize>, IOrderedQueryable<CategorySize>>>(), It.IsAny<String>())).Returns(orderBy(categorySizes.Where(filter)));

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);

            var result = categorySizeController.Index(null, searchString, null, 1) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<CategorySize>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(categorySize1.Value, model.ElementAt(0).Value);
        }

        [Fact]
        public void Index_SortingDesc_Filter_PerPage_Page()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<CategorySize, bool>>>(), It.IsAny<Func<IQueryable<CategorySize>, IOrderedQueryable<CategorySize>>>(), It.IsAny<String>())).Returns(orderByDescending(categorySizes.Where(filter)));

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);

            var result = categorySizeController.Index("Value_desc", searchString, null, 1) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<CategorySize>;

            Assert.Equal("Index", result.ViewName);

            Assert.Equal(categorySize6.Value, model.ElementAt(0).Value);
        }

        [Fact]
        public void Details()
        {
            ID(6, "Details");

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Details(6) as ViewResult;
            var model = result.ViewData.Model as CategorySize;

            Assert.Equal("Details", result.ViewName);
            Assert.Equal(categorySize, model);
        }

        [Fact]
        public void Create_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Create() as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();    
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Insert(categorySize));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Create(categorySize) as ViewResult;
            
            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_InvalidModel()
        {          
            var mockIUnitOfWork = new Mock<IUnitOfWork>();

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            categorySizeController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = categorySizeController.Create(categorySize) as ViewResult;

            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Valid()
        {           
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Insert(categorySize));

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);

            var result = categorySizeController.Create(categorySize) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get()
        {
            ID(6, "Edit");

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);

            var result = categorySizeController.Edit(6) as ViewResult;
            var model = result.ViewData.Model as CategorySize;

            Assert.Equal("Edit", result.ViewName);           
            Assert.Equal(categorySize, model);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Update(categorySize));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result = categorySizeController.Edit(categorySize) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Get(null, null, "")).Returns(categorySizes);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            categorySizeController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work

            var result = categorySizeController.Edit(categorySize) as ViewResult;

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Update(categorySize));         

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);

            var result = categorySizeController.Edit(categorySize) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get()
        {
            ID(6, "DeleteFalse");

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);

            var result = categorySizeController.Delete(6) as ViewResult;
            var model = result.ViewData.Model as CategorySize;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(categorySize, model);
        }

        [Fact]
        public void Delete_GetError()
        {
            ID(6, "DeleteTrue");

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.GetByID(6)).Returns(categorySize);

            var categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);

            var result = categorySizeController.Delete(6, true) as ViewResult;
            var model = result.ViewData.Model as CategorySize;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(categorySize, model);
            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
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
        public void Type()
        {
            CategorySizeController categorySizeController;
            Mock<IUnitOfWork> mockIUnitOfWork;
            HttpStatusCodeResult result;

            //IDNull
            categorySizeController = new CategorySizeController(new Mock<IUnitOfWork>().Object);
            result = categorySizeController.Type(null) as HttpStatusCodeResult;
            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);

            //CategorySizeNull
            mockIUnitOfWork = new Mock<IUnitOfWork>();
            List<CategorySize> categorySizeList = new List<CategorySize>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<CategorySize, bool>>>(), null, "")).Returns(categorySizeList);
            

            categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            result = categorySizeController.Type(6) as HttpStatusCodeResult;
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);

            //CategorySize
            mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.CategorySizeRepository.Get(It.IsAny<Expression<Func<CategorySize, bool>>>(), null, "")).Returns(new List<CategorySize> { categorySize } );
            mockIUnitOfWork.Setup(m => m.SizeValuesString(It.IsAny<int>()));

            categorySizeController = new CategorySizeController(mockIUnitOfWork.Object);
            var result2 = categorySizeController.Type(6) as ViewResult;
            Assert.Equal("Type", result2.ViewName); 
        }
    }
}