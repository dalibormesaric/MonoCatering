using System;
using System.Web.Helpers;

namespace Mono.Data
{
    public interface IPhotoManager
    {
        void changePhotoName(string oldFilePath, string newFilePath);
        void deletePhoto(string filePath);
        bool photoExist(string filePath);
        WebImage getImageFromRequest();
        void resize(ref WebImage image, int width, int height);
        void savePhoto(ref WebImage webImage, string filePath, string imageFormat);
    }
}
