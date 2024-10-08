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

    // Конкретный класс премиум-читателя
    public class PremiumMember : LibraryUser
    {
        public DateTime MembershipExpiry { get; private set; }

        public PremiumMember(string name, DateTime expiryDate) : base(name)
        {
            MembershipExpiry = expiryDate;
        }

        public override void DisplayUserInfo()
        {
            Console.WriteLine($"Премиум читатель: {Name}, Срок действия членства: {MembershipExpiry.ToShortDateString()}, Количество взятых предметов: {BorrowedItems.Count}");
        }
    }

    // Абстрактный класс для библиотеки
    public abstract class LibraryBase
    {
        protected List<ILibraryItem> Items { get; set; }
        protected List<LibraryUser> Members { get; set; }

        public LibraryBase()
        {
            Items = new List<ILibraryItem>();
            Members = new List<LibraryUser>();
        }

        public void AddItem(ILibraryItem item)
        {
            Items.Add(item);
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
                item.DisplayInfo();
            }
        }

        public ILibraryItem FindItemByTitle(string title)
        {
            return Items.Find(item => item.GetTitle().Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public abstract void AdditionalLibraryFunctionality();
    }

    // Конкретный класс библиотеки
    public class Library : LibraryBase
    {
        public Library() : base()
        {
        }

        public override void AdditionalLibraryFunctionality()
        {
            // Реализация специфичной функциональности для стандартной библиотеки
        }
    }

    // Конкретный класс цифровой библиотеки
    public class DigitalLibrary : LibraryBase
    {
        public DigitalLibrary() : base()
        {
        }

        public override void AdditionalLibraryFunctionality()
        {
            // Реализация специфичной функциональности для цифровой библиотеки
        }

        public void DownloadEBook(EBook ebook)
        {
            Console.WriteLine($"Скачивается электронная книга: {ebook.GetTitle()}");
            // Логика скачивания
        }
    }

    // Главная программа
    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();

            // Добавление книг
            library.AddItem(new EBook("The Master and Margarita", "Mikhail Bulgakov", 300));
            library.AddItem(new EBook("War and Peace", "Lev Tolstoy", 400));
            library.AddItem(new EBook("The Lord of the Rings", "J. R. R. Tolkien", 500));
            library.AddItem(new AudioBook("Cloud Atlas Audiobook", "David Mitchell", TimeSpan.FromHours(6)));
            library.AddItem(new AudioBook("Harry Potter Audiobook", "J.K. Rowling", TimeSpan.FromHours(8)));

            // Добавление журналов
            library.AddItem(new Magazine("National Geographic", "Susan Goldberg"));
            library.AddItem(new Magazine("Time", "Edward Felsenthal"));

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
                // Вывод всех предметов
                library.DisplayAllItems();

                Console.Write("\nВведите название предмета, который хотите взять: ");
                string itemTitle = Console.ReadLine();

                // Поиск предмета
                ILibraryItem itemToBorrow = library.FindItemByTitle(itemTitle);

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

                if (returnAnswer.Trim().ToLower() == "y" && itemToBorrow != null)
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

            Console.WriteLine("Спасибо за использование библиотеки!");
        }
    }
}
