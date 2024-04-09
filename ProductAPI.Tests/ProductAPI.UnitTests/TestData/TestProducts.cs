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
	}
}

