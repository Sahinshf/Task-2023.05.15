using Task.Models.Common;


namespace Task.Models
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double  Price { get; set; }
        public int Rating { get; set; } 
        public string Image { get; set; }
        public int CategoryId { get; set; } // Name must be like that
        public Category Category { get; set; } //Foregin key
        public bool IsDeleted { get; set; }

        
    }
}
