#nullable disable
using Microsoft.AspNetCore.Mvc;
using SalesWeb.Models.Entities;
using SalesWeb.Models.Enums;
using SalesWeb.Models.ViewModels;
using SalesWeb.Services;
using SalesWeb.Services.Exceptions;
using System.Diagnostics;

namespace SalesWeb.Controllers
{
    public class SalesRecordsController : Controller
    {
        private readonly SalesRecordService _salesRecordService;
        private readonly SellerService _sellerService;
        private readonly ILogger _logger;

        public SalesRecordsController(SalesRecordService salesRecordService, SellerService sellerService, ILogger<SellerService> logger)
        {
            _salesRecordService = salesRecordService;
            _sellerService = sellerService;
            _logger = logger;
        }

        // GET: SalesRecords
        public IActionResult Index()
        {
            _logger.LogInformation("Carregando página inicial");
            return View();
        }

        public async Task<IActionResult> SimpleSearch(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = DateTime.Now;
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            var result = await _salesRecordService.FindByDateAsync(minDate, maxDate);
            return View(result);
        }

        public async Task<IActionResult> GroupingSearch(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = DateTime.Now;
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            var result = await _salesRecordService.FindByDateGroupingAsync(minDate, maxDate);
            return View(result);
        }

        //// GET: SalesRecords
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.SalesRecord.ToListAsync());
        //}

        // GET: SalesRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for details." });
            }

            var salesRecord = await _salesRecordService.FindByIdAsync(id.Value);
            if (salesRecord == null)
            {
                return RedirectToAction(nameof(Error), new { message = $"Sale of Id {id} not found for details." });
            }

            return View(salesRecord);
        }

        // GET: SalesRecords/Create
        public async Task<IActionResult> Create()
        {
            var sellers = await _sellerService.FindAllAsync();
            var salesStatusList = _salesRecordService.SaleSatatusList();
            var viewModel = new SalesRecordFormViewModel { Sellers = sellers, StatusList = salesStatusList };
            return View(viewModel);
        }

        // POST: SalesRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesRecord salesRecord)
        {
            if (!ModelState.IsValid)
            {
                var sellers = await _sellerService.FindAllAsync();
                var salesStatusList = _salesRecordService.SaleSatatusList();
                var viewModel = new SalesRecordFormViewModel { Sellers = sellers, StatusList = salesStatusList };
                return View(viewModel);
            }
            await _salesRecordService.InsertAsync(salesRecord);
            return RedirectToAction(nameof(Index));
        }

        // GET: SalesRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for edit." });
            }
            var salesRecord = await _salesRecordService.FindByIdAsync(id.Value);
            if (salesRecord == null)
            {
                return RedirectToAction(nameof(Error), new { message = $"Sale Record of Id {id} not found edit." });
            }

            List<Seller> sellers = await _sellerService.FindAllAsync();
            List<SaleStatus> salesStatusList = _salesRecordService.SaleSatatusList();
            SalesRecordFormViewModel viewModel = new SalesRecordFormViewModel { SalesRecord = salesRecord, Sellers = sellers, StatusList = salesStatusList };
            return View(viewModel);
        }

        // POST: SalesRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SalesRecord salesRecord)
        {
            if (!ModelState.IsValid)
            {
                var sellers = await _sellerService.FindAllAsync();
                var salesStatusList = _salesRecordService.SaleSatatusList();
                var viewModel = new SalesRecordFormViewModel { SalesRecord = salesRecord, Sellers = sellers, StatusList = salesStatusList };
                return View(viewModel);
            }

            if (id != salesRecord.Id)
            {
                return RedirectToAction(nameof(Error), new { message = $"Sale Record of Id {id} is not the same as the one posted ({salesRecord.Id})." });
            }

            try
            {
                await _salesRecordService.UpdateAsync(salesRecord);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Sales Record of Id {id} not found for update. ({e.Message})" });
            }
            catch (DbConcurrencyException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Sales Record of Id {id} is being update by another user. ({e.Message})" });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = $"There was a problema editing Sales Record {id}: {ex.Message}" });
            }
        }

        // GET: SalesRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for delete." });
            var obj = await _salesRecordService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = $"Sales Record of Id {id} not found for delete." });
            return View(obj);
        }

        // POST: SalesRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _salesRecordService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Sales Records already have an Integrity Constraint and couldn't be deleted. Error message: {e.Message}" });
            }
        }

        private bool SalesRecordExists(int id)
        {
            return _salesRecordService.Any(id);
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
    }
}
