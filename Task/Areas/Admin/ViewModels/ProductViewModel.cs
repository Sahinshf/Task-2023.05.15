using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;


namespace Task.Areas.Admin.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Rating { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; } 
    }
}
