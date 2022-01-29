using System;

namespace Mixed.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string ItemId { get; set; }
        public string Content { get; set; }
        public string messenge { get; set; }
        public string UserId { get; set; }
        public string UrlImg { get; set; }
    }
}
