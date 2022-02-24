#nullable disable
using SalesWeb.Models.Entities;
using SalesWeb.Models.Enums;

namespace SalesWeb.Models.ViewModels
{
    public class SalesRecordFormViewModel
    {
        public SalesRecord SalesRecord { get; set; }
        public ICollection<Seller> Sellers { get; set; }
        public ICollection<SaleStatus> StatusList { get; set; }

    }
}
