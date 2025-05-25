// Models/TodoItem.cs
using System.ComponentModel.DataAnnotations; // For data annotations if you use them

namespace AITodoApp.Models
{
    public class TodoItem
    {
        public int Id { get; set; } // Primary Key (EF Core will assume this by convention if named Id or <ClassName>Id)

        [Required(ErrorMessage = "Title is required.")] // Example Data Annotation
        public string Title { get; set; } = "";

        public string Category { get; set; } = "";
        public string Priority { get; set; } = "";

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public bool IsDone { get; set; } = false;
    }
}