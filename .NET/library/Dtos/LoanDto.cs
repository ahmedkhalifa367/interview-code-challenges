namespace OneBeyondApi.Dtos
{
    public class LoanDto
    {
        public string BookTitle { get; set; }
        public string BorrowerName { get; set; }
        public string BorrowerEmail { get; set; }
        public DateTime? LoanEndDate { get; set; }
    }
}
