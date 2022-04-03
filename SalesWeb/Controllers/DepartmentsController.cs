#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesWeb.Data;
using SalesWeb.Models.Entities;
using SalesWeb.Models.ViewModels;
using SalesWeb.Services;
using SalesWeb.Services.Exceptions;
using System.Diagnostics;

namespace SalesWeb.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly SalesWebContext _context;
        private readonly DepartmentService _departmentService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public DepartmentsController(SalesWebContext context, DepartmentService departmentService, IConfiguration configuration,
            ILogger<DepartmentsController> logger)
        {
            _context = context;
            _departmentService = departmentService;
            _configuration = configuration;
            _logger = logger;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            _logger.LogTrace("Browsing all departments.");
            return View(await _departmentService.FindAllAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for details." });
            var department = await _departmentService.FindByIdAsync(id.Value);
            if (department == null)
            {
                return RedirectToAction(nameof(Error), new { message = $"Department of Id {id} not found for details." });
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }
            await _departmentService.InsertAsync(department);
            return RedirectToAction(nameof(Index));            
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Error), new { message = "Id can't be null for edit." });
            var department = await _departmentService.FindByIdAsync(id.Value);
            if (department == null)
                return RedirectToAction(nameof(Error), new { message = $"Department of Id {id} not found edit." });
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Department department)
        {
            if (!ModelState.IsValid)
                return View(department);
            if (id != department.Id)
                return RedirectToAction(nameof(Error), new { message = $"Department of Id {id} is not the same as the one posted ({department.Id})." });

            try
            {
                await _departmentService.UpdateAsync(department);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Department of Id {id} not found for update. ({e.Message})" });
            }
            catch (DbConcurrencyException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Department of Id {id} is being update by another user. ({e.Message})" });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = $"There was a problem editing Department {id}: {ex.Message}" });
            }

        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                RedirectToAction(nameof(Error), new { message = "Id can't be null for delete." });
            var department = await _departmentService.FindByIdAsync(id.Value);
            if (department == null)
                return RedirectToAction(nameof(Error), new { message = $"Department of Id {id} not found for delete." });
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _departmentService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Department already have Sellers and couldn't be deleted. Error message: {e.Message}" });
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
