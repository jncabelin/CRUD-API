using System;
using System.ComponentModel.DataAnnotations;

namespace ProductAPI.DTOs.Product
{
	public class CreateProductRequest
	{
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
    }
}

