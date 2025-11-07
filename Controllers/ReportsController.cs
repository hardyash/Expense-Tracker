using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            // Get current month data
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Get all transactions for the current month
            var monthlyTransactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= startOfMonth && t.Date <= endOfMonth)
                .ToListAsync();

            // Calculate totals
            var totalIncome = monthlyTransactions
                .Where(t => t.Category.Type == "Income")
                .Sum(t => t.Amount);

            var totalExpense = monthlyTransactions
                .Where(t => t.Category.Type == "Expense")
                .Sum(t => t.Amount);

            // Get expense breakdown by category
            var expensesByCategory = monthlyTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category)
                .Select(g => new { 
                    Category = g.Key, 
                    Total = g.Sum(t => t.Amount),
                    Count = g.Count(),
                    Percentage = totalExpense > 0 ? (g.Sum(t => t.Amount) / totalExpense * 100) : 0
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            // Get income breakdown by category
            var incomeByCategory = monthlyTransactions
                .Where(t => t.Category.Type == "Income")
                .GroupBy(t => t.Category)
                .Select(g => new { 
                    Category = g.Key, 
                    Total = g.Sum(t => t.Amount),
                    Count = g.Count(),
                    Percentage = totalIncome > 0 ? (g.Sum(t => t.Amount) / totalIncome * 100) : 0
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            // Get daily spending for the last 30 days
            var last30Days = now.AddDays(-30);
            var dailySpending = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= last30Days && t.Category.Type == "Expense")
                .GroupBy(t => t.Date.Date)
                .Select(g => new { 
                    Date = g.Key, 
                    Amount = g.Sum(t => t.Amount) 
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Get monthly comparison (last 6 months)
            var monthlyComparison = new List<object>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = startOfMonth.AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                var monthTransactions = await _context.Transactions
                    .Include(t => t.Category)
                    .Where(t => t.Date >= monthStart && t.Date <= monthEnd)
                    .ToListAsync();

                var income = monthTransactions.Where(t => t.Category.Type == "Income").Sum(t => t.Amount);
                var expense = monthTransactions.Where(t => t.Category.Type == "Expense").Sum(t => t.Amount);

                monthlyComparison.Add(new
                {
                    Month = monthStart.ToString("MMM yyyy"),
                    Income = income,
                    Expense = expense,
                    Balance = income - expense
                });
            }

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.Balance = totalIncome - totalExpense;
            ViewBag.ExpensesByCategory = expensesByCategory;
            ViewBag.IncomeByCategory = incomeByCategory;
            ViewBag.DailySpending = dailySpending;
            ViewBag.MonthlyComparison = monthlyComparison;
            ViewBag.CurrentMonth = startOfMonth.ToString("MMMM yyyy");

            return View();
        }

        // GET: Reports/Monthly
        public async Task<IActionResult> Monthly(int? year, int? month)
        {
            var selectedYear = year ?? DateTime.Now.Year;
            var selectedMonth = month ?? DateTime.Now.Month;
            
            var startOfMonth = new DateTime(selectedYear, selectedMonth, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var monthlyTransactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= startOfMonth && t.Date <= endOfMonth)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            ViewBag.SelectedMonth = startOfMonth.ToString("MMMM yyyy");
            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedMonthNum = selectedMonth;
            
            return View(monthlyTransactions);
        }

        // GET: Reports/Yearly
        public async Task<IActionResult> Yearly(int? year)
        {
            var selectedYear = year ?? DateTime.Now.Year;
            var startOfYear = new DateTime(selectedYear, 1, 1);
            var endOfYear = new DateTime(selectedYear, 12, 31);

            var yearlyTransactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= startOfYear && t.Date <= endOfYear)
                .ToListAsync();

            // Group by month
            var monthlyData = yearlyTransactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    Income = g.Where(t => t.Category.Type == "Income").Sum(t => t.Amount),
                    Expense = g.Where(t => t.Category.Type == "Expense").Sum(t => t.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            ViewBag.SelectedYear = selectedYear;
            ViewBag.MonthlyData = monthlyData;
            ViewBag.YearlyIncome = yearlyTransactions.Where(t => t.Category.Type == "Income").Sum(t => t.Amount);
            ViewBag.YearlyExpense = yearlyTransactions.Where(t => t.Category.Type == "Expense").Sum(t => t.Amount);
            
            return View(yearlyTransactions);
        }
    }
}
