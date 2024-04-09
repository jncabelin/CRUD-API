using System;
using ProductAPI.Models;

namespace ProductAPI.Tests.ProductAPI.UnitTests.TestData
{
	public static class TestProducts
	{
		public static Product TestProducts_ProductA = new Product
		{
			Id = 1,
			Name = "TestA",
			Brand = "BrandA",
			Price = new Decimal(50.0)
		};

        public static Product TestProducts_ProductB = new Product
        {
            Id = 2,
            Name = "TestB",
            Brand = "BrandB",
            Price = new Decimal(50.0)
        };

        public static List<Product> TestProducts_ProductsList = new List<Product>
		{
			TestProducts_ProductA,
			TestProducts_ProductB
		};
	}
}

