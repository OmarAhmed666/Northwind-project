using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind_ecomm.Data;
using Northwind_ecomm.DTOs;
using Northwind_ecomm.Models;

namespace Northwind_ecomm.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly NorthwindContext _context;

		public ProductsController(NorthwindContext context)
		{
			_context = context;
		}
		// GET /api/Product
		[HttpGet]
		public async Task<IActionResult> GetProducts()
		{
			// AsNoTracking: we only read, so EF doesn't need to track changes
			var products = await _context.Products.AsNoTracking().ToListAsync();

			var result = new List<ProductResponseDto>();
			foreach (var product in products)
			{
				result.Add(ToResponse(product));
			}

			return Ok(result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetProduct(int id)
		{
			var product = await _context.Products.FindAsync(id);

			if (product == null)
				return NotFound();

			return Ok(ToResponse(product));
		}

		[HttpPost]
		public async Task<IActionResult> CreateProduct(CreateProductDto dto)
		{
			// 1. Validate the request
			// (a null value is not less than 0, so these only fail for negative numbers)
			if (dto.UnitPrice < 0)
				return BadRequest("Unit price cannot be negative.");

			if (dto.UnitsInStock < 0)
				return BadRequest("Units in stock cannot be negative.");

			// The category and supplier are optional, but if sent they must exist
			if (dto.CategoryId != null)
			{
				var category = await _context.Categories.FindAsync(dto.CategoryId.Value);
				if (category == null)
					return NotFound("Category not found.");
			}

			if (dto.SupplierId != null)
			{
				var supplier = await _context.Suppliers.FindAsync(dto.SupplierId.Value);
				if (supplier == null)
					return NotFound("Supplier not found.");
			}

			// 2. Build the product (copy field by field from the DTO)
			var product = new Product();
			product.ProductName = dto.ProductName;
			product.SupplierId = dto.SupplierId;
			product.CategoryId = dto.CategoryId;
			product.QuantityPerUnit = dto.QuantityPerUnit;
			product.UnitPrice = dto.UnitPrice;
			product.UnitsInStock = dto.UnitsInStock;

			// The server decides these, not the client
			product.UnitsOnOrder = 0;
			product.ReorderLevel = 0;
			product.Discontinued = false;

			// 3. Save
			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			// 4. Return 201 Created + the address of the GET endpoint
			//return CreatedAtAction(nameof(Product), new { id = product.ProductId }, ToResponse(product));
			return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, ToResponse(product));
		}

		// Copies a Product (database entity) into a ProductResponseDto
		private ProductResponseDto ToResponse(Product product)
		{
			var response = new ProductResponseDto();
			response.ProductId = product.ProductId;
			response.ProductName = product.ProductName;
			response.SupplierId = product.SupplierId;
			response.CategoryId = product.CategoryId;
			response.QuantityPerUnit = product.QuantityPerUnit;
			response.UnitPrice = product.UnitPrice;
			response.UnitsInStock = product.UnitsInStock;
			response.Discontinued = product.Discontinued;
			return response;
		}

	}
}
