#nullable disable

using Microsoft.EntityFrameworkCore;
using SalesWeb.Data;
using SalesWeb.Models.Entities;
using SalesWeb.Services.Exceptions;

namespace SalesWeb.Services
{
    public class DepartmentService
    {
        private readonly SalesWebContext _context;

        public DepartmentService(SalesWebContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> FindAllAsync()
        {
            return await _context.Department.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<Department> FindByIdAsync(int id)
        {
            return await _context.Department.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task InsertAsync(Department obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department obj)
        {
            var hasAny = await _context.Department.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
                throw new NotFoundException($"Department Id {obj.Id} not found for update.");
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

        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Department.FindAsync(id);
                _context.Department.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new IntegrityException(e.Message);
            }
        }
    }
}
