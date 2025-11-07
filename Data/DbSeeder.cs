using ExpenseTracker.Models;

namespace ExpenseTracker.Data
{
    public static class DbSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if categories already exist
            if (context.Categories.Any())
            {
                return; // Data already seeded
            }

            // Add sample categories
            var categories = new List<Category>
            {
                // Expense Categories
                new Category { Title = "Food & Dining", Icon = "🍽️", Type = "Expense" },
                new Category { Title = "Transportation", Icon = "🚗", Type = "Expense" },
                new Category { Title = "Shopping", Icon = "🛒", Type = "Expense" },
                new Category { Title = "Entertainment", Icon = "🎬", Type = "Expense" },
                new Category { Title = "Bills & Utilities", Icon = "💡", Type = "Expense" },
                new Category { Title = "Healthcare", Icon = "⚕️", Type = "Expense" },
                new Category { Title = "Education", Icon = "📚", Type = "Expense" },
                new Category { Title = "Travel", Icon = "✈️", Type = "Expense" },
                new Category { Title = "Groceries", Icon = "🥕", Type = "Expense" },
                new Category { Title = "Clothing", Icon = "👕", Type = "Expense" },
                
                // Income Categories
                new Category { Title = "Salary", Icon = "💼", Type = "Income" },
                new Category { Title = "Freelance", Icon = "💻", Type = "Income" },
                new Category { Title = "Investment", Icon = "📈", Type = "Income" },
                new Category { Title = "Gift", Icon = "🎁", Type = "Income" },
                new Category { Title = "Other Income", Icon = "💰", Type = "Income" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Add sample transactions
            var transactions = new List<Transaction>
            {
                new Transaction 
                { 
                    CategoryId = categories.First(c => c.Title == "Salary").CategoryId,
                    Amount = 5000, 
                    Note = "Monthly Salary", 
                    Date = DateTime.Now.AddDays(-25)
                },
                new Transaction 
                { 
                    CategoryId = categories.First(c => c.Title == "Groceries").CategoryId,
                    Amount = 150, 
                    Note = "Weekly Groceries", 
                    Date = DateTime.Now.AddDays(-5)
                },
                new Transaction 
                { 
                    CategoryId = categories.First(c => c.Title == "Food & Dining").CategoryId,
                    Amount = 45, 
                    Note = "Lunch with colleagues", 
                    Date = DateTime.Now.AddDays(-3)
                },
                new Transaction 
                { 
                    CategoryId = categories.First(c => c.Title == "Transportation").CategoryId,
                    Amount = 25, 
                    Note = "Gas for car", 
                    Date = DateTime.Now.AddDays(-2)
                },
                new Transaction 
                { 
                    CategoryId = categories.First(c => c.Title == "Entertainment").CategoryId,
                    Amount = 30, 
                    Note = "Movie tickets", 
                    Date = DateTime.Now.AddDays(-1)
                },
                new Transaction 
                { 
                    CategoryId = categories.First(c => c.Title == "Bills & Utilities").CategoryId,
                    Amount = 120, 
                    Note = "Electricity bill", 
                    Date = DateTime.Now.AddDays(-10)
                },
                new Transaction 
                { 
                    CategoryId = categories.First(c => c.Title == "Freelance").CategoryId,
                    Amount = 800, 
                    Note = "Website project", 
                    Date = DateTime.Now.AddDays(-7)
                }
            };

            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }
    }
}
