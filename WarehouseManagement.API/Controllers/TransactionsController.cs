using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.WarehouseTransactions;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Shared;

namespace WarehouseManagement.API.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController(IWarehouseTransactionService service) : ControllerBase
    {
        private readonly IWarehouseTransactionService _service = service;

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction(CreateWarehouseTransactionDto dto)
        {
            var create = await _service.CreateTransactionAsync(dto);
            if (create != null)
            {
                return Ok(new ApiResponse<WarehouseTransactionDto>(200, "Create Transaction Successfully", create));
            }
            return BadRequest(new ApiResponse<WarehouseTransactionDto>(400, "Create Transaction Failed"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromServices] ICurrentUserService currentUserService)
        {
            var approve = await _service.ApproveTransactionAsync(id, currentUserService);
            if (approve != null)
            {
                return Ok(new ApiResponse<WarehouseTransactionDto>(200, "Approve Transaction Successfully", approve));
            }
            return BadRequest(new ApiResponse<WarehouseTransactionDto>(400, "Approve Transaction Failed"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] string reason)
        {
            var reject = await _service.RejectTransactionAsync(id, reason);
            if (reject != null)
            {
                return Ok(new ApiResponse<WarehouseTransactionDto>(200, "Reject Transaction Successfully", reject));
            }
            return BadRequest(new ApiResponse<WarehouseTransactionDto>(400, "Reject Transaction Failed"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransaction()
        {
            var transactions = await _service.GetAllTransactionsAsync();
            return Ok(new ApiResponse<IEnumerable<WarehouseTransactionDto>>(200, "Get Transactions Successfully", transactions));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdTransaction(Guid id)
        {
            var transaction = await _service.GetTransactionByIdAsync(id);
            if (transaction != null)
            {
                return Ok(new ApiResponse<WarehouseTransactionDto>(200, "Get Transactions with Id Successfully", transaction));
            }
            return NotFound(new ApiResponse<WarehouseTransactionDto>(404, "Transaction with ID does not exist"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateWarehouseTransactionDto dto, [FromServices] ICurrentUserService currentUserService)
        {
            var update = await _service.UpdateTransactionAsync(id, dto, currentUserService);
            if (update != null)
            {
                return Ok(new ApiResponse<WarehouseTransactionDto>(200, "Update Transaction Successfully", update));
            }
            return BadRequest(new ApiResponse<WarehouseTransactionDto>(400, "Update Transaction Failed"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            await _service.DeleteTransactionAsync(id);
            return Ok(new ApiResponse<string>(200, "Delete Transaction Successfully", null));
        }
    }
}
