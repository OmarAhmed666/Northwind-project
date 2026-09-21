namespace Northwind_ecomm.DTOs
{
	// What the client sends for ONE item in the order
	public class CreateOrderItemDto
	{
		public int ProductId { get; set; }
		public short Quantity { get; set; }
	}

	// What the client sends to create an order
	public class CreateOrderDto
	{
		public string CustomerId { get; set; } = "";
		public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
	}

	// What we return for ONE item of an order
	public class OrderItemResponseDto
	{
		public int ProductId { get; set; }
		public decimal UnitPrice { get; set; }
		public short Quantity { get; set; }
	}

	// What we return for an order
	public class OrderResponseDto
	{
		public int OrderId { get; set; }
		public string? CustomerId { get; set; }
		public DateTime? OrderDate { get; set; }
		public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
	}
}