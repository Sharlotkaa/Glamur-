using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Serialization;

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
    [Serializable]
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

        public string GetTitle() => Title;
        public string GetAuthor() => Author;

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

        public void Return() => IsAvailable = true;
    }

    // Класс для электронной книги с расширенным функционалом
    [Serializable]
    public class EBook : Book
    {
        public int FileSize { get; private set; } // в МБ
        public string FilePath { get; private set; }

        public EBook(string title, string author, int fileSize, string filePath)
            : base(title, author)
        {
            FileSize = fileSize;
            FilePath = filePath;
            EnsureDirectoryExists(Path.GetDirectoryName(filePath)); // Убедиться, что директория существует
        }

        // Изменение расширения файла
        public void ChangeFileExtension(string newExtension)
        {
            FilePath = Path.ChangeExtension(FilePath, newExtension);
            Console.WriteLine($"Новое расширение файла: {FilePath}");
        }

        // Запись данных в файл
        public void WriteData(string data)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(data);
                Console.WriteLine($"Данные записаны в файл: {FilePath}");
            }
        }

        // Чтение данных из файла
        public void ReadData()
        {
            if (File.Exists(FilePath))
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs))
                {
                    string data = reader.ReadToEnd();
                    Console.WriteLine($"Содержимое файла {FilePath}:\n{data}");
                }
            }
            else
            {
                Console.WriteLine("Файл не найден.");
            }
        }

        // Копирование файла
        public void CopyFile()
        {
            string directory = Path.GetDirectoryName(FilePath);
            string newFileName = Path.GetFileNameWithoutExtension(FilePath) + "_copy" + Path.GetExtension(FilePath);
            string newFilePath = Path.Combine(directory, newFileName);

            File.Copy(FilePath, newFilePath);
            Console.WriteLine($"Копия книги создана: {newFilePath}");

            string originalHash = CalculateMD5Hash();
            string copiedHash = new EBook(Path.GetFileNameWithoutExtension(newFileName), Author, FileSize, newFilePath).CalculateMD5Hash();

            Console.WriteLine($"MD5-хеш оригинального файла: {originalHash}");
            Console.WriteLine($"MD5-хеш копии файла: {copiedHash}");
        }

        // Удаление файла
        public void DeleteFile()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                Console.WriteLine($"Файл {FilePath} удалён.");
                FilePath = null;
            }
            else
            {
                Console.WriteLine("Файл не найден, удаление невозможно.");
            }
        }

        // Проверка существования файла
        public bool FileExists()
        {
            bool exists = File.Exists(FilePath);
            Console.WriteLine($"Файл {FilePath} существует: {exists}");
            return exists;
        }

        public string CalculateMD5Hash()
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(FilePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Console.WriteLine($"Создана директория: {directoryPath}");
            }
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[E-Book] Название: {Title}, Автор: {Author}, Доступна: {IsAvailable}, Размер файла: {FileSize} MB, Путь: {FilePath}");
        }
    }

    // Класс аудиокниги
    public class AudioBook : Book
    {
        public TimeSpan Duration { get; private set; }
        public string TempFilePath { get; private set; }

        public AudioBook(string title, string author, TimeSpan duration)
            : base(title, author)
        {
            Duration = duration;
            TempFilePath = Path.GetTempFileName(); // Создаем временный файл
            Console.WriteLine($"Временный файл для аудиокниги: {TempFilePath}");
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[AudioBook] Название: {Title}, Автор: {Author}, Доступна: {IsAvailable}, Длительность: {Duration}, Временный файл: {TempFilePath}");
        }
    }

    // Класс журнала
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

        public string GetTitle() => Title;
        public string GetAuthor() => Editor;

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

        public void Return() => IsAvailable = true;
    }

    // Абстрактный класс для пользователей библиотеки
    public abstract class LibraryUser
    {
        public string Name { get; private set; }
        protected Queue<ILibraryItem> BorrowedItems { get; set; }

        public LibraryUser(string name)
        {
            Name = name;
            BorrowedItems = new Queue<ILibraryItem>();
        }

        // Метод для взятия предмета
        public void BorrowItem(ILibraryItem item)
        {
            if (item.IsAvailable)
            {
                BorrowedItems.Enqueue(item); // Добавляем предмет в очередь
                item.Borrow();
                Console.WriteLine($"{Name} взял(а) предмет: {item.GetTitle()}");
            }
            else
            {
                Console.WriteLine($"Предмет {item.GetTitle()} недоступен.");
            }
        }

        // Метод для возврата предмета
        public void ReturnItem()
        {
            if (BorrowedItems.Count > 0)
            {
                var item = BorrowedItems.Dequeue(); // Убираем предмет из очереди (первый вошел, первый вышел)
                item.Return();
                Console.WriteLine($"{Name} вернул(а) предмет: {item.GetTitle()}");
            }
            else
            {
                Console.WriteLine($"{Name} не имеет взятых предметов.");
            }
        }

        public abstract void DisplayUserInfo();
    }

    // Класс обычного члена библиотеки
    public class Member : LibraryUser
    {
        public Member(string name) : base(name) { }

        public override void DisplayUserInfo()
        {
            Console.WriteLine($"Читатель: {Name}, обычный член библиотеки, взятые предметы: {BorrowedItems.Count}");
        }
    }

    // Класс премиум-члена библиотеки
    public class PremiumMember : LibraryUser
    {
        public DateTime MembershipExpirationDate { get; private set; }

        public PremiumMember(string name, DateTime membershipExpirationDate) : base(name)
        {
            MembershipExpirationDate = membershipExpirationDate;
        }

        public override void DisplayUserInfo()
        {
            Console.WriteLine($"Читатель: {Name}, премиум-член (срок действия до {MembershipExpirationDate.ToShortDateString()}), взятые предметы: {BorrowedItems.Count}");
        }
    }

    // Базовый класс библиотеки
    public abstract class LibraryBase
    {
        protected List<ILibraryItem> Items { get; set; }
        protected List<LibraryUser> Members { get; set; }

        // Публичное свойство для доступа к списку пользователей библиотеки
        public List<LibraryUser> LibraryMembers
        {
            get { return Members; }
        }

        public LibraryBase()
        {
            Items = new List<ILibraryItem>();
            Members = new List<LibraryUser>();
        }

        // Использование params для добавления нескольких предметов
        public void AddItems(params ILibraryItem[] items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
                Console.WriteLine($"Добавлен предмет: {item.GetTitle()}");
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
                item.DisplayInfo();
            }
        }

        public ILibraryItem FindItemByTitle(string title)
        {
            return Items.Find(item => item.GetTitle().Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public abstract void AdditionalLibraryFunctionality();
    }

    public class Library : LibraryBase
    {
        private List<EBook> eBooks;
        public Library() : base() { eBooks = new List<EBook>(); }

        public override void AdditionalLibraryFunctionality()
        {
            // Специфичная функциональность для библиотеки
        }
        // Информация о логических дисках
        public void DisplayDriveInfo()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            Console.WriteLine("Информация о логических дисках системы:");
            
            foreach (DriveInfo drive in drives)
            {
                Console.WriteLine($"Диск: {drive.Name}");
                Console.WriteLine($"  Формат: {drive.DriveFormat}");
                Console.WriteLine($"  Общий размер: {drive.TotalSize / (1024 * 1024 * 1024)} ГБ");
                Console.WriteLine($"  Свободное место: {drive.TotalFreeSpace / (1024 * 1024 * 1024)} ГБ");
                Console.WriteLine($"  Корневая директория: {drive.RootDirectory}");
                Console.WriteLine();
            }
        }
    }
class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();
            library.DisplayDriveInfo();
            library.AddItems(
                new EBook("The Master and Margarita", "Mikhail Bulgakov", 300, "books/master_margarita.txt"),
                new EBook("War and Peace", "Lev Tolstoy", 400, "books/war_peace.txt"),
                new AudioBook("The Lord of the Rings", "J. R. R. Tolkien", TimeSpan.FromHours(6)),
                new AudioBook("Harry Potter", "J.K. Rowling", TimeSpan.FromHours(8)),
                new Magazine("Science Monthly", "John Doe")
            );

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- Меню ---");
                Console.WriteLine("1. Зарегистрировать нового пользователя");
                Console.WriteLine("2. Показать все предметы");
                Console.WriteLine("3. Взять предмет");
                Console.WriteLine("4. Вернуть предмет");
                Console.WriteLine("5. Показать информацию о пользователе");
                Console.WriteLine("6. Изменить расширение файла электронной книги");
                Console.WriteLine("7. Записать данные в файл электронной книги");
                Console.WriteLine("8. Прочитать данные из файла электронной книги");
                Console.WriteLine("9. Копировать электронную книгу");
                Console.WriteLine("10. Удалить электронную книгу");
                Console.WriteLine("11. Проверить наличие электронной книги");
                Console.WriteLine("12. Выход");
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterUser(library);
                        break;
                    case "2":
                        library.DisplayAllItems();
                        break;
                    case "3":
                        BorrowItem(library);
                        break;
                    case "4":
                        ReturnItem(library);
                        break;
                    case "5":
                        DisplayUserInfo(library);
                        break;
                    case "6":
                        ChangeEBookExtension(library);
                        break;
                    case "7":
                        WriteEBookData(library);
                        break;
                    case "8":
                        ReadEBookData(library);
                        break;
                    case "9":
                        CopyEBookFile(library);
                        break;
                    case "10":
                        DeleteEBookFile(library);
                        break;
                    case "11":
                        CheckEBookFileExists(library);
                        break;
                    case "12":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }

            Console.WriteLine("Спасибо за использование библиотеки!");
        }

        static void RegisterUser(Library library)
        {
            Console.Write("Введите имя нового пользователя: ");
            string name = Console.ReadLine();
            Console.WriteLine("Выберите тип пользователя: 1 - Обычный, 2 - Премиум");
            string userType = Console.ReadLine();

            if (userType == "1")
            {
                library.RegisterMember(new Member(name));
            }
            else if (userType == "2")
            {
                Console.Write("Введите срок действия премиум-подписки (в днях): ");
                if (int.TryParse(Console.ReadLine(), out int days))
                {
                    DateTime expirationDate = DateTime.Now.AddDays(days);
                    library.RegisterMember(new PremiumMember(name, expirationDate));
                }
                else
                {
                    Console.WriteLine("Некорректный ввод.");
                }
            }
            else
            {
                Console.WriteLine("Некорректный выбор.");
            }
        }

        static void BorrowItem(Library library)
        {
            Console.Write("Введите имя пользователя, который берет предмет: ");
            string borrowerName = Console.ReadLine();
            LibraryUser borrower = library.LibraryMembers.Find(u => u.Name == borrowerName);

            if (borrower != null)
            {
                Console.Write("Введите название предмета: ");
                string itemTitle = Console.ReadLine();
                ILibraryItem item = library.FindItemByTitle(itemTitle);

                if (item != null)
                {
                    borrower.BorrowItem(item);
                }
                else
                {
                    Console.WriteLine("Предмет не найден.");
                }
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }
        }

        static void ReturnItem(Library library)
        {
            Console.Write("Введите имя пользователя, который возвращает предмет: ");
            string returnerName = Console.ReadLine();
            LibraryUser returner = library.LibraryMembers.Find(u => u.Name == returnerName);

            if (returner != null)
            {
                returner.ReturnItem();
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }
        }

        static void DisplayUserInfo(Library library)
        {
            Console.Write("Введите имя пользователя: ");
            string userInfoName = Console.ReadLine();
            LibraryUser userInfo = library.LibraryMembers.Find(u => u.Name == userInfoName);

            if (userInfo != null)
            {
                userInfo.DisplayUserInfo();
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }
        }

        static void ChangeEBookExtension(Library library)
        {
            Console.Write("Введите название электронной книги: ");
            string ebookTitle = Console.ReadLine();
            EBook ebook = library.FindItemByTitle(ebookTitle) as EBook;

            if (ebook != null)
            {
                Console.Write("Введите новое расширение (например, .epub): ");
                string newExtension = Console.ReadLine();
                ebook.ChangeFileExtension(newExtension);
            }
            else
            {
                Console.WriteLine("Электронная книга не найдена.");
            }
        }

        static void WriteEBookData(Library library)
        {
            Console.Write("Введите название электронной книги: ");
            string ebookTitle = Console.ReadLine();
            EBook ebook = library.FindItemByTitle(ebookTitle) as EBook;

            if (ebook != null)
            {
                Console.Write("Введите данные для записи в файл: ");
                string data = Console.ReadLine();
                ebook.WriteData(data);
            }
            else
            {
                Console.WriteLine("Электронная книга не найдена.");
            }
        }

        static void ReadEBookData(Library library)
        {
            Console.Write("Введите название электронной книги: ");
            string ebookTitle = Console.ReadLine();
            EBook ebook = library.FindItemByTitle(ebookTitle) as EBook;

            if (ebook != null)
            {
                ebook.ReadData();
            }
            else
            {
                Console.WriteLine("Электронная книга не найдена.");
            }
        }

        static void CopyEBookFile(Library library)
        {
            Console.Write("Введите название электронной книги для копирования: ");
            string ebookTitle = Console.ReadLine();
            EBook ebook = library.FindItemByTitle(ebookTitle) as EBook;

            if (ebook != null)
            {
                ebook.CopyFile();
            }
            else
            {
                Console.WriteLine("Электронная книга не найдена.");
            }
        }

        static void DeleteEBookFile(Library library)
        {
            Console.Write("Введите название электронной книги для удаления: ");
            string ebookTitle = Console.ReadLine();
            EBook ebook = library.FindItemByTitle(ebookTitle) as EBook;

            if (ebook != null)
            {
                ebook.DeleteFile();
            }
            else
            {
                Console.WriteLine("Электронная книга не найдена.");
            }
        }

        static void CheckEBookFileExists(Library library)
        {
            Console.Write("Введите название электронной книги для проверки: ");
            string ebookTitle = Console.ReadLine();
            EBook ebook = library.FindItemByTitle(ebookTitle) as EBook;

            if (ebook != null)
            {
                ebook.FileExists();
            }
            else
            {
                Console.WriteLine("Электронная книга не найдена.");
            }
        }
    }
}
