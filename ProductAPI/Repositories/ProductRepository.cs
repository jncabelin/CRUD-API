using FluentResults;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;
using ProductAPI.Pagination;

namespace ProductAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private ApplicationDbContext _dbContext;
        private ILogger<ProductRepository> _logger;

        public ProductRepository(ApplicationDbContext context,ILogger<ProductRepository> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public async Task<Result> DeleteProductAsync(int productId)
        {
            try
            {
                var result = await _dbContext.ProductTable.FirstOrDefaultAsync(x => x.Id == productId);
                if (result == null)
                    return Result.Fail("Product not found.");

                _dbContext.ProductTable.Remove(result);

                var saveResult = await SaveChangesAsync();
                if (saveResult.IsFailed)
                    return Result.Fail(saveResult.Reasons.First().ToString());

                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result<Product>> GetProductByIdAsync(int productId)
        {
            try
            {
                var result = await _dbContext.ProductTable.AsNoTracking().FirstOrDefaultAsync(x => x.Id == productId);
                if (result == null)
                    return Result.Fail("Product not found.");

                return Result.Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result<PagedResponse<Product>>> GetProductsWithOffsetPaginationAsync(int pageNumber, int pageSize)
        {
            try
            {
                var totalRecords = await _dbContext.ProductTable.AsNoTracking().CountAsync();

                var products = await _dbContext.ProductTable.AsNoTracking()
                                .OrderBy(x => x.Id)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
                var pagedResponse = new PagedResponse<Product>(products, pageNumber, pageSize, totalRecords);

                return Result.Ok<PagedResponse<Product>>(pagedResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result<int>> InsertProductAsync(Product product)
        {
            try
            {
                var result = await _dbContext.ProductTable.AddAsync(product);

                var saveResult = await SaveChangesAsync();
                if (saveResult.IsFailed)
                    return Result.Fail(saveResult.Reasons.First().ToString());

                return Result.Ok(product.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result> UpdateProductAsync(Product product)
        {
            try
            {
                _dbContext.ProductTable.Update(product);

                var saveResult = await SaveChangesAsync();
                if (saveResult.IsFailed)
                    return Result.Fail(saveResult.Reasons.First().ToString());

                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result<int>> ProductExistsAsync(Product product)
        {
            try
            {
                // Check for duplicates
                var findResult = await _dbContext.ProductTable.AsNoTracking().FirstOrDefaultAsync(x => x.Name == product.Name && x.Brand == product.Brand);
                if (findResult == null)
                    return Result.Fail("Product not found.");

                return Result.Ok(findResult.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result> SaveChangesAsync()
        {
            try
            {
                var result = await _dbContext.SaveChangesAsync();
                if (result < 0)
                    Result.Fail("Changes not saved.");
                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Result.Fail(e.Message);
            }
        }
    }
}

