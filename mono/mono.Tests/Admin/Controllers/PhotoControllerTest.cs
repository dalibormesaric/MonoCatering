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
using Mono.Areas.Admin.Models;
using System.Web;
using System.Web.Helpers;
using System.Web.Routing;

namespace Mono.Tests.Admin.Controllers
{
    public class PhotoControllerTest
    {
        [Fact]
        public void Index()
        {
            //ControllerHelper.newSearchPageNumber

            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Get(It.IsAny<Expression<Func<Photo, bool>>>(), It.IsAny<Func<IQueryable<Photo>, IOrderedQueryable<Photo>>>(), It.IsAny<String>())).Returns(PhotoFake.photos);

            var photoController = new PhotoController(mockIUnitOfWork.Object, null, null);
            var result = photoController.Index("", "", "", 1) as ViewResult;
            var model = result.ViewData.Model as IEnumerable<Photo>;

            //ControllerHelper.newSearchPageNumber

            Assert.Equal("Index", result.ViewName);
            Assert.Equal(PhotoFake.photosPagedList, model);
        }

        [Fact]
        public void PhotoUpload()
        {
            var photoController = new PhotoController(null, null, null);
            var result = photoController.PhotoUpload() as ViewResult;

            Assert.Equal("PhotoUpload", result.ViewName);
        }

        [Fact]
        public void PhotoUploadSubmit_InvalidModel()
        {
            var photoController = new PhotoController(null, null, null);
            photoController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = photoController.PhotoUploadSubmit(PhotoFake.photoUploadViewModel) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("PhotoUpload", result.ViewName);
        }

        [Fact]
        public void PhotoUploadSubmit_ImageNull()
        {
            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.getImageFromRequest()).Returns(PhotoFake.webImageNull);

            var photoController = new PhotoController(null, mockIPhotoManager.Object, null);
            var result = photoController.PhotoUploadSubmit(PhotoFake.photoUploadViewModelImageNull) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("No image.", result.ViewData.ModelState[string.Empty].Errors.First().ErrorMessage);
            Assert.Equal("PhotoUpload", result.ViewName);
        }

        [Fact]
        public void PhotoUploadSubmit_FilaNameTaken()
        {
            var mockIUnitOfWork = new Mock<UnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photo);

            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.getImageFromRequest()).Returns(PhotoFake.webImage);

            var photoController = new PhotoController(mockIUnitOfWork.Object, mockIPhotoManager.Object, null);
            var result = photoController.PhotoUploadSubmit(PhotoFake.photoUploadViewModelImageNull) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("File name already taken.", result.ViewData.ModelState[string.Empty].Errors.First().ErrorMessage);
            Assert.Equal("PhotoUpload", result.ViewName);
        }
        
        [Fact]
        public void PhotoUploadSubmit_DataException()
        {          
            var mockIUnitOfWork = new Mock<UnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photoNull);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Insert(PhotoFake.photo));
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.getImageFromRequest()).Returns(PhotoFake.webImage);

            var photoController = new PhotoController(mockIUnitOfWork.Object, mockIPhotoManager.Object, null);
            var result = photoController.PhotoUploadSubmit(PhotoFake.photoUploadViewModelImageNull) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Unable to save changes. Try again, and if the problem persists contact your system administrator.", result.ViewData.ModelState[string.Empty].Errors.First().ErrorMessage);
            Assert.Equal("PhotoUpload", result.ViewName);
        }
        
        [Fact]
        public void PhotoUploadSubmit_Valid()
        {
            var mockIUnitOfWork = new Mock<UnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photoNull);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Insert(PhotoFake.photo));

            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.getImageFromRequest()).Returns(PhotoFake.webImage);

            var photoController = new PhotoController(mockIUnitOfWork.Object, mockIPhotoManager.Object, null);
            var result = photoController.PhotoUploadSubmit(PhotoFake.photoUploadViewModelImageNull) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Edit_Get_Null()
        {
            var photoController = new PhotoController(null, null, null);
            var result = photoController.Edit(String.Empty) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void Edit_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photoNull);

            var photoController = new PhotoController(mockIUnitOfWork.Object, null, null);
            var result = photoController.Edit("6") as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photo);

            var photoController = new PhotoController(mockIUnitOfWork.Object, null, null);
            var result = photoController.Edit("6") as ViewResult;
            var model = result.ViewData.Model as PhotoEditViewModel;

            Assert.Equal("Edit", result.ViewName);
            Assert.Equal(PhotoFake.photo.FileName, model.FileName);
            Assert.Equal(PhotoFake.photo.FileName, model.NewFileName);
        }

        [Fact]
        public void Edit_InvalidModel()
        {
            var photoController = new PhotoController(null, null, null);
            photoController.ModelState.AddModelError(string.Empty, "Invalid model");   //because Model Binding doesn’t work
            var result = photoController.Edit(PhotoFake.photoEditViewModel) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_NameTaken()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photo);
           
            var photoController = new PhotoController(mockIUnitOfWork.Object, null, null);
            var result = photoController.Edit(PhotoFake.photoEditViewModel) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("File name already taken.", result.ViewData.ModelState[string.Empty].Errors.First().ErrorMessage);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_PhotoNotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photoNull);

            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.photoExist(It.IsAny<string>())).Returns(false);

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns("");

            var photoController = new PhotoController(mockIUnitOfWork.Object, mockIPhotoManager.Object, server.Object);
            var result = photoController.Edit(PhotoFake.photoEditViewModel) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Photo doesn't exist.", result.ViewData.ModelState[string.Empty].Errors.First().ErrorMessage);
            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public void Edit_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photoNull);
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();

            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.photoExist(It.IsAny<string>())).Returns(true);

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns("");

            var photoController = new PhotoController(mockIUnitOfWork.Object, mockIPhotoManager.Object, server.Object);
            var result = photoController.Edit(PhotoFake.photoEditViewModel) as ViewResult;

            Assert.Equal(false, result.ViewData.ModelState.IsValid);
            Assert.Equal("Unable to save changes. Try again, and if the problem persists contact your system administrator.", result.ViewData.ModelState[string.Empty].Errors.First().ErrorMessage);
            Assert.Equal("Edit", result.ViewName);
        }



        [Fact]
        public void Edit_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photoNull);
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Delete(It.IsAny<object>()));
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Insert(It.IsAny<Photo>()));

            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.photoExist(It.IsAny<string>())).Returns(true);

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns("");

            var photoController = new PhotoController(mockIUnitOfWork.Object, mockIPhotoManager.Object, server.Object);
            var result = photoController.Edit(PhotoFake.photoEditViewModel) as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void Delete_Get_Null()
        {
            var photoController = new PhotoController(null, null, null);
            var result = photoController.Delete(string.Empty) as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void Delete_Get_NotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID(It.IsAny<string>())).Returns(PhotoFake.photoNull);

            var photoController = new PhotoController(mockIUnitOfWork.Object, null, null);
            var result = photoController.Delete("5") as HttpStatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Delete_Get_ErrorMessage()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID("6")).Returns(PhotoFake.photo);

            var photoController = new PhotoController(mockIUnitOfWork.Object, null, null);
            var result = photoController.Delete("6", true) as ViewResult;
            var model = result.ViewData.Model as Photo;

            Assert.Equal("Delete failed. Try again, and if the problem persists see your system administrator.", result.ViewBag.ErrorMessage);
            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(PhotoFake.photo, model);
        }

        [Fact]
        public void Delete_Get()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID("6")).Returns(PhotoFake.photo);

            var photoController = new PhotoController(mockIUnitOfWork.Object, null, null);
            var result = photoController.Delete("6") as ViewResult;
            var model = result.ViewData.Model as Photo;

            Assert.Equal("Delete", result.ViewName);
            Assert.Equal(PhotoFake.photo, model);
        }

        [Fact]
        public void DeleteConfirmed_PhotoNotFound()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID("6")).Returns(PhotoFake.photo);

            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.photoExist(It.IsAny<string>())).Returns(false);

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns("");

            var photoController = new PhotoController(mockIUnitOfWork.Object, mockIPhotoManager.Object, server.Object);
            var result = photoController.DeleteConfirmed("6") as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal("6", result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_DataException()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID("6")).Returns(PhotoFake.photo);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(It.IsAny<Expression<Func<Category, bool>>>(), null, "")).Returns(new List<Category>());
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(It.IsAny<Expression<Func<Food, bool>>>(), null, "")).Returns(new List<Food>());
            mockIUnitOfWork.Setup(m => m.Save()).Throws<DataException>();           

            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.photoExist(It.IsAny<string>())).Returns(true);

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns("");

            var photoController = new PhotoController(mockIUnitOfWork.Object, mockIPhotoManager.Object, server.Object);
            var result = photoController.DeleteConfirmed("6") as RedirectToRouteResult;

            Assert.Equal("Delete", result.RouteValues["action"]);
            Assert.Equal("6", result.RouteValues["id"]);
            Assert.Equal(true, result.RouteValues["saveChangesError"]);
        }

        [Fact]
        public void DeleteConfirmed_Valid()
        {
            var mockIUnitOfWork = new Mock<IUnitOfWork>();
            mockIUnitOfWork.Setup(m => m.PhotoRepository.GetByID("6")).Returns(PhotoFake.photo);
            mockIUnitOfWork.Setup(m => m.CategoryRepository.Get(It.IsAny<Expression<Func<Category, bool>>>(), null, "")).Returns(new List<Category>());
            mockIUnitOfWork.Setup(m => m.FoodRepository.Get(It.IsAny<Expression<Func<Food, bool>>>(), null, "")).Returns(new List<Food>());
            mockIUnitOfWork.Setup(m => m.PhotoRepository.Delete(It.IsAny<object>()));
            mockIUnitOfWork.Setup(m => m.Save());

            var mockIPhotoManager = new Mock<IPhotoManager>();
            mockIPhotoManager.Setup(m => m.photoExist(It.IsAny<string>())).Returns(true);
            mockIPhotoManager.Setup(m => m.deletePhoto(It.IsAny<string>()));

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns("");

            var photoController = new PhotoController(mockIUnitOfWork.Object, mockIPhotoManager.Object, server.Object);
            var result = photoController.DeleteConfirmed("6") as RedirectToRouteResult;

            Assert.Equal("Index", result.RouteValues["action"]);
        }

    }
}
