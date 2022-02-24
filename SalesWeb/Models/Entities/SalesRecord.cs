#nullable disable
using SalesWeb.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SalesWeb.Models.Entities
{
    public class SalesRecord
    {
        public int Id { get; set; }

        [Display(Name ="Sale Date")]
        [Required(ErrorMessage ="{0} is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }

        [Display(Name ="Sale Amount")]
        [Required(ErrorMessage ="{0} is required")]
        [Range(0.01, 999999.99, ErrorMessage = "{0} should be a value between {1} and {2}")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }

        public SaleStatus Status { get; set; }

        public Seller Seller { get; set; }

        [Display(Name = "Seller")]
        public int SellerId { get; set; }

        public SalesRecord()
        {
        }

        public SalesRecord(int id, DateTime date, double amount, SaleStatus status, Seller seller)
        {
            Id = id;
            Date = date;
            Amount = amount;
            Status = status;
            Seller = seller;
        }
    }
}
