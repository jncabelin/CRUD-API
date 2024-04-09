# CRUD-API

Simple .NET Core WebAPI Project involving CRUD Operations. This project includes FluentResult Result Pattern, FluentValidation Input Validation, Xunit/Moq Unit testing, and ILogger. Microsoft SQL Server is used for the database.

Swagger Endpoint: localhost:{assigned_port}/swagger/index.html

Endpoints:
- CreateProduct
> used to create product in the database. Combination of Name and Brand must be unique.
> CreateProductRequest { Name (string), Brand (string), Price (decimal) }

- GetProducts?pageNumber={pageNumber}&pageSize={pageSize}
> retrieves all products in the database. PageNumber and PageSize are required.

- GetProductById/{id}
> retrieves the product with the corresponding product id

- UpdateProduct/{id}
> updates the product with corresponding id
> UpdateProductRequest { Id (string), Name (string), Brand (string), Price (decimal) }

- DeleteProduct/{id}
> removes the product with the corresponding product id




