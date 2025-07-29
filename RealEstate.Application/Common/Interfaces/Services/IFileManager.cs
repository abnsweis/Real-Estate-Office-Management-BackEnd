using FluentResults;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Interfaces.Services
{
    public interface IFileManager
    {  
        Task<Result<string>> SaveUserProfileImageAsync(IFormFile image);
        Task<Result<string>> SavePropertyImageAsync(IFormFile image);
        Task<Result<string>> SaveSaleContractImageAsync(IFormFile image);
        Task<Result<string>> SaveRentalContractImageAsync(IFormFile image);
        Task<Result<string>> SavePropertyVideoAsync(IFormFile video);
        Result DeleteFile(string fileName);
        Result<string> SetDefaultUserProfileImage();
        Result<string> SetDefaultContractImage();
        string GetPublicURL(string FilePath); 
        Result<string> GetDefaultFileUrl(string defaultFileKey, string FolderName); 
        void Initialize(); 
        void CreateIfNotExists(string path);
    }
}
