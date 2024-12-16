namespace uniqilo.Models
{
    public class Comment:BaseEntity
    {
        public string Content { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
