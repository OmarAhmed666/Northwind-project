using System.ComponentModel.DataAnnotations;

namespace Northwind_ecomm.DTOs
{
	public class CreateProductDto
	{
		[Required]
		public string ProductName { get; set; } = "";
		public int? SupplierId { get; set; }
		public int? CategoryId { get; set; }
		public string? QuantityPerUnit { get; set; }
		public decimal? UnitPrice { get; set; }
		public short? UnitsInStock { get; set; }
	}
}