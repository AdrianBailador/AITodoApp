# AITodoApp 📝✨

A simple To-Do List application built with ASP.NET Core MVC that leverages the Google Gemini API to suggest tasks. Users can manually add tasks or get AI-powered suggestions for new tasks, including a title, category, and priority.

## Features

*   **Manual Task Creation:** Add your own to-do items with a title, category, and priority.
*   **AI Task Suggestion:** Get intelligent task suggestions from Google's Gemini API. The AI suggests a title, category (e.g., "Work", "Personal"), and priority (e.g., "High", "Medium", "Low").
*   **Task Listing:** View all your current tasks.
*   **Persistent Storage:** Tasks are saved in an SQLite database using Entity Framework Core.
*   **Web Interface:** Clean and user-friendly interface built with ASP.NET Core MVC and Bootstrap.

## Technologies Used

*   **.NET 7/8** (or your specific version) - ASP.NET Core MVC
*   **C#**
*   **Entity Framework Core** - For ORM and database interaction.
*   **SQLite** - As the local database provider.
*   **Google Gemini API** - For AI-powered task suggestions.
*   **HTML, CSS, JavaScript**
    *   Bootstrap (likely, for styling)
    *   jQuery, jQuery Validate, jQuery Unobtrusive Validation (for client-side form validation)

## Project Structure (Key Folders & Files)

*   `AITodoApp.csproj`: Project file with dependencies.
*   `Program.cs`: Application entry point and service configuration.
*   `appsettings.json`: Configuration file (including Gemini API key).
*   `Controllers/`:
    *   `TodoController.cs`: Handles requests related to to-do items (CRUD, suggestions).
*   `Views/`: Razor views for the UI.
    *   `Todo/`: Views for Index, Create, etc.
    *   `Shared/`: Layout files, partial views (e.g., `_Layout.cshtml`, `_ValidationScriptsPartial.cshtml`).
*   `Models/`:
    *   `TodoItem.cs`: Represents a to-do task.
*   `Data/`:
    *   `AppDbContext.cs`: Entity Framework Core database context.
    *   `todos.db`: The SQLite database file (created after migrations).
*   `Services/`:
    *   `GeminiService.cs`: Service to interact with the Google Gemini API.
*   `Migrations/`: EF Core database migration files.
*   `wwwroot/`: Static assets (CSS, JS, images).

## Setup and Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/[your-username]/AITodoApp.git
    cd AITodoApp
    ```

2.  **Configure Gemini API Key:**
    You need a Google Gemini API key.
    *   Go to [Google AI Studio](https://aistudio.google.com/app/apikey) to get your API key.
    *   **Option A (Recommended for development): User Secrets**
        Initialize user secrets (if you haven't already):
        ```bash
        dotnet user-secrets init
        ```
        Set your API key and optionally the model name:
        ```bash
        dotnet user-secrets set "Gemini:ApiKey" "YOUR_GEMINI_API_KEY"
        # Optional: If you want to use a different model than the default (gemini-1.5-flash-latest)
        # dotnet user-secrets set "Gemini:ModelName" "your-chosen-model"
        ```
    *   **Option B: `appsettings.Development.json` (Less secure, do not commit API keys)**
        Create or open `appsettings.Development.json` and add your API key:
        ```json
        {
          "Logging": {
            "LogLevel": {
              "Default": "Information",
              "Microsoft.AspNetCore": "Warning"
            }
          },
          "Gemini": {
            "ApiKey": "YOUR_GEMINI_API_KEY",
            "ModelName": "gemini-1.5-flash-latest" // Or your preferred model
          }
        }
        ```
        **Warning:** Ensure `appsettings.Development.json` is in your `.gitignore` file if you use this method to avoid committing your API key.

3.  **Restore .NET dependencies:**
    ```bash
    dotnet restore
    ```

4.  **Apply Database Migrations:**
    This will create the `todos.db` SQLite database file and its schema if it doesn't exist.
    ```bash
    dotnet ef database update
    ```
    *(If you don't have EF tools installed globally, you might need to run `dotnet tool install --global dotnet-ef` first, or use `dotnet ef ...` from the project directory if it's a local tool.)*

5.  **Run the application:**
    ```bash
    dotnet run
    ```

6.  **Open in your browser:**
    Navigate to `http://localhost:5098` (or the port specified in your `launchSettings.json` / console output).

## Usage

*   **View Tasks:** The homepage lists all current to-do items.
*   **Add Task Manually:** Click the `+ New Task (Manual)` button, fill in the details, and click "Save Task".
*   **Get AI Suggestion:** Click the `✨ Suggest a Task (AI)` button. The application will query the Gemini API. The suggested task details (Title, Category, Priority) will pre-fill the form. Review or modify the suggestion and click "Save Task".

## Future Enhancements (Potential Ideas)

*   Editing and Deleting tasks.
*   Marking tasks as complete/incomplete.
*   User authentication and authorization.
*   More sophisticated AI prompts for varied suggestions.
*   Ability to choose different AI models.
*   Deployment instructions (e.g., Azure, Docker).

## Contributing

Contributions are welcome! If you have suggestions or want to improve the app, please feel free to:
1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

## License

Distributed under the MIT License. 

---
