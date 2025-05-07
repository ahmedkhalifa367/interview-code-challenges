using Microsoft.EntityFrameworkCore;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Dtos;
using OneBeyondApi.Model;

namespace OneBeyondApi.Services
{
    public class OnLoanService : IOnLoanService
    {
        private LibraryContext DbContext { get; }

        public OnLoanService(LibraryContext context)
        {
            DbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<LoanDto>> GetActiveLoans()
        public async Task<IList<LoanDto>> GetActiveLoans()
        {
            return await DbContext.Catalogue
                .Include(bs => bs.Book)
                .Include(bs => bs.OnLoanTo)
                .Where(bs => bs.OnLoanTo != null)
                .Select(bs => new LoanDto
                {
                    BookTitle = bs.Book.Name,
                    BorrowerName = bs.OnLoanTo.Name,
                    BorrowerEmail = bs.OnLoanTo.EmailAddress,
                    LoanEndDate = bs.LoanEndDate
                })
                .ToListAsync();
        }

        public async Task<(bool Success, bool WasLate, Fine? Fine)> ReturnBook(Guid bookStockId)
        {
            var bookStock = await DbContext.Catalogue
                .Include(bs => bs.Book)
                .Include(bs => bs.OnLoanTo)
                .FirstOrDefaultAsync(bs => bs.Id == bookStockId);

            if (bookStock == null || bookStock.OnLoanTo == null)
                return (false, false, null);

            bool isLate = bookStock.LoanEndDate.HasValue && bookStock.LoanEndDate.Value < DateTime.UtcNow;
            Fine? fine = null;

            if (isLate)
            {
                fine = new Fine
                {
                    Id = Guid.NewGuid(),
                    BorrowerId = bookStock.OnLoanTo.Id,
                    Amount = 5.00m,
                    IssuedDate = DateTime.UtcNow,
                    Reason = $"Late return of '{bookStock.Book.Name}'",
                    IsPaid = false
                };
                DbContext.Fines.Add(fine);
            }

            bookStock.OnLoanTo = null;
            bookStock.LoanEndDate = null;

            await DbContext.SaveChangesAsync();
            return (true, isLate, fine);
        }

        public async Task<bool> ReserveBook(Guid bookId, Guid borrowerId)
        {
            var exists = await DbContext.Reservations
                .AnyAsync(r => r.BookId == bookId && r.BorrowerId == borrowerId);
            if (exists) return false;

            DbContext.Reservations.Add(new Reservation
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                BorrowerId = borrowerId,
                ReservedDate = DateTime.UtcNow
            });

            await DbContext.SaveChangesAsync();
            return true;
        }

        public ReservationStatusDto GetReservationStatus(Guid bookId, Guid borrowerId)
        public ReservationStatusDto? GetReservationStatus(Guid bookId, Guid borrowerId)
        {
            var reservations = DbContext.Reservations
                .Where(r => r.BookId == bookId)
                .OrderBy(r => r.ReservedDate)
                .ToList();

            var position = reservations.FindIndex(r => r.BorrowerId == borrowerId);
            if (position == -1) return null;

            var loan = DbContext.Catalogue
                .Where(bs => bs.Book.Id == bookId && bs.OnLoanTo != null)
                .OrderBy(bs => bs.LoanEndDate)
                .FirstOrDefault();

            return new ReservationStatusDto
            {
                PositionInQueue = position + 1,
                EstimatedAvailability = loan?.LoanEndDate?.AddDays(position * 7)
            };
        }
    }
}

