using System;
using System.Web.Helpers;

namespace Mono.Data
{
    public class PhotoManager : Mono.Data.IPhotoManager
    {
        public virtual void changePhotoName(string oldFilePath, string newFilePath)
        {
            System.IO.File.Move(oldFilePath, newFilePath);
        }

        public virtual void deletePhoto(string filePath)
        {
            System.IO.File.Delete(filePath);
        }

        public virtual bool photoExist(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }
        public virtual WebImage getImageFromRequest()
        {
            return WebImage.GetImageFromRequest();
        }

        public virtual void resize(ref WebImage image, int width,int height)
        {
            image.Resize(width, height, false);
        }

        public virtual void savePhoto(ref WebImage image, string filePath, string imageFormat)
        {
            image.Save(filePath, imageFormat);
        }
    }
}
