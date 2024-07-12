﻿using Microsoft.EntityFrameworkCore;
using Pastbin.Application.Interfaces;
using Pastbin.Application.Services;
using Pastbin.Domain.Entities;
using Pastbin.Infrastructure.DataAccess;
using System.Text;
namespace Pastbin.Infrastructure.Services
{
    public class PostService : IPostService
    {

        private readonly IFileService _fileService;
        private readonly PastbinDbContext _db;
        public PostService(IFileService fileService, PastbinDbContext db)
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
                // Использование MemoryStream для загрузки файла
                var response = await _fileService.UploadFileAsync("shokir-demo-bucket", memoryStream, "file.txt", entity.ExpireHour, null);

                // Обновление сущности
                entity.UrlAWS = response.UploadedFilePath;
                entity.HashUrl = string.Join("", HashGenerator.sha256_hash(response.UploadedFilePath).Select(x => x).Take(40));
            }
            await _db.Posts.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Post>> GetAllFromUsernameAsync(string Username)
        {
            var User = await _db.Users.FirstOrDefaultAsync(x => x.Username == Username);
            if (User == null) return new List<Post>();
            return User.Posts;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            var Posts = _db.Posts.ToList();
            return Posts;
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            return await _db.Posts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<Post> UpdateAsync(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
