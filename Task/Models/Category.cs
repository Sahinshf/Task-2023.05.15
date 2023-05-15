using Task.Models.Common; 

namespace Task.Models
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public bool IsDeleted { get; set; }
        public ICollection<Product> Products { get; set;} // Bir category`nin birdən çox product`ı var. Add-migration etməyə ehtiyac yoxdur. Bu bizə kod tərəfdə lazım olur

    }
}
