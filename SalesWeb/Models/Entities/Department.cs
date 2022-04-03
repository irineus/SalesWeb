#nullable disable
namespace SalesWeb.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Seller> Sellers { get; set;} = new List<Seller>();

        public Department()
        {
        }

        public Department(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public void AddSeller(Seller seller) 
        {
            if (seller == null)
                throw new ArgumentNullException(nameof(seller));
            Sellers.Add(seller);
        }

        public double TotalBilledSales(DateTime initial, DateTime final)
        {
            if (initial > final)
                throw new ArgumentOutOfRangeException(nameof(initial), "Initial date is ahead of final date.");
            return Sellers.Sum(s => s.TotalBilledSales(initial, final));
        }

        public double TotalSales(DateTime initial, DateTime final)
        {
            if (initial > final)
                throw new ArgumentOutOfRangeException(nameof(initial), "Initial date is ahead of final date.");
            return Sellers.Sum(s => s.TotalSales(initial, final));
        }
    }
}
