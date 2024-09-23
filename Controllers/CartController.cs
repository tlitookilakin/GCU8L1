using CartAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartAPI.Controllers
{
	[Controller]
	[Route("cart-items")]
	public class CartController : ControllerBase
	{
		private int nextId = 1;
		private static readonly List<CartItem> items = [];

		[HttpGet]
		public IActionResult GetAll(int maxPrice = -1, string? prefix = null, int pageSize = 0, int page = 0)
		{
			IEnumerable<CartItem> filtered = items;

			if (maxPrice >= 0)
				filtered = filtered.Where(x => x.Price <= maxPrice);

			if (prefix is not null)
				filtered = filtered.Where(x => x.Product.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

			if (pageSize > 0)
				filtered = filtered.Take((page * pageSize)..((page + 1) * pageSize));

			return Ok(filtered);
		}
		// curl http://localhost:5131/cart-items
		// curl http://localhost:5131/cart-items?prefix=so

		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			if (items.FirstOrDefault(x => x.Id == id) is CartItem item)
				return Ok(item);
			return NotFound();
		}
		// curl http://localhost:5131/cart-items/1

		[HttpPost]
		public IActionResult AddCartItem([FromBody] CartItem item)
		{
			item.Id = nextId++;
			items.Add(item);
			return Created($"cart-items/{item.Id}", item);
		}
		// curl --header "Content-Type: application/json" --data "{\"Product\": \"Soap\", \"Price\": 1, \"Quantity\": 100}" http://localhost:5131/cart-items

		[HttpPut("{id}")]
		public IActionResult UpdateCartItem([FromBody] CartItem item, int id)
		{
			if (item.Id != id)
				return BadRequest();

			int index = items.FindIndex(x => x.Id == id);
			if (index == -1)
				return NotFound();

			items[index] = item;
			return Ok(item);
		}
		// curl --header "Content-Type: application/json" --request PUT --data "{\"Product\": \"Soap\", \"Price\": 1, \"Quantity\": 10, \"Id\": 1}" http://localhost:5131/cart-items/1

		[HttpDelete("{id}")]
		public IActionResult RemoveCartItem(int id)
		{
			int index = items.FindIndex(x => x.Id == id);
			if (index == -1)
				return NotFound();

			items.RemoveAt(index);
			return NoContent();
		}
		// curl --request DELETE http://localhost:5131/cart-items/1
	}
}
