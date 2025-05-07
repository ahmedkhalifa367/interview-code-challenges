using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.Services;

namespace OneBeyondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnLoanController : ControllerBase
    {
        private IOnLoanService OnLoanService { get; }

        public OnLoanController(IOnLoanService onLoanService)
        {
            OnLoanService = onLoanService ?? throw new ArgumentNullException(nameof(onLoanService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveLoans()
        {
            var loans = await OnLoanService.GetActiveLoans();
            return Ok(loans);
        }

        [HttpPost("return/{bookStockId}")]
        public async Task<IActionResult> ReturnBook(Guid bookStockId)
        {
            var result = await OnLoanService.ReturnBook(bookStockId);
            if (!result.Success) return BadRequest("Book is not on loan.");

            return Ok(new
            {
                Message = "Book returned successfully.",
                WasLate = result.WasLate,
                Fine = result.Fine != null ? new { result.Fine.Amount, result.Fine.Reason } : null
            });
        }

        [HttpPost("reserve/{bookId}/{borrowerId}")]
        public async Task<IActionResult> ReserveBook(Guid bookId, Guid borrowerId)
        {
            var success = await OnLoanService.ReserveBook(bookId, borrowerId);
            if (!success) return BadRequest("Already reserved.");

            return Ok("Reservation placed successfully.");
        }

        [HttpGet("reservation-status/{bookId}/{borrowerId}")]
        public IActionResult GetReservationStatus(Guid bookId, Guid borrowerId)
        {
            var status = OnLoanService.GetReservationStatus(bookId, borrowerId);
            if (status == null) return NotFound("No reservation found.");
            return Ok(status);
        }
    }
}
