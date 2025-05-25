using Microsoft.EntityFrameworkCore;
using AITodoApp.Models;

namespace AITodoApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<TodoItem> TodoItems { get; set; }
}
