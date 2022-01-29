using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mixed.Models
{
    public class Collection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public string Theme { get; set; }
        public int CountItems { get; set; }
    }
}
