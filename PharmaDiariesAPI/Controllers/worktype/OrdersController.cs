using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PharmaDiaries.DataAccess;
using PharmaDiaries.Models;
using System;
using System.Collections.Generic;

namespace PharmaDiariesAPI.Controllers.worktype
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersRepository _repository;

        public OrdersController(IOrdersRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Save a new order (POB - Personal Order Booking)
        /// </summary>
        [HttpPost("Save")]
        public IActionResult Save([FromBody] OrderModel order)
        {
            try
            {
                var result = _repository.Save(order);
                return Ok(new { success = result, message = result ? "Order saved successfully" : "Failed to save order" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save multiple orders for a transaction
        /// </summary>
        [HttpPost("SaveMultiple")]
        public IActionResult SaveMultiple([FromBody] List<OrderModel> orders)
        {
            try
            {
                int savedCount = 0;
                foreach (var order in orders)
                {
                    if (_repository.Save(order))
                    {
                        savedCount++;
                    }
                }
                return Ok(new
                {
                    success = savedCount == orders.Count,
                    message = $"{savedCount} of {orders.Count} orders saved successfully",
                    savedCount = savedCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing order
        /// </summary>
        [HttpPost("Update")]
        public IActionResult Update([FromBody] OrderModel order)
        {
            try
            {
                var result = _repository.Update(order);
                return Ok(new { success = result, message = result ? "Order updated successfully" : "Failed to update order" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an order (soft delete)
        /// </summary>
        [HttpPost("Delete")]
        public IActionResult Delete([FromQuery] int orderId, [FromQuery] int modifiedBy)
        {
            try
            {
                var result = _repository.Delete(orderId, modifiedBy);
                return Ok(new { success = result, message = result ? "Order deleted successfully" : "Failed to delete order" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get orders by TransID (for a specific DCR entry)
        /// </summary>
        [HttpGet("GetByTransID")]
        public IActionResult GetByTransID([FromQuery] string transId)
        {
            try
            {
                var orders = _repository.GetByTransID(transId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get orders by company with optional date range filter
        /// </summary>
        [HttpGet("GetByCompany")]
        public IActionResult GetByCompany([FromQuery] int compId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var orders = _repository.GetByCompany(compId, fromDate, toDate);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
