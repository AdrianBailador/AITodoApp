// Assuming you have a TodoController.cs
using Microsoft.AspNetCore.Mvc;
using AITodoApp.Models; // Your TodoItem model namespace
using AITodoApp.Services; // Your GeminiService namespace
using System.Threading.Tasks;
using AITodoApp.Data; // For AppDbContext if needed in controller
using Microsoft.EntityFrameworkCore; // Add this line

public class TodoController : Controller
{
    private readonly AppDbContext _context;
    private readonly GeminiService _geminiService;

    public TodoController(AppDbContext context, GeminiService geminiService)
    {
        _context = context;
        _geminiService = geminiService;
    }

    // GET: /Todo/Index
    public async Task<IActionResult> Index()
    {
        var todoItems = await _context.TodoItems.ToListAsync();
        return View(todoItems); // Assuming Index.cshtml expects IEnumerable<TodoItem>
    }

    // GET: /Todo/Create
    // This action is called when a user navigates directly to the create page.
    public IActionResult Create()
    {
        // Always pass a new TodoItem instance to the view.
        var model = new TodoItem();
        return View(model);
    }

    // POST: /Todo/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Category,Priority")] TodoItem todoItem) // Bind only necessary properties
    {
        if (ModelState.IsValid)
        {
            todoItem.DateCreated = DateTime.UtcNow; // Set server-side
            todoItem.IsDone = false; // Default value
            _context.Add(todoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // If ModelState is not valid, return the view with the current todoItem to show validation errors.
        return View(todoItem);
    }

    // GET: /Todo/Suggest
    // This action is called when the "Suggest a Task (AI)" button is clicked.
    public async Task<IActionResult> Suggest()
    {
        var suggestion = await _geminiService.GetDetailedSuggestionAsync();
        
        // Create a new TodoItem and populate it from the suggestion.
        // Even if the suggestion is minimal (e.g., an error message in Title),
        // we still pass a valid TodoItem object to the view.
        var todoItem = new TodoItem
        {
            Title = suggestion.Title ?? "No title suggested", // Ensure Title is not null
            Category = suggestion.Category ?? "Uncategorized", // Ensure Category is not null
            Priority = suggestion.Priority ?? "Medium" // Ensure Priority is not null
            // ID will be 0 (default for new item)
            // DateCreated and IsDone will be set upon saving (POST Create)
        };

        // Use the "Create" view to display the suggested task for review.
        // The Create.cshtml view is designed to handle both new and suggested tasks.
        return View("Create", todoItem);
    }

    // Add other actions like Edit, Delete, Details as needed.
}