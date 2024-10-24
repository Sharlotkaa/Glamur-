using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    public interface ILibraryItem
    {
        string GetTitle();
        string GetAuthor();
        void DisplayInfo();
        bool IsAvailable { get; }
        void Borrow();
        void Return();
    }

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

    public class EBook : Book
    {
        public int FileSize { get; private set; }

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

    public class Member : LibraryUser
    {
        public Member(string name) : base(name) { }

        public override void DisplayUserInfo()
        {
            Console.WriteLine($"Читатель: {Name}, Количество взятых предметов: {BorrowedItems.Count}");
        }
    }

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

    public abstract class LibraryBase
    {
        public Dictionary<int, ILibraryItem> Items { get; set; }  // Изменено на public
        public List<LibraryUser> Members { get; set; }  // Изменено на public
        public ILibraryItem[] ItemsArray;  // Массив для хранения предметов
        public LibraryUser[] MembersArray;  // Массив для хранения читателей

        public LibraryBase()
        {
            Items = new Dictionary<int, ILibraryItem>();
            Members = new List<LibraryUser>();
            ItemsArray = new ILibraryItem[100];  // Предположим, что у нас есть место для 100 предметов
            MembersArray = new LibraryUser[100];  // Предположим, что у нас есть место для 100 читателей
        }

        public ILibraryItem this[int id] => Items.ContainsKey(id) ? Items[id] : null;

        public void AddItem(int id, ILibraryItem item)
        {
            Items[id] = item;
            ItemsArray[id] = item;  // Также добавляем предмет в массив
        }

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
            MembersArray[Members.Count - 1] = member;  // Добавляем читателя в массив
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
        public override void AdditionalLibraryFunctionality()
        {
            // Дополнительная функциональность библиотеки
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();

            Console.WriteLine("Добро пожаловать в библиотеку!");
            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Добавить предмет в библиотеку");
                Console.WriteLine("2. Зарегистрировать читателя");
                Console.WriteLine("3. Взять предмет");
                Console.WriteLine("4. Вернуть предмет");
                Console.WriteLine("5. Показать все предметы");
                Console.WriteLine("6. Выйти");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Выберите тип предмета: 1. Электронная книга 2. Аудиокнига 3. Журнал");
                        string itemType = Console.ReadLine();

                        Console.Write("Введите название: ");
                        string title = Console.ReadLine();
                        Console.Write("Введите автора/редактора: ");
                        string authorOrEditor = Console.ReadLine();
                        int id = library.Items.Count + 1;

                        switch (itemType)
                        {
                            case "1":
                                Console.Write("Введите размер файла (в MB): ");
                                int fileSize = int.Parse(Console.ReadLine());
                                library.AddItem(id, new EBook(title, authorOrEditor, fileSize));
                                break;
                            case "2":
                                Console.Write("Введите длительность аудиокниги (в часах): ");
                                int duration = int.Parse(Console.ReadLine());
                                library.AddItem(id, new AudioBook(title, authorOrEditor, TimeSpan.FromHours(duration)));
                                break;
                            case "3":
                                library.AddItem(id, new Magazine(title, authorOrEditor));
                                break;
                        }
                        break;

                    case "2":
                        Console.Write("Введите имя читателя: ");
                        string name = Console.ReadLine();
                        library.RegisterMember(new Member(name));
                        break;

                    case "3":
                        Console.Write("Введите ID предмета: ");
                        int itemId = int.Parse(Console.ReadLine());
                        Console.Write("Введите имя читателя: ");
                        string borrower = Console.ReadLine();

                        LibraryUser user = library.Members.Find(m => m.Name == borrower);
                        if (user != null)
                        {
                            ILibraryItem itemToBorrow = library[itemId];
                            if (itemToBorrow != null)
                            {
                                user.BorrowItem(itemToBorrow);
                            }
                            else
                            {
                                Console.WriteLine("Предмет с таким ID не найден.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Читатель не зарегистрирован.");
                        }
                        break;

                    case "4":
                        Console.Write("Введите ID предмета, который хотите вернуть: ");
                        int returnId = int.Parse(Console.ReadLine());
                        Console.Write("Введите имя читателя: ");
                        string returner = Console.ReadLine();

                        LibraryUser returningUser = library.Members.Find(m => m.Name == returner);
                        if (returningUser != null)
                        {
                            ILibraryItem itemToReturn = library[returnId];
                            if (itemToReturn != null)
                            {
                                returningUser.ReturnItem(itemToReturn);
                            }
                            else
                            {
                                Console.WriteLine("Предмет с таким ID не найден.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Читатель не зарегистрирован.");
                        }
                        break;

                    case "5":
                        library.DisplayAllItems();
                        break;

                    case "6":
                        return;

                    default:
                        Console.WriteLine("Некорректный ввод. Попробуйте еще раз.");
                        break;
                }
            }
        }
    }
}
