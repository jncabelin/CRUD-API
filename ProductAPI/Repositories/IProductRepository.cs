using ProductAPI.Models;
using FluentResults;
using ProductAPI.Pagination;

namespace ProductAPI.Repositories
{
    public interface IProductRepository
    {
        public Task<Result<PagedResponse<Product>>> GetProductsWithOffsetPaginationAsync(int pageNumber, int pageSize);
        public Task<Result<Product>> GetProductByIdAsync(int Id);
        public Task<Result<int>> InsertProductAsync(Product product);
        public Task<Result> UpdateProductAsync(Product product);
        public Task<Result> DeleteProductAsync(int Id);
        public Task<Result<int>> ProductExistsAsync(Product product);
        public Task<Result> SaveChangesAsync();
    }
}

