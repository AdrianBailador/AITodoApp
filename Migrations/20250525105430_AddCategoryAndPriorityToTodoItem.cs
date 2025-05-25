using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITodoApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndPriorityToTodoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "TodoItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "TodoItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TodoItems");
        }
    }
}
