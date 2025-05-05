using System.ComponentModel.DataAnnotations.Schema;

namespace OneBeyondApi.Model
{
    public class Fine
    {
        public Guid Id { get; set; }

        [ForeignKey("Borrower")]
        public Guid BorrowerId { get; set; }
        public Borrower Borrower { get; set; }

        public decimal Amount { get; set; }
        public DateTime IssuedDate { get; set; }
        public string Reason { get; set; }
        public bool IsPaid { get; set; }
    }
}
