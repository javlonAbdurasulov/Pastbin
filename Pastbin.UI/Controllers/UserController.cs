using Microsoft.AspNetCore.Mvc;
using Pastbin.Application.Interfaces;
using Pastbin.Domain.Entities;
using Pastbin.Domain.Models;
using Pastbin.Domain.Models.DTO;

namespace Pastbin.UI.Controllers
{
    [ApiController]
    [Route("pastbin/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<ResponseModel<UserDTO>> CreateAsync(UserCreateDTO userCreateDTO)
        {
            if (userCreateDTO == null) { return new ("User is null"); }
            var hasInDb=await _userService.GetByUsername(userCreateDTO.Username);
            if (hasInDb!=null) { return new("User has in Db"); }
            User newUser=new User() { 
            Username= userCreateDTO.Username,
            Password= userCreateDTO.Password
            };
            var response = await _userService.CreateAsync(newUser);
            UserDTO responseModel = new()
            {
                Username = response.Username,
                Posts = new List<int>()
            };

            return new(responseModel);
        }

        [HttpGet]
        public async Task<ResponseModel<IEnumerable<UserDTO>>> GetAllAsync()
        {
            var Users=await _userService.GetAllAsync();
            IEnumerable<UserDTO> responseList = Users.Select(p => new UserDTO()
            {
                Username = p.Username,
                Posts = p.Posts==null ? new List<int>() : p.Posts.Select(a=>a.Id).ToList()
            });
            return new(responseList);
        }

    }
}
