using CartPaymentAPI;
using CartPaymentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CartAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartPaymentController : ControllerBase
    {
        private readonly ILogger<CartPaymentController> _logger;
        private readonly CartPaymentRepository _cartPaymentRepository;

        public CartPaymentController(ILogger<CartPaymentController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _cartPaymentRepository = new CartPaymentRepository(configuration);
        }

        // Cart Endpoints

        [HttpPost("createcart/{userId}", Name = "CreateCart")]
        public async Task<IActionResult> CreateCartAsync(string userId)
        {
            var createdCart = await _cartPaymentRepository.CreateCartAsync(userId);
            return CreatedAtRoute("GetCartById", new { cartId = createdCart.Id }, createdCart);
        }

        [HttpGet("cart/{cartId}", Name = "GetCartById")]
        public async Task<ActionResult<Cart>> GetCartByIdAsync(string cartId)
        {
            var cart = await _cartPaymentRepository.GetCartByIdAsync(cartId);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        [HttpGet("cart/user/{userId}", Name = "GetCartByUserId")]
        public async Task<ActionResult<Cart>> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartPaymentRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        [HttpPut("carts/edit/{cartId}", Name = "EditCartById")]
        public async Task<IActionResult> EditCartAsync(string cartId, [FromBody] Cart updatedCart)
        {
            var success = await _cartPaymentRepository.EditCartAsync(cartId, updatedCart);

            if (success)
            {
                return Ok("Cart updated successfully");
            }
            else
            {
                return NotFound("Cart not found");
            }
        }

        [HttpPost("cart/{cartId}/additem", Name = "AddCartItem")]
        public async Task<IActionResult> AddCartItemAsync(string cartId, [FromBody] InventoryItem cartItem)
        {
            await _cartPaymentRepository.AddCartItemAsync(cartId, cartItem);
            return Ok();
        }

        [HttpDelete("cart/{cartId}/removeitem/{cartItemId}", Name = "RemoveCartItemById")]
        public async Task<IActionResult> RemoveCartItemAsync(string cartId, string cartItemId)
        {
            await _cartPaymentRepository.RemoveCartItemAsync(cartId, cartItemId);
            return Ok();
        }

        [HttpDelete("cart/{cartId}", Name = "DeleteCart")]
        public async Task<IActionResult> DeleteCartAsync(string cartId)
        {
            await _cartPaymentRepository.DeleteCartByIdAsync(cartId);
            return Ok();
        }

        // Payment Endpoints

        [HttpPost("payment/add", Name = "AddPayment")]
        public async Task<IActionResult> AddPaymentAsync([FromBody] Payment payment)
        {
            try
            {
                // Set necessary properties like PaymentDate, PaymentId, etc.
                payment.PaymentDate = DateTime.Now;
                payment.Id = Guid.NewGuid().ToString();

                await _cartPaymentRepository.AddPaymentAsync(payment);

                // Optionally, you can save the cart, reset it, or perform other actions here

                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("payment/remove/{paymentId}", Name = "DeletePaymentById")]
        public async Task<IActionResult> DeletePaymentByIdAsync(string paymentId)
        {
            await _cartPaymentRepository.DeletePaymentByIdAsync(paymentId);
            return Ok();
        }
    }
}
