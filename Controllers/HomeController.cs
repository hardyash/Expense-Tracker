using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Get recent transactions
        var recentTransactions = await _context.Transactions
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .Take(7)
            .ToListAsync();

        // Get total income and expense for current month
        var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var monthlyTransactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.Date >= startOfMonth && t.Date <= endOfMonth)
            .ToListAsync();

        var totalIncome = monthlyTransactions
            .Where(t => t.Category.Type == "Income")
            .Sum(t => t.Amount);

        var totalExpense = monthlyTransactions
            .Where(t => t.Category.Type == "Expense")
            .Sum(t => t.Amount);

        var balance = totalIncome - totalExpense;

        // Get categories
        var categories = await _context.Categories.ToListAsync();

        // Get expense categories with totals for current month (using already fetched data)
        var expensesByCategory = monthlyTransactions
            .Where(t => t.Category.Type == "Expense")
            .GroupBy(t => t.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
            .OrderByDescending(x => x.Total)
            .ToList();

        ViewBag.RecentTransactions = recentTransactions;
        ViewBag.TotalIncome = totalIncome;
        ViewBag.TotalExpense = totalExpense;
        ViewBag.Balance = balance;
        ViewBag.Categories = categories;
        ViewBag.ExpensesByCategory = expensesByCategory;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
