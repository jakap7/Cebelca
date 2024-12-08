using System.ComponentModel.DataAnnotations;

namespace Cebelica.Models
{
    public class ProductsModel
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public bool IsActive { get; set; }
        public string? PicturePath { get; set; }
    }
}
