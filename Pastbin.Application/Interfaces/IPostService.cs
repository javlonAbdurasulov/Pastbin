using Pastbin.Domain.Entities;
using Pastbin.Domain.Models;

namespace Pastbin.Application.Interfaces
{
    public interface IPostService : ICRUDService<Post>
    {
        public Task<IEnumerable<Post>> GetAllFromUsernameAsync(string username);
        public Task<Post> CreateAsync(Post post, string text);
        public Task<ResponseModel<string>> DeleteAsync(string hashUrl, string username);
    }
}
