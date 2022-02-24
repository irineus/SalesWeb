#nullable disable
using SalesWeb.Models.Entities;

namespace SalesWeb.Models.ViewModels
{
    public class SellerFormViewModel
    {
        public Seller  Seller { get; set; }
        public ICollection<Department> Departments { get; set; }
    }
}
