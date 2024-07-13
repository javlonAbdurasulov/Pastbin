using Pastbin.Application.Interfaces;
using Pastbin.Application.Services;
using Pastbin.Domain.Entities;
namespace Pastbin.Infrastructure.Services
{
    public class PostService : IPostService
    {
        public Task<Post> CreateAsync(Post entity,string Text)
        {
            _db = db;
            _fileService = fileService;
        }
        public async Task<Post> CreateAsync(Post entity, string text)
        {
            // Создание MemoryStream из текста
            byte[] byteArray = Encoding.UTF8.GetBytes(text);
            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                string filename = $"{Guid.NewGuid()}.txt";
                var response = await _fileService.UploadFileAsync("shokir-demo-bucket", memoryStream, filename, entity.ExpireHour, entity.User.Username);
                
                entity.UrlAWS = response.UploadedFilePath;
                entity.HashUrl = string.Join("", HashGenerator.sha256_hash(response.UploadedFilePath).Select(x => x).Take(40));
                entity.fileName = filename;
                entity.CreateTime = DateTime.Now.ToUniversalTime();
                entity.EndTime = entity.CreateTime.AddHours(entity.ExpireHour).ToUniversalTime();
            }
            await _db.Posts.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        public async Task<string> DeleteAsync(string hashUrl, string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null) return $"{username} doesn't exist in database";

            var post = await _db.Posts.FirstOrDefaultAsync(x => x.HashUrl == hashUrl);

            bool response = await _fileService.DeleteFileAsync("shokir-demo-bucket", $"{username}/{post.fileName}");
            if (response) return "Object successfully deleted";

            return "an error occurred while deleting";
        }

        public async Task<IEnumerable<Post>> GetAllFromUsernameAsync(string username)
        {
            var user = await _db.Users
            .Include(u => u.Posts)
            .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return new List<Post>();

            return user.Posts;
        }
        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> UpdateAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
