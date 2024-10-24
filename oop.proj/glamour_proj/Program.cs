using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    // Общий интерфейс для всех библиотечных предметов
    public interface ILibraryItem
    {
        string GetTitle();
        string GetAuthor();
        void DisplayInfo();
        bool IsAvailable { get; }
        void Borrow();
        void Return();
    }

    // Абстрактный базовый класс для книг
    public abstract class Book : ILibraryItem
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

        public abstract void DisplayInfo();

        public void Borrow()
        {
            if (IsAvailable)
            {
                IsAvailable = false;
            }
            else
            {
                Console.WriteLine("Предмет уже взят.");
            }
        }

        public void Return()
        {
            IsAvailable = true;
        }
    }

    // Наследник для электронной книги
    public class EBook : Book
    {
        public int FileSize { get; private set; } // в МБ

        public EBook(string title, string author, int fileSize)
            : base(title, author)
        {
            FileSize = fileSize;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[E-Book] Название: {Title}, Автор: {Author}, Доступна: {IsAvailable}, Размер файла: {FileSize} MB");
        }
    }

    // Наследник для аудиокниги
    public class AudioBook : Book
    {
        public TimeSpan Duration { get; private set; }

        public AudioBook(string title, string author, TimeSpan duration)
            : base(title, author)
        {
            Duration = duration;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[AudioBook] Название: {Title}, Автор: {Author}, Доступна: {IsAvailable}, Длительность: {Duration}");
        }
    }

    // Класс для журналов
    public class Magazine : ILibraryItem
    {
        public string Title { get; private set; }
        public string Editor { get; private set; }
        public bool IsAvailable { get; private set; }

        public Magazine(string title, string editor)
        {
            Title = title;
            Editor = editor;
            IsAvailable = true;
        }

        public string GetTitle()
        {
            return Title;
        }

        public string GetAuthor()
        {
            return Editor;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"[Magazine] Название: {Title}, Редактор: {Editor}, Доступна: {IsAvailable}");
        }

        public void Borrow()
        {
            if (IsAvailable)
            {
                IsAvailable = false;
            }
            else
            {
                Console.WriteLine("Журнал уже взят.");
            }
        }

        public void Return()
        {
            IsAvailable = true;
        }
    }

    // Абстрактный класс для пользователей библиотеки
    public abstract class LibraryUser
    {
        public string Name { get; private set; }
        protected List<ILibraryItem> BorrowedItems { get; set; }

        public LibraryUser(string name)
        {
            Name = name;
            BorrowedItems = new List<ILibraryItem>();
        }

        public void BorrowItem(ILibraryItem item)
        {
            if (item.IsAvailable)
            {
                BorrowedItems.Add(item);
                item.Borrow();
                Console.WriteLine($"{Name} взял(а) предмет: {item.GetTitle()}");
            }
            else
            {
                Console.WriteLine($"Предмет {item.GetTitle()} недоступен.");
            }
        }

        public void ReturnItem(ILibraryItem item)
        {
            if (BorrowedItems.Contains(item))
            {
                BorrowedItems.Remove(item);
                item.Return();
                Console.WriteLine($"{Name} вернул(а) предмет: {item.GetTitle()}");
            }
            else
            {
                Console.WriteLine($"{Name} не брал(а) предмет: {item.GetTitle()}");
            }
        }

        public abstract void DisplayUserInfo();
    }

    // Конкретный класс читателя
    public class Member : LibraryUser
    {
        public Member(string name) : base(name)
        {
        }

        public override void DisplayUserInfo()
        {
            Console.WriteLine($"Читатель: {Name}, Количество взятых предметов: {BorrowedItems.Count}");
        }
    }

    // Абстрактный класс для библиотеки
    public abstract class LibraryBase
    {
        // Используем Dictionary для хранения предметов с уникальными идентификаторами
        protected Dictionary<int, ILibraryItem> Items { get; set; }
        protected List<LibraryUser> Members { get; set; }

        public LibraryBase()
        {
            Items = new Dictionary<int, ILibraryItem>();
            Members = new List<LibraryUser>();
        }

        //индексатор для доступа по ID
        public ILibraryItem this[int id]
        {
            get => Items.ContainsKey(id) ? Items[id] : null;
        }

        public void AddItem(int id, ILibraryItem item)
        {
            Items[id] = item;
        }

        //использование params для добавления нескольких книг
        public void AddItems(params (int id, ILibraryItem item)[] items)
        {
            foreach (var (id, item) in items)
            {
                AddItem(id, item);
            }
        }

        public void RegisterMember(LibraryUser member)
        {
            Members.Add(member);
            Console.WriteLine($"Зарегистрирован читатель: {member.Name}");
        }

        public void DisplayAllItems()
        {
            Console.WriteLine("\nПредметы в библиотеке:");
            foreach (var item in Items)
            {
                item.Value.DisplayInfo();
            }
        }

        public abstract void AdditionalLibraryFunctionality();
    }

    public class Library : LibraryBase
    {
        public Library() : base()
        {
        }

        public override void AdditionalLibraryFunctionality()
        {
           //
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();

            library.AddItems(
                (1, new EBook("The Master and Margarita", "Mikhail Bulgakov", 300)),
                (2, new EBook("War and Peace", "Lev Tolstoy", 400)),
                (3, new AudioBook("Harry Potter Audiobook", "J.K. Rowling", TimeSpan.FromHours(8)))
            );

            Console.Write("Добро пожаловать в библиотеку. Введите ваше имя: ");
            string fullName = Console.ReadLine();

            // Регистрация читателя
            Member member = new Member(fullName);
            library.RegisterMember(member);

            bool continueLibrary = true;

            while (continueLibrary)
            {
                library.DisplayAllItems();

                Console.Write("\nВведите ID предмета, который хотите взять: ");
                if (int.TryParse(Console.ReadLine(), out int itemId))
                {
                    ILibraryItem itemToBorrow = library[itemId]; // Используем индексатор

                    if (itemToBorrow != null)
                    {
                        member.BorrowItem(itemToBorrow);
                    }
                    else
                    {
                        Console.WriteLine("Такого предмета нет в библиотеке.");
                    }

                    library.DisplayAllItems();

                    Console.Write("\nХотите вернуть предмет? (y/n): ");
                    string returnAnswer = Console.ReadLine();

                    if (returnAnswer.Trim().ToLower() == "y")
                    {
                        member.ReturnItem(itemToBorrow);
                    }

                    library.DisplayAllItems();

                    Console.Write("\nХотите взять другой предмет? (y/n): ");
                    string continueAnswer = Console.ReadLine();
                    if (continueAnswer.Trim().ToLower() != "y")
                    {
                        continueLibrary = false;
                    }
                }
                else
                {
                    Console.WriteLine("Неправильный ввод ID.");
                }
            }

            Console.WriteLine("Спасибо за использование библиотеки!");
        }
    }
}
