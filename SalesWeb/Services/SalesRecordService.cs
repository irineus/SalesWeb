#nullable disable
using Microsoft.EntityFrameworkCore;
using SalesWeb.Data;
using SalesWeb.Models.Entities;
using SalesWeb.Models.Enums;
using SalesWeb.Services.Exceptions;

namespace SalesWeb.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebContext _context;

        public SalesRecordService(SalesWebContext context)
        {
            _context = context;
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            return await result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<IGrouping<Department, SalesRecord>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }

            var data = await result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return data.GroupBy(s => s.Seller.Department).ToList();

            //return await result
            //    .Include(x => x.Seller)
            //    .Include(x => x.Seller.Department)
            //    .OrderByDescending(x => x.Date)
            //    .GroupBy(x => x.Seller.Department)
            //    .ToListAsync();
        }

        public async Task<List<SalesRecord>> FindAllAsync()
        {
            return await _context.SalesRecord.ToListAsync();
        }

        public async Task InsertAsync(SalesRecord obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<SalesRecord> FindByIdAsync(int id)
        {
            return await _context.SalesRecord.Include(obj => obj.Seller).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.SalesRecord.FindAsync(id);
                _context.SalesRecord.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new IntegrityException(e.Message);
            }
        }

        public async Task UpdateAsync(SalesRecord obj)
        {
            var hasAny = await _context.SalesRecord.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
                throw new NotFoundException($"Sale Record Id {obj.Id} not found for update.");
            try
            {
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }

        public bool Any(int id)
        {
            return _context.SalesRecord.Any(x => x.Id == id);
        }

        public List<SaleStatus> SaleSatatusList()
        {
            var salesStatusList = Enum.GetValues(typeof(SaleStatus)).Cast<SaleStatus>().ToList();
            return salesStatusList;
        }
    }
}
