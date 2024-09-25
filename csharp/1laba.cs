using System;
using System.Collections.Generic;

codespace-laughing-chainsaw-7v9wx565pw77hr9x9
// Класс, представляющий книгу
public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }


    // Конструктор
    public Book(string title, string author, int year)
    {
        Title = title;
        Author = author;
        Year = year;
    }

    // Переопределение метода для удобного вывода
    public override string ToString()
    {
        return $"Название: {Title}, Автор: {Author}, Год: {Year}";
    }
}

// Главный класс программы
class Program
{
    static void Main(string[] args)
    {
 codespace-laughing-chainsaw-7v9wx565pw77hr9x9
        // Создаём список книг
        List<Book> books = new List<Book>();

        // Добавляем книги в список
        books.Add(new Book("The Catcher in the Rye", "J.D. Salinger", 1951));
        books.Add(new Book("To Kill a Mockingbird", "Harper Lee", 1960));
        books.Add(new Book("1984", "George Orwell", 1949));
        books.Add(new Book("The Great Gatsby", "F. Scott Fitzgerald", 1925));
        books.Add(new Book("Moby Dick", "Herman Melville", 1851));


        // Выводим список книг на экран
        Console.WriteLine("Список книг:");
        foreach (Book book in books)
        {
            Console.WriteLine(book.ToString());
        }
        
        // Пример поиска книги по названию
        string searchTitle = "1984";
        Book foundBook = books.Find(b => b.Title == searchTitle);
        
        if (foundBook != null)
        {
            Console.WriteLine($"\nНайдена книга: {foundBook}");
        }
        else
        {
            Console.WriteLine($"\nКнига с названием '{searchTitle}' не найдена.");
        }
    }
}
