using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public DateTime Date { get; set; }=DateTime.Now;
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}