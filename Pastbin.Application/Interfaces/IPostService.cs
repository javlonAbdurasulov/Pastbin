﻿using Pastbin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pastbin.Application.Interfaces
{
    public interface IPostService
    {
        Task<Post> CreateAsync(Post entity,string text);
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post> GetByIdAsync();
        Task<Post> UpdateAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
