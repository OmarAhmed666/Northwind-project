using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind_ecomm.Data;
using Northwind_ecomm.DTOs;
using Northwind_ecomm.Models;

namespace Northwind_ecomm.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly NorthwindContext _context;

		public OrdersController(NorthwindContext context)
		{
			_context = context;
		}

		// GET api/orders/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetOrder(int id)
		{
			// Load the order AND its items in one query
			var order = await _context.Orders
				.Include(o => o.OrderDetails)
				.FirstOrDefaultAsync(o => o.OrderId == id);

			if (order == null)
				return NotFound();

			// Copy from the database entity into the response DTO
			var response = new OrderResponseDto();
			response.OrderId = order.OrderId;
			response.CustomerId = order.CustomerId;
			response.OrderDate = order.OrderDate;

			foreach (var detail in order.OrderDetails)
			{
				var item = new OrderItemResponseDto();
				item.ProductId = detail.ProductId;
				item.UnitPrice = detail.UnitPrice;
				item.Quantity = detail.Quantity;
				response.Items.Add(item);
			}

			return Ok(response);
		}

		// POST api/orders
		[HttpPost]
		public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
		{
			// 1. Validate the request
			if (dto.Items.Count == 0)
				return BadRequest("The order must have at least one item.");

			var customer = await _context.Customers.FindAsync(dto.CustomerId);
			if (customer == null)
				return NotFound("Customer not found.");

			// 2. Build the order (the server decides the date, not the client)
			var order = new Order();
			order.CustomerId = dto.CustomerId;
			order.OrderDate = DateTime.Now;

			// 3. Build each order item
			foreach (var item in dto.Items)
			{
				if (item.Quantity <= 0)
					return BadRequest("Quantity must be greater than 0.");

				var product = await _context.Products.FindAsync(item.ProductId);
				if (product == null)
					return NotFound("Product " + item.ProductId + " not found.");

				var detail = new OrderDetail();
				detail.ProductId = item.ProductId;
				detail.Quantity = item.Quantity;
				detail.UnitPrice = product.UnitPrice ?? 0; // price comes from the database, not the client
				detail.Discount = 0;

				order.OrderDetails.Add(detail);
			}

			// 4. Save the order and all its items together
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();

			// 5. Return 201 Created + the address of the GET endpoint
			return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, new { orderId = order.OrderId });
		}
	}
}