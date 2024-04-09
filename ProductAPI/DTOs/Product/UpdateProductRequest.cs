﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ProductAPI.DTOs.Product
{
	public class UpdateProductRequest
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
    }
}

