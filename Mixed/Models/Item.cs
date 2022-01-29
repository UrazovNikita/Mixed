using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mixed.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public string CollectionId { get; set; }
        public DateTime AddTime { get; set; }
        public int LikeCount { get; set; }       
    }
}
