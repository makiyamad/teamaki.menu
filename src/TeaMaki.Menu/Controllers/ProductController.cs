using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections;
using TeaMaki.Persistence;

namespace TeaMaki.Menu.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class ProductController : ControllerBase{

        private readonly IRepository<Product> _repository;

        public ProductController(IRepository<Product> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get(string productId){

            var product = _repository.Get(productId);


            var productToGet = new ProductToGet()
            {
                ImagePath = product.ImagePath,
                Name = product.Name,
                Price = product.Price,
                ProductId = product.Id,
                Tax = product.Tax
            };
            
            return Ok(productToGet);
        }
    }

    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public string ImagePath { get; set; }
    }


}
