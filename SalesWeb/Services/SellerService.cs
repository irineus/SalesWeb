using Microsoft.EntityFrameworkCore;
using SalesWeb.Data;
using SalesWebMVC.Models.Entities;

namespace SalesWeb.Services
{
    public class SellerService : DbContext
    {
        private readonly SalesWebContext _context;

        public SellerService(SalesWebContext context)
        { 
            _context = context;
        }

        public List<Seller> FindAll()
        {         
            return _context.Seller.ToList();
        }
    }
}
