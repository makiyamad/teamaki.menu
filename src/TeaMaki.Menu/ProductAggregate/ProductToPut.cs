using System.ComponentModel.DataAnnotations;

namespace TeaMaki.Menu.ProductAggregate
{
    public class ProductToPut
    {
        public string ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal Tax { get; set; }
        public string ImagePath { get; set; }
    }


}
