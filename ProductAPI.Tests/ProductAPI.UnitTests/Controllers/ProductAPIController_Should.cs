using Moq;
using ProductAPI.Repositories;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using FluentResults;
using ProductAPI.Tests.ProductAPI.UnitTests.TestData;
using AutoMapper;
using ProductAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using ProductAPI.Models;
using ProductAPI.DTOs.Product;
using ProductAPI.Pagination;
using Xunit;

namespace ProductAPI.Tests.ProductAPI.UnitTests.Repositories
{
	public class ProductAPIController_Should
	{
        Mock<ILogger<ProductAPIController>> _logger;
        Mock<IProductRepository> _productRepository;
        Mock<IMapper> _mapper;

        public ProductAPIController_Should()
        {
            _logger = new Mock<ILogger<ProductAPIController>>();
            _productRepository = new Mock<IProductRepository>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        [DisplayName("Fail_CreateProduct_NullRequest")]
        public async void Fail_CreateProduct_NullRequest()
        {
            // Arrange
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);

            // Act
            var result = await sut.CreateProduct(null);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_CreateProduct_MapError")]
        public async void Fail_CreateProduct_MapError()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _mapper.Setup(c => c.Map<Product>(It.IsAny<CreateProductRequest>())).Returns<Product>(null);
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);
            var request = new CreateProductRequest { Name = "name", Brand = "brand", Price = new Decimal(10.0) };

            // Act
            var result = await sut.CreateProduct(request);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_CreateProduct_Duplicate")]
        public async void Fail_CreateProduct_Duplicate()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _mapper.Setup(c => c.Map<Product>(It.IsAny<CreateProductRequest>())).Returns(TestProducts.TestProducts_ProductA);
            _productRepository.Setup(c => c.ProductExistsAsync(It.IsAny<Product>())).ReturnsAsync(Result.Ok(10));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);
            var request = new CreateProductRequest { Name = "name", Brand = "brand", Price = new Decimal(10.0) };

            // Act
            var result = await sut.CreateProduct(request);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_CreateProduct_DbFail")]
        public async void Fail_CreateProduct_DbFail()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _mapper.Setup(c => c.Map<Product>(It.IsAny<CreateProductRequest>())).Returns(TestProducts.TestProducts_ProductA);
            _productRepository.Setup(c => c.ProductExistsAsync(It.IsAny<Product>())).ReturnsAsync(Result.Fail("Product exists."));
            _productRepository.Setup(c => c.InsertProductAsync(It.IsAny<Product>())).ReturnsAsync(Result.Fail("Error on update."));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);
            var request = new CreateProductRequest { Name = "name", Brand = "brand", Price = new Decimal(10.0) };

            // Act
            var result = await sut.CreateProduct(request);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Succeed_CreateProduct")]
        public async void Succeed_CreateProduct()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _mapper.Setup(c => c.Map<Product>(It.IsAny<CreateProductRequest>())).Returns(TestProducts.TestProducts_ProductA);
            _productRepository.Setup(c => c.ProductExistsAsync(It.IsAny<Product>())).ReturnsAsync(Result.Fail("Product exists."));
            _productRepository.Setup(c => c.InsertProductAsync(It.IsAny<Product>())).ReturnsAsync(Result.Ok());
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);
            var request = new CreateProductRequest { Name = "name", Brand = "brand", Price = new Decimal(10.0) };

            // Act
            var result = await sut.CreateProduct(request);
            var objResult = result as OkResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status200OK, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_RetrieveProductById")]
        public async void Fail_RetrieveProductById()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Fail("Product does not exist."));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);

            // Act
            var result = await sut.RetrieveProductById(1);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Succeed_RetrieveProductById")]
        public async void Succeed_RetrieveProductById()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);

            // Act
            var result = await sut.RetrieveProductById(1);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status200OK, objResult.StatusCode);
            Assert.Equal(TestProducts.TestProducts_ProductA, objResult.Value);
        }

        [Fact]
        [DisplayName("Fail_RetrieveProducts_InvalidRequest")]
        public async void Fail_RetrieveProducts_InvalidRequest()
        {
            // Arrange
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);

            // Act
            var result = await sut.RetrieveProducts(-1, 10);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_RetrieveProducts_dbError")]
        public async void Fail_RetrieveProducts_dbError()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductsWithOffsetPaginationAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(Result.Fail("Product does not exist."));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);

            // Act
            var result = await sut.RetrieveProducts(1, 10);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_UpdateProduct_NullRequest")]
        public async void Fail_UpdateProduct_NullRequest()
        {
            // Arrange
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);

            // Act
            var result = await sut.UpdateProduct(null);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_UpdateProduct_InvalidId")]
        public async void Fail_UpdateProduct_InvalidId()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Fail("Product ID not found."));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);
            var request = new UpdateProductRequest {Id = 1, Name = "name", Brand = "brand", Price = new Decimal(10.0)};
            
            // Act
            var result = await sut.UpdateProduct(request);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status404NotFound, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_UpdateProduct_MapError")]
        public async void Fail_UpdateProduct_MapError()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _mapper.Setup(c => c.Map<Product>(It.IsAny<UpdateProductRequest>())).Returns<Product>(null);
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);
            var request = new UpdateProductRequest { Id = 1, Name = "name", Brand = "brand", Price = new Decimal(10.0) };

            // Act
            var result = await sut.UpdateProduct(request);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_UpdateProduct_Duplicate")]
        public async void Fail_UpdateProduct_Duplicate()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _mapper.Setup(c => c.Map<Product>(It.IsAny<UpdateProductRequest>())).Returns(TestProducts.TestProducts_ProductA);
            _productRepository.Setup(c => c.ProductExistsAsync(It.IsAny<Product>())).ReturnsAsync(Result.Ok(10));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);
            var request = new UpdateProductRequest { Id = 1, Name = "name", Brand = "brand", Price = new Decimal(10.0) };

            // Act
            var result = await sut.UpdateProduct(request);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_UpdateProduct_DbFail")]
        public async void Fail_UpdateProduct_DbFail()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _mapper.Setup(c => c.Map<Product>(It.IsAny<UpdateProductRequest>())).Returns(TestProducts.TestProducts_ProductA);
            _productRepository.Setup(c => c.ProductExistsAsync(It.IsAny<Product>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA.Id));
            _productRepository.Setup(c => c.UpdateProductAsync(It.IsAny<Product>())).ReturnsAsync(Result.Fail("Error on update."));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);
            var request = new UpdateProductRequest { Id = 1, Name = "name", Brand = "brand", Price = new Decimal(10.0) };

            // Act
            var result = await sut.UpdateProduct(request);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Succeed_UpdateProduct")]
        public async void Succeed_UpdateProduct()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _mapper.Setup(c => c.Map<Product>(It.IsAny<UpdateProductRequest>())).Returns(TestProducts.TestProducts_ProductA);
            _productRepository.Setup(c => c.ProductExistsAsync(It.IsAny<Product>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA.Id));
            _productRepository.Setup(c => c.UpdateProductAsync(It.IsAny<Product>())).ReturnsAsync(Result.Ok());
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);
            var request = new UpdateProductRequest { Id = 1, Name = "name", Brand = "brand", Price = new Decimal(10.0) };

            // Act
            var result = await sut.UpdateProduct(request);
            var objResult = result as OkResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status200OK, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_DeleteProductById_InvalidId")]
        public async void Fail_DeleteProductById_InvalidId()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Fail("Product not found."));
            _productRepository.Setup(c => c.DeleteProductAsync(It.IsAny<int>())).ReturnsAsync(Result.Fail("Delete failed."));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);

            // Act
            var result = await sut.DeleteProduct(1);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status404NotFound, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Fail_DeleteProductById")]
        public async void Fail_DeleteProductById()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _productRepository.Setup(c => c.DeleteProductAsync(It.IsAny<int>())).ReturnsAsync(Result.Fail("Delete failed."));
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);

            // Act
            var result = await sut.DeleteProduct(1);
            var objResult = result as ObjectResult;

            // Assert
            Assert.NotNull(objResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, objResult.StatusCode);
        }

        [Fact]
        [DisplayName("Succeed_DeleteProductById")]
        public async void Succeed_DeleteProductById()
        {
            // Arrange
            _productRepository.Setup(c => c.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok(TestProducts.TestProducts_ProductA));
            _productRepository.Setup(c => c.DeleteProductAsync(It.IsAny<int>())).ReturnsAsync(Result.Ok());
            var sut = new ProductAPIController(_productRepository.Object, _mapper.Object, _logger.Object);

            // Act
            var result = await sut.DeleteProduct(1);
            var codeResult = result as StatusCodeResult;

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status200OK, codeResult.StatusCode);
        }
    }
}

