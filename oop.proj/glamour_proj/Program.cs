using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    // Интерфейс
    public interface IBookInfo
    {
        string GetTitle();
        string GetAuthor();
        void DisplayInfo();
    }

    // Базовый класс для книги, реализующий интерфейс
    public class Book : IBookInfo
    {
        protected string Title { get; private set; }
        protected string Author { get; private set; }
        public bool IsAvailable { get; private set; }

        public Book(string title, string author)
        {
            Title = title;
            Author = author;
            IsAvailable = true;
        }

        public string GetTitle()
        {
            return Title;
        }

        public string GetAuthor()
        {
            return Author;
        }

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"Название: {Title}, Автор: {Author}, Доступна: {IsAvailable}");
        }

        public void Borrow()
        {
            if (IsAvailable)
            {
                IsAvailable = false;
            }
            else
            {
                Console.WriteLine("Книга уже взята.");
            }
        }

        public void Return()
        {
            IsAvailable = true;
        }
    }

    // Наследник для электронной книги, также реализующий интерфейс
    public class EBook : Book, IBookInfo
    {
        public int FileSize { get; private set; }

        public EBook(string title, string author, int fileSize)
            : base(title, author)
        {
            FileSize = fileSize;
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Размер файла: {FileSize} MB");
        }
    }

    // Класс для читателя
    public class Member
    {
        public string Name { get; private set; }
        private List<Book> BorrowedBooks { get; set; }

        public Member(string name)
        {
            Name = name;
            BorrowedBooks = new List<Book>();
        }

        public void BorrowBook(Book book)
        {
            if (book.IsAvailable)
            {
                BorrowedBooks.Add(book);
                book.Borrow();
                Console.WriteLine($"{Name} взял(а) книгу: {book.GetTitle()}");
            }
            else
            {
                Console.WriteLine($"Книга {book.GetTitle()} недоступна.");
            }
        }

        public void ReturnBook(Book book)
        {
            if (BorrowedBooks.Contains(book))
            {
                BorrowedBooks.Remove(book);
                book.Return();
                Console.WriteLine($"{Name} вернул(а) книгу: {book.GetTitle()}");
            }
        }
    }

    // Класс библиотеки
    public class Library
    {
        private List<Book> Books { get; set; }
        private List<Member> Members { get; set; }

        public Library()
        {
            Books = new List<Book>();
            Members = new List<Member>();
        }

        public void AddBook(Book book)
        {
            Books.Add(book);
        }

        public void RegisterMember(Member member)
        {
            Members.Add(member);
            Console.WriteLine($"Зарегистрирован читатель: {member.Name}");
        }

        public void DisplayAllBooks()
        {
            Console.WriteLine("\nКниги в библиотеке:");
            foreach (var book in Books)
            {
                book.DisplayInfo();
            }
        }

        public Book FindBookByTitle(string title)
        {
            return Books.Find(book => book.GetTitle().Equals(title, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Главная программа
    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();

            // Добавление книг
            library.AddBook(new Book("The Master and Margarita", "Mikhail Bulgakov"));
            library.AddBook(new Book("War and Peace", "Lev Tolstoy"));
            library.AddBook(new EBook("The Lord of the Rings", "J. R. R. Tolkien", 500));
            library.AddBook(new Book("Cloud Atlas", "David Mitchell"));

            Console.Write("Добро пожаловать в библиотеку. Введите ваше имя: ");
            string firstName = Console.ReadLine();

            Console.Write("Введите вашу фамилию: ");
            string lastName = Console.ReadLine();

            string fullName = $"{firstName} {lastName}";

            // Регистрация читателя
            Member member = new Member(fullName);
            library.RegisterMember(member);

            bool continueLibrary = true;

            while (continueLibrary)
            {
                // Вывод всех книг
                library.DisplayAllBooks();

                Console.Write("\nВведите название книги, которую хотите взять: ");
                string bookTitle = Console.ReadLine();

                // Поиск книги
                Book bookToBorrow = library.FindBookByTitle(bookTitle);

                if (bookToBorrow != null)
                {
                    member.BorrowBook(bookToBorrow);
                }
                else
                {
                    Console.WriteLine("Такой книги нет в библиотеке.");
                }

                library.DisplayAllBooks();

                Console.Write("\nХотите вернуть книгу? (y/n): ");
                string returnAnswer = Console.ReadLine();

                if (returnAnswer.ToLower() == "y" && bookToBorrow != null)
                {
                    member.ReturnBook(bookToBorrow);
                }

                library.DisplayAllBooks();

                Console.Write("\nХотите взять другую книгу? (y/n): ");
                string continueAnswer = Console.ReadLine();
                if (continueAnswer.ToLower() != "y")
                {
                    continueLibrary = false;
                }
            }

            Console.WriteLine("Спасибо за использование библиотеки!");
        }
    }
}

            Console.WriteLine("Спасибо за использование библиотеки!");
        }
    }
}
