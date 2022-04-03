#nullable disable
using Microsoft.AspNetCore.Mvc;
using SalesWeb.Models.ViewModels;
using SalesWeb.Services;
using SalesWeb.Models.Entities;
using SalesWeb.Services.Exceptions;
using System.Diagnostics;
using SalesWeb.Models.Interfaces;

namespace SalesWeb.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;
        private readonly SellerImgStorageService _sellerImgStorage;
        private readonly ILogger _logger;

        public SellersController(SellerService sellerService, DepartmentService departmentService, 
            ILogger<SellersController> logger, SellerImgStorageService sellerImgStorage)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
            _logger = logger;
            _sellerImgStorage = sellerImgStorage;
        }

        // GET: Sellers
        public async Task<IActionResult> Index()
        {
            _logger.LogTrace("Browsing all sellers.");
            return View(await _sellerService.FindAllAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for details." });
            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = $"Seller of Id {id} not found for details." });

            string fileName = id.Value + ".jpg";
            string workingDirectory = Environment.CurrentDirectory;
            string filePath = workingDirectory + "\\wwwroot\\Pictures\\Sellers";
            _sellerImgStorage.DownloadFile(fileName, filePath);
            _sellerImgStorage.GetBlobsByTag("seller", id.Value.ToString());

            ViewData["SellerImgSrc"] = "../../Pictures/Sellers/" + fileName;
            return View(obj);
        }

        // GET: Sellers
        public async Task<IActionResult> Create() 
        {
            var departments = await _departmentService.FindAllAsync();
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
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for edit." });
            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null)
                return RedirectToAction(nameof(Error), new { message = $"Seller of Id {id} not found edit." });

            List<Department> departments = await _departmentService.FindAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments };
            return View(viewModel);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
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
