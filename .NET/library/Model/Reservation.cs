using System.ComponentModel.DataAnnotations.Schema;

namespace OneBeyondApi.Model
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public DateTime ReservedDate { get; set; }

        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        [ForeignKey("Borrower")]
        public Guid BorrowerId { get; set; }
        public Borrower Borrower { get; set; }
    }
}
