using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Constants;
using ProductAPI.DTOs.Product;
using ProductAPI.Models;
using ProductAPI.Repositories;

namespace ProductAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductAPIController : ControllerBase
{
    private readonly ILogger<ProductAPIController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductAPIController(IProductRepository repository,
        IMapper mapper,
        ILogger<ProductAPIController> logger)
    {
        _logger = logger;
        _mapper = mapper;
        _productRepository = repository;
    }


    [HttpPost("/CreateProduct")]
    public async Task<IActionResult> CreateProduct([FromBody,Required] CreateProductRequest request)
    {
        if (request == null)
        {
            _logger.LogInformation("Invalid request.");
            return new ObjectResult(ProductMessage.NullRequest)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        Product product = _mapper.Map<Product>(request);
        if (product == null)
        {
            _logger.LogInformation("Product is null.");
            return new ObjectResult(ProductMessage.NullRequest)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        var productExists = await _productRepository.ProductExistsAsync(product);
        if (productExists.IsSuccess)
        {
            _logger.LogInformation(ProductMessage.ProductDuplicate);
            return new ObjectResult(ProductMessage.ProductDuplicate)
            {
                StatusCode = StatusCodes.Status400BadRequest,
            };
        }

        var result = await _productRepository.InsertProductAsync(product);
        if (result.IsFailed)
        {
            _logger.LogWarning(result.Reasons.First().ToString());
            return new ObjectResult(result.Reasons.First().ToString())
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }

        _logger.LogInformation($"Product ID:{result.Value} created.");
        return Ok();
    }

    [HttpGet("/GetProducts")]
    public async Task<IActionResult> RetrieveProducts(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogInformation("Invalid request");
            return new ObjectResult(ProductMessage.PageNumberAndSizeRequired)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        var result = await _productRepository.GetProductsWithOffsetPaginationAsync(pageNumber,pageSize);
        if (result.IsFailed)
        {
            _logger.LogInformation(result.Reasons.First().ToString());
            return new ObjectResult(result.Reasons.First().ToString())
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }
        
        return Ok(result.Value);
    }

    [HttpGet("/GetProductById/{id}")]
    public async Task<IActionResult> RetrieveProductById([FromRoute] int id)
    {
        var result = await _productRepository.GetProductByIdAsync(id);
        if (result.IsFailed)
        {
            _logger.LogInformation(result.Reasons.First().ToString());
            return new ObjectResult(result.Reasons.First().ToString())
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }

        return Ok(result.Value);
    }

    [HttpPatch("/UpdateProduct/{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody, Required] UpdateProductRequest request)
    {
        if (request == null)
        {
            _logger.LogInformation("Invalid request.");
            return new ObjectResult(ProductMessage.NullRequest)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        var result = await _productRepository.GetProductByIdAsync(id);
        if (result.IsFailed)
        {
            _logger.LogInformation(result.Reasons.First().ToString());
            return new ObjectResult(result.Reasons.First().ToString())
            {
                StatusCode = StatusCodes.Status404NotFound,
            };
        }

        Product product = _mapper.Map<Product>(request);
        if (product == null)
        {
            _logger.LogInformation("Mapping error.");
            return new ObjectResult(ProductMessage.NullRequest)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        var productExists = await _productRepository.ProductExistsAsync(product);
        if (productExists.IsSuccess && productExists.Value != product.Id)
        {
            _logger.LogInformation(ProductMessage.ProductDuplicate);
            return new ObjectResult(ProductMessage.ProductDuplicate)
            {
                StatusCode = StatusCodes.Status400BadRequest,
            };
        }

        var updateResult = await _productRepository.UpdateProductAsync(product);
        if (updateResult.IsFailed)
        {
            _logger.LogInformation(updateResult.Reasons.First().ToString());
            return new ObjectResult(updateResult.Reasons.First().ToString())
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }

        _logger.LogInformation($"Product ID: {request.Id} was updated.");
        return Ok();
    }

    [HttpDelete("/DeleteProduct/{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        var findResult = await _productRepository.GetProductByIdAsync(id);
        if (findResult.IsFailed)
        {
            _logger.LogInformation(findResult.Reasons.First().ToString());
            return new ObjectResult(findResult.Reasons.First().ToString())
            {
                StatusCode = StatusCodes.Status404NotFound,
            };
        }

        var result = await _productRepository.DeleteProductAsync(id);
        if (result.IsFailed)
        {
            _logger.LogInformation(result.Reasons.First().ToString());
            return new ObjectResult(result.Reasons.First().ToString())
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }

        _logger.LogInformation($"Product ID: {id} was deleted.");
        return StatusCode(StatusCodes.Status200OK);
    }
}

