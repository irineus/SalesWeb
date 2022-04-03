#nullable disable
using System.ComponentModel.DataAnnotations;

namespace SalesWeb.Models.Entities
{
    public class Seller
    {

        public int Id { get; set; }

        [Display(Name = "Seller Name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(60, MinimumLength = 5, ErrorMessage = "{0} size should be between {2} and {1} characters")]
        public string Name { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Enter a valid e-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Birth Date")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Base Salary")]
        [Required(ErrorMessage = "{0} is required")]
        [Range(100.0, 50000.0, ErrorMessage = "{0} should be a value between {1} and {2}")]
        [DataType(DataType.Currency)]
        public double BaseSalary { get; set; }

        public Department Department { get; set; }
        [Display(Name = "Seller Department")]
        public int DepartmentId { get; set; }

        public ICollection<SalesRecord> Sales { get; set; } = new List<SalesRecord>();

        public Seller()
        {
        }

        public Seller(int id, string name, string email, DateTime birthDate, double baseSalary, Department department)
        {
            Id = id;
            Name = name;
            Email = email;
            BirthDate = birthDate;
            BaseSalary = baseSalary;
            Department = department;
        }

        public void AddSales(SalesRecord sr)
        {
            if (sr == null)
            {
                throw new ArgumentNullException(nameof(sr));
            }
            Sales.Add(sr);
        }

        public void RemoveSales(SalesRecord sr)
        {
            if (sr == null)
            {
                throw new ArgumentNullException(nameof(sr));
            }
            if (!Sales.Contains(sr))
            {
                throw new KeyNotFoundException(nameof(sr));
            }
            Sales.Remove(sr);
        }

        public double TotalBilledSales(DateTime initial, DateTime final)
        {
            if (initial > final)
                throw new ArgumentOutOfRangeException(nameof(initial), "Initial date is ahead of final date.");
            return Sales.Where(sr => sr.Date >= initial && sr.Date <= final).Where(sr => sr.Status == Enums.SaleStatus.BILLED).Sum(sr => sr.Amount);
        }

        public double TotalSales(DateTime initial, DateTime final)
        {
            if (initial > final)
                throw new ArgumentOutOfRangeException(nameof(initial), "Initial date is ahead of final date.");
            return Sales.Where(sr => sr.Date >= initial && sr.Date <= final).Sum(sr => sr.Amount);
        }
    }
}
