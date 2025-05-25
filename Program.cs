using AITodoApp.Data;
using AITodoApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura EF Core con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=todos.db"));

// Configura HttpClient para GeminiService
builder.Services.AddHttpClient<GeminiService>();

// Agrega controladores y vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware básico
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todo}/{action=Index}/{id?}");

app.Run();
