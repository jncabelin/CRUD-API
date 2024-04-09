using System;
namespace ProductAPI.Constants
{
    public static class ProductMessage
    {
        public const string IdIsRequired = "Id is required";
        public const string NameIsRequired = "Name is required";
        public const string NameLength = "Name must be between 3 and 50 characters";
        public const string BrandIsRequired = "Brand is required";
        public const string BrandLength = "Brand must be between 3 and 20 characters";
        public const string PriceIsRequired = "Price is required";
        public const string PriceGreaterThan = "Price must be greater than 0";
        public const string NullRequest = "Request is null";
        public const string PageNumberAndSizeRequired = "Page Number and Page Size must be greater than 0";
        public const string ProductNotFoundById = "Product not found by Id";
        public const string ProductDuplicate = "Error: product is a duplicate.";
    }
}

