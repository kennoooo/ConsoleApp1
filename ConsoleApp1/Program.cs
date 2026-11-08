using System.Net.Http.Json;

class Program
{
    static readonly HttpClient httpClient = new HttpClient();
    static string baseUrl = "https://jsonplaceholder.typicode.com";
    static int userId;

    static async Task Main()
    {
        userId = ReadInt("Введите ID пользователя: ");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("1. Показать задачи пользователя");
            Console.WriteLine("2. Найти задачу по ID");
            Console.WriteLine("3. Создать задачу");
            Console.WriteLine("4. Изменить статус задачи");
            Console.WriteLine("5. Удалить задачу");
            Console.WriteLine("0. Выход");
            Console.WriteLine();
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            if (choice == "1")
            {
                await ShowTodos();
            }
            else if (choice == "2")
            {
                await FindTodo();
            }
            else if (choice == "3")
            {
                await CreateTodo();
            }
            else if (choice == "4")
            {
                await UpdateTodoStatus();
            }
            else if (choice == "5")
            {
                await DeleteTodo();
            }
            else if (choice == "0")
            {
                break;
            }
            else
            {
                Console.WriteLine("Неверный выбор");
            }

            Console.WriteLine();
        }
    }

    static int ReadInt(string text)
    {
        int number;
        Console.Write(text);
        while (!int.TryParse(Console.ReadLine(), out number))
        {
            Console.Write("Введите число: ");
        }
        return number;
    }

    static string GetStatus(bool completed)
    {
        if (completed)
        {
            return "выполнена";
        }
        return "не выполнена";
    }

    static async Task ShowTodos()
    {
        var response = await httpClient.GetAsync($"{baseUrl}/todos?userId={userId}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Ошибка: {response.StatusCode}");
            return;
        }

        List<Todo> todos = await response.Content.ReadFromJsonAsync<List<Todo>>();

        if (todos.Count == 0)
        {
            Console.WriteLine("Задач не найдено");
            return;
        }

        foreach (Todo todo in todos)
        {
            Console.WriteLine($"{todo.Id}. {todo.Title} - {GetStatus(todo.Completed)}");
        }
    }

    static async Task FindTodo()
    {
        int id = ReadInt("Введите ID задачи: ");

        var response = await httpClient.GetAsync($"{baseUrl}/todos/{id}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Ошибка: {response.StatusCode}");
            return;
        }

        Todo todo = await response.Content.ReadFromJsonAsync<Todo>();
        Console.WriteLine($"{todo.Id}. {todo.Title} - {GetStatus(todo.Completed)}");
    }

    static async Task CreateTodo()
    {
        Console.Write("Введите название задачи: ");
        string title = Console.ReadLine();

        Todo newTodo = new Todo
        {
            UserId = userId,
            Title = title,
            Completed = false
        };

        var response = await httpClient.PostAsJsonAsync($"{baseUrl}/todos", newTodo);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Ошибка: {response.StatusCode}");
            return;
        }

        Todo createdTodo = await response.Content.ReadFromJsonAsync<Todo>();
        Console.WriteLine($"Создана задача #{createdTodo.Id}: {createdTodo.Title}");
    }

    static async Task UpdateTodoStatus()
    {
        int id = ReadInt("Введите ID задачи: ");
        int status = ReadInt("Введите статус (1 - выполнена, 0 - не выполнена): ");

        var data = new { completed = status == 1 };

        var response = await httpClient.PatchAsJsonAsync($"{baseUrl}/todos/{id}", data);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Ошибка: {response.StatusCode}");
            return;
        }

        Todo todo = await response.Content.ReadFromJsonAsync<Todo>();
        Console.WriteLine($"Задача #{todo.Id} теперь {GetStatus(todo.Completed)}");
    }

    static async Task DeleteTodo()
    {
        int id = ReadInt("Введите ID задачи: ");

        var response = await httpClient.DeleteAsync($"{baseUrl}/todos/{id}");

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Задача #{id} успешно удалена");
        }
        else
        {
            Console.WriteLine($"Ошибка: {response.StatusCode}");
        }
    }
}

public class Todo
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool Completed { get; set; }
}