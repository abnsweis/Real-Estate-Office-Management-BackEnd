using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Interfaces.Services;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IFileManager fileManager;

        public TestController(IFileManager fileManager)
        {
            this.fileManager = fileManager;
        }



        [HttpPost]
        public async Task<IActionResult> Get(IFormFile file)
        {
             


            var test1 = fileManager.SetDefaultUserProfileImage()    ; 


            return Ok(new
            {
                
                test1 = fileManager.GetPublicURL(test1.Value)
            });
        }
           

    }
}
