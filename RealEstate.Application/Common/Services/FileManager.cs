using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RealEstate.Application.Common.Errors;
using RealEstate.Domain.Enums;
using MediatR;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.Buffers.Text;
using Microsoft.EntityFrameworkCore.Internal;
using static System.Net.Mime.MediaTypeNames;
namespace RealEstate.Application.Common.Services
{
    public class FileManager : IFileManager
    {
        private readonly IAppEnvironmentService _env;
        private readonly IConfiguration _config;
        private readonly FileSettings _settings;

        public string Root { get; private set; }
        public Dictionary<string, string> AllFolderPaths { get; private set; }
        public Dictionary<string, string[]> AllAllowedExtensions { get; private set; }


        public FileManager(
            IAppEnvironmentService env,
            IConfiguration configuration,
            IOptions<FileSettings> fileSettingsOptions)
        { 
            _env = env;
            this._config = configuration;
            _settings = fileSettingsOptions.Value;


            Initialize();
        }


        public void Initialize()
        {
            Root = Path.Combine(_env.WebRootPath, _settings.Root);
            CreateIfNotExists(Root);


            AllFolderPaths = new Dictionary<string, string>();

            foreach (var folder in _settings.FolderPaths)
            {
                AllFolderPaths[folder.Key] = Path.Combine(Root,folder.Value);
                CreateIfNotExists(AllFolderPaths[folder.Key]);
            }

            AllAllowedExtensions = new Dictionary<string, string[]>();
            foreach (var extension in _settings.AllowedExtensions)
            {
                AllAllowedExtensions[extension.Key] = extension.Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public void CreateIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
 

        private async Task<Result<string>> SaveFileAsync(IFormFile file,string folderName, string subFolder)
        {
            if (file is null)
                return Result.Fail(new BadRequestError("File", "File is required. Please upload a valid file.", enApiErrorCode.MissingUploadedFile));
            
            var ext = Path.GetExtension(file.FileName);

            if (!AllFolderPaths.TryGetValue(folderName,out string? baseFolder))
            {
                return Result.Fail(new BadRequestError("FileCategory", $"Unsupported file category '{folderName}'.", enApiErrorCode.UnsupportedFileType));
            }
            if (!AllAllowedExtensions.TryGetValue(folderName, out var allowedExtensions))
            {
                return Result.Fail(new BadRequestError("Configuration", $"Allowed extensions not configured for '{folderName}'.", enApiErrorCode.ServerConfigurationError));
            }

            if (!allowedExtensions.Contains(ext))
                return Result.Fail(new BadRequestError("FileExtension", $"System Not Allowed This Extensions '{ext}' for Images , use({string.Join(" or ", AllAllowedExtensions).Replace('.', ' ')})", enApiErrorCode.InvalidFileExtension));

            var entityFolder = Path.Combine(baseFolder, subFolder);
            CreateIfNotExists(entityFolder);

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(entityFolder, uniqueName);
              
            try
            {
                await using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                var fileUrl = Path.GetRelativePath(_env.WebRootPath, fullPath).Replace('\\', '/');  
                return Result.Ok(fileUrl);
            }
            catch (Exception ex)
            {
                return Result.Fail(new InternalServerError("File", $"Error while saving file: {ex.Message}", enApiErrorCode.FileSaveError));
            }
        }
 
        public async Task<Result<string>> SaveUserProfileImageAsync(IFormFile file)
        {
            return await SaveFileAsync(file, "Images", "Users");  
        }

        public async Task<Result<string>> SavePropertyImageAsync(IFormFile file)
        {
            return await SaveFileAsync(file, "Images", "Propertys");
        }
        
        public async Task<Result<string>> SaveSaleContractImageAsync(IFormFile image)
        {
            return await SaveFileAsync(image, "Images", "Contracts/Sales");
        }        
        public async Task<Result<string>> SaveRentalContractImageAsync(IFormFile image)
        {
            return await SaveFileAsync(image, "Images", "Contracts/Rentals");
        }

   
         

        public Result DeleteFile(string fileName)
        {
            try
            {
                var fileUrl = $"{_env.WebRootPath}/{fileName}".Replace("/", "\\");

                if (!File.Exists(fileUrl)) {

                    return Result.Fail(new InternalServerError(
                        "File",
                        $"Not Found File With Name {fileName}"
                        , enApiErrorCode.MissingUploadedFile));

                }; 
                File.Delete(fileUrl);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new InternalServerError("File", $"Error while Deleting file: {ex.Message}", enApiErrorCode.FileDeleteError));
            }
             
        }


         
        public Result<string> GetDefaultFileUrl(string defaultFileKey , string FolderName)
        {
            var defaultImage = _settings.DefaultFiles[defaultFileKey].Replace("/","\\");
            var _src = Path.Combine(_env.WebRootPath, defaultImage);
            var ext = Path.GetExtension(defaultImage);

            if (string.IsNullOrEmpty(defaultFileKey) || string.IsNullOrEmpty(defaultImage))
            {
                return Result.Fail(new BadRequestError("DefaultFileKey", "Default File Key is required.", enApiErrorCode.MissingFileName));
            }

            if (!AllFolderPaths.TryGetValue("Images", out string? baseFolder))
            {
                return Result.Fail(new BadRequestError("FileCategory", $"Unsupported file category 'Images'.", enApiErrorCode.UnsupportedFileType));
            }
            if (!AllAllowedExtensions.TryGetValue("Images", out var allowedExtensions))
            {
                return Result.Fail(new BadRequestError("Configuration", $"Unsupported file category 'Images'.", enApiErrorCode.ServerConfigurationError));
            }

            if (!allowedExtensions.Contains(ext))
                return Result.Fail(new BadRequestError("FileExtension", $"System Not Allowed This Extensions '{ext}' for Images , use({string.Join(" or ", AllAllowedExtensions).Replace('.', ' ')})", enApiErrorCode.InvalidFileExtension));



            var entityFolder = Path.Combine(baseFolder, FolderName);
            CreateIfNotExists(entityFolder);

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(entityFolder, uniqueName);



            File.Copy(_src, fullPath);
            var fileUrl = Path.GetRelativePath(_env.WebRootPath, fullPath).Replace('\\', '/');
            return Result.Ok(fileUrl); 
        }

        public Result<string> SetDefaultUserProfileImage()
        {
            return GetDefaultFileUrl("UserProfileImage", "Users");
        }

        public Result<string> SetDefaultContractImage()
        {
            return GetDefaultFileUrl("ContractImage", "Contracts/Sales");
        }
        public string GetPublicURL(string FilePath)
        {
            return Path.Combine(_settings.BaseUrl, FilePath).Replace('\\', '/');
        }

        public async Task<Result<string>> SavePropertyVideoAsync(IFormFile video)
        {
            return await SaveFileAsync(video, "Videos", "Propertys");
        }
    }
}
