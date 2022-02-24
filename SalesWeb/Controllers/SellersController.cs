#nullable disable
using Microsoft.AspNetCore.Mvc;
using SalesWeb.Models.ViewModels;
using SalesWeb.Services;
using SalesWeb.Models.Entities;
using SalesWeb.Services.Exceptions;
using System.Diagnostics;

namespace SalesWeb.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;
        private readonly ILogger _logger;

        public SellersController(SellerService sellerService, DepartmentService departmentService, ILogger<SellersController> logger)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
            _logger = logger;
        }

        // GET: Sellers
        public async Task<IActionResult> Index()
        {
            return View(await _sellerService.FindAllAsync());
        }

        // GET: Sellers
        public async Task<IActionResult> Create() 
        {
            var departments = await _departmentService.FindaAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        // POST: Sellers
        [HttpPost]
        // Avoid Injection Attacks
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller)
        {
            // If JavaScrit is disabled, don't post
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindaAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }

        // GET: Sellers
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for delete." });
            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = $"Seller of Id {id} not found for delete." });
            return View(obj);
        }

        // POST: Sellers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) 
        {
            try
            {
                await _sellerService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Seller already have Sales Records and couldn't be deleted. Error message: {e.Message}" }); 
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for details." });
            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = $"Seller of Id {id} not found for details." });
            return View(obj);
        }

        // GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for edit." });
            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = $"Seller of Id {id} not found edit." });

            List<Department> departments = await _departmentService.FindaAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments};
            return View(viewModel);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindaAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            if (id != seller.Id)
                return RedirectToAction(nameof(Error), new { message = $"Seller of Id {id} is not the same as the one posted ({seller.Id})." });

            try
            {
                await _sellerService.UpdateAsync(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Seller of Id {id} not found for update. ({e.Message})" });
            }
            catch (DbConcurrencyException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Seller of Id {id} is being update by another user. ({e.Message})" });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = $"There was a problema editing Seller {id}: {ex.Message}" });
            }
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }

        //private readonly SalesWebContext _context;

        //public SellersController(SalesWebContext context)
        //{
        //    _context = context;
        //}

        //// GET: Sellers
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Seller.ToListAsync());
        //}

        //// GET: Sellers/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var seller = await _context.Seller
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (seller == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(seller);
        //}

        //// GET: Sellers/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Sellers/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Email,BirthDate,BaseSalary")] Seller seller)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(seller);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(seller);
        //}

        //// GET: Sellers/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //if (id == null)
        //{
        //    return NotFound();
        //}

        //var seller = await _context.Seller.FindAsync(id);
        //if (seller == null)
        //{
        //    return NotFound();
        //}
        //return View(seller);
        //}

        //// POST: Sellers/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,BirthDate,BaseSalary")] Seller seller)
        //{
        //    if (id != seller.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(seller);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!SellerExists(seller.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(seller);
        //}

        //// GET: Sellers/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var seller = await _context.Seller
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (seller == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(seller);
        //}

        //// POST: Sellers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var seller = await _context.Seller.FindAsync(id);
        //    _context.Seller.Remove(seller);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool SellerExists(int id)
        //{
        //    return _context.Seller.Any(e => e.Id == id);
        //}
    }
}
