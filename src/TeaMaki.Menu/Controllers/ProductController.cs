using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections;
using TeaMaki.Persistence;
using TeaMaki.Menu.ProductAggregate;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace TeaMaki.Menu.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class ProductController : ControllerBase
    {

        private readonly IRepository<Product> _repository;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(IRepository<Product> repository, 
            ILogger<ProductController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get(string productId)
        {

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

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertOrUpdate(ProductToPut productToPut)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productToPut);

            try
            {
                _repository.InsertOrUpdate(product);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return StatusCode(500);
            }

            if (string.IsNullOrWhiteSpace(productToPut.ProductId))
            {
                return CreatedAtAction(nameof(Get), new { productId = product.Id });
            }

            return Ok();
        }

    }


}
