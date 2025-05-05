using OneBeyondApi.Dtos;
using OneBeyondApi.Model;

namespace OneBeyondApi.Services
{
    public interface IOnLoanService
    {
        Task<List<LoanDto>> GetActiveLoans();
        Task<(bool Success, bool WasLate, Fine? Fine)> ReturnBook(Guid bookStockId);
        Task<bool> ReserveBook(Guid bookId, Guid borrowerId);
        ReservationStatusDto GetReservationStatus(Guid bookId, Guid borrowerId);
    }
}
