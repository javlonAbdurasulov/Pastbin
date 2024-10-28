using Microsoft.AspNetCore.Mvc;
using Pastbin.Application.Interfaces;
using Pastbin.Domain.Entities;
using Pastbin.Domain.Models;
using Pastbin.Domain.Models.DTO;

namespace Pastbin.UI.Controllers
{
    [Route("pastbin/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        public PostsController(IUserService _userService, IPostService postService)
        {
            this._userService = _userService;
            _postService = postService;

        }
        [HttpPost("Create")]
        public async Task<ResponseModel<Post>> CreatePost(PostDTO postDTO)
        {
            User user = await _userService.GetByUsername(postDTO.UserName);

            if (user == null) return new($"User {postDTO.UserName} not found");

            Post post = new Post()
            {
                ExpireHour = postDTO.ExpireHour,
                User = user,
            };

            Post response = await _postService.CreateAsync(post, postDTO.Text);
            return new(response);
        }

        [HttpGet("{keyword}")]
        public async Task<IActionResult> GetUrlsForKeyword(string keyword)
        {
            var posts = _postService.GetAllAsync().Result.ToList();
            var urls = posts.Where(k => k.HashUrl == keyword)
                    .Select(k => k.UrlAWS)
                    .ToList();

            if (urls == null || urls.Count == 0)
            {
                return NotFound();
            }

            return Ok(urls);
        }

        [HttpGet("GetAllFromUsername")]
        public async Task<ResponseModel<IEnumerable<Post>>> GetAllFromUsernameAsync(string Username)
        {
            var Posts = await _postService.GetAllFromUsernameAsync(Username);
            if (Posts == null) return new("in db not exist");
            return new(Posts);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(_postService.GetAllAsync());
        }
        [HttpDelete("Delete")]
        public async Task<ResponseModel<string>> DeleteAsync(PostDeleteDTO postDeleteDTO)
        {
            ResponseModel<string> result = await _postService.DeleteAsync(postDeleteDTO.hashUrl, postDeleteDTO.username);
            return result;
        }

    }
}
