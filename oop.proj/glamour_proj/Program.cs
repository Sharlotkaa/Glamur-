using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsAvailable { get; set; }

        public Book(string title, string author)
        {
            Title = title;
            Author = author;
            IsAvailable = true;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Название: {Title}, Автор: {Author}, Доступна: {IsAvailable}");
        }
    }

    //читатель и его действия(возврат, взятие)
    public class Member
    {
        public string Name { get; set; }
        public List<Book> BorrowedBooks { get; set; }

        public Member(string name)
        {
            Name = name;
            BorrowedBooks = new List<Book>();
        }

        //проверка взятия книги
        public void BorrowBook(Book book)
        {
            if (book.IsAvailable)
            {
                BorrowedBooks.Add(book);
                book.IsAvailable = false;
                Console.WriteLine($"{Name} взял(а) книгу: {book.Title}");
            }
            else
            {
                Console.WriteLine($"Книга {book.Title} недоступна.");
            }
        }
        //возврат
        public void ReturnBook(Book book)
        {
            if (BorrowedBooks.Contains(book))
            {
                BorrowedBooks.Remove(book);
                book.IsAvailable = true;
                Console.WriteLine($"{Name} вернул(а) книгу: {book.Title}");
            }
        }
    }

    //управление библиотекой
    public class Library
    {
        public List<Book> Books { get; set; }
        public List<Member> Members { get; set; }

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
            foreach (var book in Books)
            {
                if (string.Equals(book.Title, title, StringComparison.OrdinalIgnoreCase))
                {
                    return book;
                }
            }
            return null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //создание самой библиотеки
            Library library = new Library();

            //добавление книг
            library.AddBook(new Book("The Master and Margarita", "Mikhail Bulgakov"));
            library.AddBook(new Book("War and Peace", "Lev Tolstoy"));
            library.AddBook(new Book("The Lord of the Rings", "J. R. R. Tolkien"));
            library.AddBook(new Book("Cloud Atlas", "David Mitchell"));

            //пользователь
            Console.Write("Добро пожаловать в библиотеку. Введите ваше имя: ");
            string firstName = Console.ReadLine();

            Console.Write("Введите вашу фамилию: ");
            string lastName = Console.ReadLine();

            string fullName = $"{firstName} {lastName}";

            //регистрация читателя
            Member member = new Member(fullName);
            library.RegisterMember(member);

            bool continueLibrary = true;

            while (continueLibrary)
            {
                //вывод всех книг
                library.DisplayAllBooks();

                Console.Write("\nВведите название книги, которую хотите взять: ");
                string bookTitle = Console.ReadLine();

                //поиск книги
                Book bookToBorrow = library.FindBookByTitle(bookTitle);

                if (bookToBorrow != null)
                {
                    //тут взятие книги
                    member.BorrowBook(bookToBorrow);
                }
                else
                {
                    Console.WriteLine("Такой книги нет в библиотеке.");
                }

                //выводит все книги после взятия
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
