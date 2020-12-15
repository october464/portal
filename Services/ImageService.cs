using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Services
{
    public class ImageService : IImageService
    {
        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            //Uploads
            MemoryStream memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var byteFile = memoryStream.ToArray();
            memoryStream.Close();
            memoryStream.Dispose();

            return byteFile;
        }

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            //Turns it into a usable image
            string imageBase64Data = Convert.ToBase64String(fileData);

            //Converts img from byte array to src 
            return string.Format($"data:image/{extension};base64, {imageBase64Data}");
        }

        public  Task<byte[]> AssignDefaultAvatarAsync(string avatar)
        {
            var defaultAvatarPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/assets/img", avatar);
            return  File.ReadAllBytesAsync(defaultAvatarPath);
        }
    }
}
