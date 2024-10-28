using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pastbin.Domain.Models.DTO
{
    public class PostDeleteDTO
    {
        public string hashUrl { get; set; }
        public string username { get; set; }
    }
}
