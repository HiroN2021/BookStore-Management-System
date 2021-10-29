using System;
using System.Linq;
using System.Text;
using Persistence;
using BL;
using System.Collections.Generic;

namespace ConsoleAppPL
{
    using static ConsoleHelper;
    class Program
    {
        static Employee employee;
        static int pageWidth = 133;
        static void Main(string[] args)
        {
            Console.OutputEncoding = Console.InputEncoding = Encoding.UTF8;
            Console.Clear();
            do
            {
                Login();
            } while (MainMenu() != 0);
        }
        static int Login()
        {
            employee = null;
            EmployeeBL EmployeeBL = new EmployeeBL();
            do
            {
                Console.Clear();
                PrintHeader("Please Login");
                Console.WriteLine();
                // userName = "cashier1";
                Console.Write("User Name: ");
                string userName = Console.ReadLine();
                // password = "test";
                Console.Write("Password: ");
                string password = GetPassword();
                Console.WriteLine();
                // Validdate username & password
                employee = EmployeeBL.GetEmployee(userName, password, out Exception ex);
                if (ex != null)
                {
                    employee = null;
                    // Unable to connect to any of the specified MySQL hosts
                    if (ex.Message.Contains("Unable to connect"))
                        Wait("Unable to connect to database!", ConsoleColor.Red);
                    else
                        Wait(ex.ToString(), ConsoleColor.Red);
                }
                else
                {
                    if (employee != null)
                        Wait("Login success!", ConsoleColor.Green);
                    else
                        Wait("UserName or Password is not correct.", ConsoleColor.Red);
                }
            } while (employee == null);
            Console.Clear();
            // Console.WriteLine($"Welcome Employee: {employee.FirstName}\n");
            return 1;
        }
        static string GetPassword()
        {
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            return pass;
        }
        static int MainMenu()
        {
            int status = 0;
            int choice;
            do
            {
                PrintHeader("Main Menu");
                Console.WriteLine();
                string[] menu = { "Search Book", "Create Invoice", "Exit" };
                for (int i = 0; i < menu.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {menu[i]}");
                }
                Console.WriteLine();
                Console.Write("Your choice (number 1 to {0}): ", menu.Length);
                Int32.TryParse(Console.ReadLine(), out choice);
                switch (choice)
                {
                    case 1:
                        status = SearchBook();
                        break;
                    case 2:
                        status = CreateInvoice();
                        break;
                    case 3:
                        Wait("You choose exit!", ConsoleColor.DarkRed);
                        break;
                    default:
                        Wait("Invalid choice!", ConsoleColor.Red);
                        break;
                }
                if (status <= -100)
                    return status;
                Console.Clear();
            } while (choice != 3);
            return 0;
        }

        private static int CreateInvoice()
        {
            int status = 0;
            char choice;
            Console.Clear();
            PrintHeader("Create Invoice");
            Console.WriteLine();

            Customer customer = GetNewCustomer();

            Console.WriteLine();
            choice = GetChoice("Do you want to continue (y - YES, n - NO): ", 'y', 'n');
            if (choice == 'n')
            {
                PrintLine("Invoice was discarded!", ConsoleColor.Red);
                return 0;
            }
            Console.WriteLine();
            PrintLine("Input books to buy: \n", ConsoleColor.Yellow);
            var invoiceDetails = new List<InvoiceDetail>();
            BookBL bookBL = new BookBL();
            uint totalAmount = 0;
            do
            {
                string isbn = GetString("Input ISBN10 or ISBN13: ");
                Console.WriteLine();
                Book book = bookBL.GetBookByISBN(isbn, out Exception ex);
                if (book == null)
                {
                    PrintLine("Book does not exist!", ConsoleColor.DarkGray);
                }
                else
                {
                    ShowBookInfo(book);
                    Console.WriteLine();
                    if (book.Status != BookStatus.Available)
                    {
                        PrintLine("Book is not available (see book detail)!", ConsoleColor.DarkRed);
                    }
                    else if (book.Quantity == 0)
                    {
                        PrintLine("Book is out of stock!", ConsoleColor.DarkRed);
                    }
                    else if (invoiceDetails.Find(o => o.Book_Info.BookID == book.BookID) != null)
                    {
                        var od = invoiceDetails.Find(o => o.Book_Info.BookID == book.BookID);
                        PrintLine($"Already added {od.ItemQuantity} of this book to cart", ConsoleColor.Blue);
                        uint itemQuantity = GetUInt32("Input number of book to buy more or input 0 to skip the book: ");
                        if (itemQuantity > 0)
                        {
                            while (itemQuantity + od.ItemQuantity > book.Quantity)
                            {
                                PrintLine($"Book has {book.Quantity} item available, but you want to add {itemQuantity} more book so total book to buy = ({od.ItemQuantity} = {od.ItemQuantity + itemQuantity}) > book's quantity in stock", ConsoleColor.Red);
                                PrintLine($"Already added {od.ItemQuantity} items of this book to cart", ConsoleColor.Blue);
                                itemQuantity = GetUInt32("Input number of book to buy more or input 0 to skip the book: ");
                            }
                            if (itemQuantity == 0)
                            {
                                PrintLine("Book is skipped!", ConsoleColor.DarkRed);
                            }
                            else
                            {
                                od.ItemQuantity += itemQuantity;
                                od.Amount = (uint)book.Price.Value * od.ItemQuantity;
                                totalAmount += (uint)book.Price.Value * itemQuantity;
                            }
                        }
                        else
                        {
                            PrintLine("Book is skipped!", ConsoleColor.DarkRed);
                        }
                    }
                    else
                    {
                        uint itemQuantity = GetUInt32("Input number of book to buy or input 0 to skip the book: ");
                        if (itemQuantity > 0)
                        {
                            while (itemQuantity > book.Quantity)
                            {
                                PrintLine($"Book has {book.Quantity} item available, but you input {itemQuantity}", ConsoleColor.DarkRed);
                                itemQuantity = GetUInt32("Input number of book to buy or input 0 to skip the book: ");
                            }
                            if (itemQuantity == 0)
                            {
                                PrintLine("Book is skipped!", ConsoleColor.DarkRed);
                            }
                            else
                            {
                                totalAmount += (uint)book.Price.Value * itemQuantity;
                                invoiceDetails.Add(new InvoiceDetail() { Book_Info = book, ItemQuantity = itemQuantity, Amount = (uint)book.Price.Value * itemQuantity });
                            }
                        }
                        else
                        {
                            PrintLine("Book is skipped!", ConsoleColor.DarkRed);
                        }
                    }
                }
                Console.WriteLine();
                choice = GetChoice("Add more books (y - YES, n - NO): ", 'y', 'n');
                Console.WriteLine();
            } while (choice == 'y');
            if (invoiceDetails.Count == 0)
            {
                PrintLine("Invoice was discarded beacause no book is addded!", ConsoleColor.DarkRed);
                Wait();
                return 0;
            }
            // 
            PrintHeader("CREATE INVOICE");
            Console.WriteLine();

            // PRINT CUSTOMER DETAILS
            PrintCustomerInfo(customer);
            Console.WriteLine("|{0}|", new string('#', pageWidth - 2));
            // PRINT INVOICE DETAILS
            PrintInvoiceDetails(invoiceDetails);

            Console.WriteLine("\nTotal amount: {0:0,0} VND", invoiceDetails.Sum(x => x.Amount));
            Console.WriteLine();
            choice = GetChoice("Do you want to commit payment (y - YES, n - NO): ", 'y', 'n');
            if (choice == 'y')
            {
                Invoice invoice = new Invoice() { Customer_Info = customer, Employee_Info = employee, CreatedTime = DateTime.Now, InvoiceDetails = invoiceDetails };
                InvoiceBL invoiceBL = new InvoiceBL();
                if (invoiceBL.AddToDataBase(invoice, out Exception ex))
                {
                    PrintHeader("CREATE INVOICE");
                    Console.WriteLine();
                    PrintStoreInfo();
                    PrintLine($"InvoiceID: {invoice.InvoiceID}".CenterLine(pageWidth), ConsoleColor.Yellow);
                    PrintCustomerInfo(customer);
                    Console.WriteLine("|{0}|", new string('#', pageWidth - 2));
                    PrintInvoiceDetails(invoiceDetails);
                    Console.WriteLine($"Date Created: {invoice.CreatedTime.ToShortDateString()}".CenterLine(pageWidth));
                    Console.WriteLine("\nTotal amount: {0:0,0} VND", invoiceDetails.Sum(x => x.Amount));
                    Console.WriteLine();
                    PrintLine("Invoice created successfully!", ConsoleColor.Green);
                }
                else
                {
                    PrintLine("An error was ocurred. Invoice was discarded!", ConsoleColor.Red);
                }
                Wait();
            }
            else
            {
                Wait("Invoice was discarded!", ConsoleColor.Red);
            }

            return status;
        }
        static Customer GetNewCustomer()
        {
            Customer customer = new Customer();
            var lf = "{0,41} : ";
            PrintLine("Input customer information: \n", ConsoleColor.Yellow);
            customer.FirstName = GetStringTrim(string.Format(lf, "First Name"));
            customer.LastName = GetString(string.Format(lf, "Last Name"));
            customer.ContactTitle = GetStringOrNull(string.Format(lf, "Contact Title"));
            char choice = GetChoice(string.Format(lf, "Gender (m - male, f - female, u - unknow)"), 'm', 'f', 'u');
            string gender = choice switch
            {
                'm' => "male",
                'f' => "female",
                'u' => "unknow",
                _ => null
            };
            customer.Gender = Enum.Parse<PersonGender>(gender, true);
            customer.Address = GetStringOrNull(string.Format(lf, "Address"));
            customer.City = GetStringOrNull(string.Format(lf, "City"));
            customer.Phone = GetStringOrNull(string.Format(lf, "Phone"));
            customer.Fax = GetStringOrNull(string.Format(lf, "Fax"));
            customer.Email = GetStringOrNull(string.Format(lf, "Email"));
            customer.Note = GetStringOrNull(string.Format(lf, "Note"));
            return customer;
        }
        static void PrintStoreInfo()
        {
            Console.WriteLine(@"BookStoreVTC");
            Console.WriteLine("VTC Online Building, 18 Đường Tam Trinh, Mai Động, Hai Bà Trưng, Hà Nội");
            Console.WriteLine("Phone:");
            Console.WriteLine("Fax:");
        }
        static void PrintInvoiceDetails(List<InvoiceDetail> invoiceDetails)
        {
            List<List<string>> rowsContents = new List<List<string>>()
            {
                new List<string>(){"Index", "Book Title", "Unit", "Price (VND)", "Quantity", "Amount (VND)"},
            };
            List<int> columnsFormat = new List<int>() { 5, 0, 6, 12, 8, 12 };
            columnsFormat[1] = -(pageWidth - columnsFormat.Sum<int>(x => Math.Abs(x)) - columnsFormat.Count * 3 - 1);
            for (int i = 0; i < invoiceDetails.Count; i++)
            {
                Book book = invoiceDetails[i].Book_Info;
                rowsContents.Add(new List<string>()
                {
                    (i+1).ToString(),  book.Title, "book", string.Format("{0:0,0}", book.Price),
                    invoiceDetails[i].ItemQuantity.ToString(),
                    string.Format("{0:0,0}",book.Price * invoiceDetails[i].ItemQuantity)
                });
            }
            PrintTable(rowsContents, columnsFormat, "INVOICE DETAILS");
        }
        static void PrintCustomerInfo(Customer customer)
        {
            List<List<string>> rowsContents = new List<List<string>>()
            {
                new List<string>(){"FIRST NAME", customer.FirstName + ""},
                new List<string>(){"LAST NAME", customer.LastName + ""},
                new List<string>(){"CONTACT TITLE", customer.ContactTitle + ""},
                new List<string>(){"GENDER", customer.Gender.ToString() + ""},
                new List<string>(){"ADDRESS", customer.Address + ""},
                new List<string>(){"CITY", customer.City + ""},
                new List<string>(){"PHONE", customer.Phone + ""},
                new List<string>(){"FAX", customer.Fax + ""},
                new List<string>(){"EMAIL", customer.Email + ""},
                new List<string>(){"NOTE", customer.Note + ""},
            };
            PrintTable(rowsContents, new List<int>() { -20, -(pageWidth - 27) }, "CUSTOMER INFO", false);
        }
        static int SearchBook()
        {
            int status = 0;
            int choice;
            string[] menu = { "Search by ISBN", "Search by TITLE", "Search by CATEGORY", "Search by AUTHOR", "Search by PUBLISHER", "Return to Main Menu" };
            do
            {
                Console.Clear();
                PrintHeader("Search Book");
                Console.WriteLine();
                for (int i = 0; i < menu.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {menu[i]}");
                }
                Console.Write("\nYour choice (number 1 to {0}): ", menu.Length);
                Int32.TryParse(Console.ReadLine(), out choice);
                switch (choice)
                {
                    case 1:
                        status = SearchBookByISBN();
                        break;
                    case 2:
                        status = SearchBookByTitle();
                        break;
                    case 3:
                        status = SearchBookByCategory();
                        break;
                    case 4:
                        status = SearchBookByAuthor();
                        break;
                    case 5:
                        status = SearchBookByPublisher();
                        break;
                    case 6:
                        break;
                    default:
                        Wait("Invalid choice!");
                        break;
                }
                if (status <= -100)
                    return status;
                Console.Clear();
            } while (choice != menu.Length);
            return 0;
        }
        private static int SearchBookByISBN()
        {
            int status = 0;
            Console.Clear();
            PrintHeader("Search Book by ISBN");
            string isbn = GetString("Input ISBN10 or ISBN13: ");
            BookBL bookBL = new BookBL();
            Book book = bookBL.GetBookByISBN(isbn, out Exception e);
            Console.WriteLine();
            if (book == null)
            {
                PrintLine("Found nothing.", ConsoleColor.DarkGray);
            }
            else
                ShowBookInfo(book);
            Wait();
            return status;
        }
        private static int SearchBookByCategory()
        {
            Category categoryToFind = null;
            int status = 0;
            char choice;
            CategoryBL categoryBL = new CategoryBL();
            PrintHeader("Search Book by Category");
            string pattern = GetString("\nInput category to find: ");
            List<Category> foundCategories = categoryBL.FindCategories(pattern, out Exception ex);
            Console.WriteLine();
            if (foundCategories.Count == 0)
            {
                PrintLine("Found nothing.", ConsoleColor.DarkGray);
            }
            else
            {
                int maxNumberOfItemsPerPage = 10;
                int pageNumber = 1;
                char button;
                do
                {
                    PrintHeader("Search Book by category");
                    Console.WriteLine();
                    PrintLine("Categories filter: " + pattern, ConsoleColor.Yellow);
                    Console.WriteLine();
                    List<List<string>> rowsContents = new List<List<string>>() { new List<string>() { "Index", "Category Name" } };
                    List<int> columnsFormat = new List<int>() { 5, -70 };
                    for (int i = (pageNumber - 1) * maxNumberOfItemsPerPage; i < Math.Min(pageNumber * maxNumberOfItemsPerPage, foundCategories.Count); i++)
                    {
                        Category category = foundCategories[i];
                        rowsContents.Add(new List<string>() { (i + 1).ToString(), category.CategoryName });
                    }
                    Console.WriteLine("Page {0,3}/{1,3}", pageNumber, Math.Ceiling((double)foundCategories.Count / maxNumberOfItemsPerPage));
                    PrintTable(rowsContents, columnsFormat);
                    Console.WriteLine("Previous page: {0} | Next page: {1}", "<", ">");
                    Console.WriteLine("Press Enter key to continue!");
                    button = Console.ReadKey(true).KeyChar;
                    if ((button == '<' || button == ',') && pageNumber > 1)
                    {
                        pageNumber--;
                    }
                    if ((button == '>' || button == '.') && pageNumber * maxNumberOfItemsPerPage < foundCategories.Count)
                    {
                        pageNumber++;
                    }
                } while (button != ((char)ConsoleKey.Enter));
                Console.WriteLine();
                uint index = GetUInt32("Input category index: ");
                Console.WriteLine();
                if (index > 0 && index <= foundCategories.Count)
                {
                    categoryToFind = foundCategories[(int)(index - 1)];
                    do
                    {
                        PrintHeader("Search Book by category");
                        Console.WriteLine();
                        PrintLine("Categories filter: " + categoryToFind.CategoryName, ConsoleColor.Yellow);
                        Console.WriteLine();
                        BookBL bookBL = new BookBL();
                        var foundBooks = new List<Book>();
                        foundBooks = bookBL.FindBooksByCategory(categoryToFind.CategoryID, out ex);
                        if (foundBooks == null || foundBooks.Count == 0)
                        {
                            Print("No book found!", ConsoleColor.DarkGray);
                            break;
                        }
                        pageNumber = 1;
                        do
                        {
                            PrintHeader("Search Book by category");
                            Console.WriteLine();
                            PrintLine("Categories filter: " + categoryToFind.CategoryName, ConsoleColor.Yellow);
                            Console.WriteLine();
                            button = PrintListOfBooks(foundBooks, ref pageNumber, maxNumberOfItemsPerPage);
                        } while (button != ((char)ConsoleKey.Enter));
                        Console.WriteLine();
                        index = GetUInt32("Input book index: ");
                        Console.WriteLine();
                        if (index > 0 && index <= foundBooks.Count)
                        {
                            Book book = foundBooks[(int)index - 1];
                            ShowBookInfo(book);
                        }
                        else
                            PrintLine("Invalid Book index!", ConsoleColor.Red);
                        Console.WriteLine();
                        choice = GetChoice("Choose another book (y - YES, n - NO): ", 'y', 'n');
                    } while (choice == 'y');
                }
                else
                    PrintLine("Invalid index!", ConsoleColor.Red);
            }
            return status;
        }
        private static int SearchBookByAuthor()
        {
            Author authorToFind = null;
            int status = 0;
            char choice;
            AuthorBL authorBL = new AuthorBL();
            PrintHeader("Search Book by author");
            Console.WriteLine();
            string pattern = GetString("Search for Author's name: ");
            List<Author> foundAuthors = authorBL.FindAuthorByPatternMatching(pattern, out Exception ex);
            Console.WriteLine();
            if (foundAuthors.Count == 0)
                PrintLine("Found nothing.", ConsoleColor.DarkGray);
            else
            {
                int maxNumberOfItemsPerPage = 10;
                int pageNumber = 1;
                char button;
                do
                {
                    PrintHeader("Search Book by author");
                    Console.WriteLine();
                    PrintLine("Search for Author: " + pattern, ConsoleColor.Yellow);
                    Console.WriteLine();
                    Console.WriteLine("Page {0,3}/{1,3}", pageNumber, Math.Ceiling((double)foundAuthors.Count / maxNumberOfItemsPerPage));
                    List<List<string>> rowsContents = new List<List<string>>() { new List<string>() { "Index", "Author Name" } };
                    List<int> columnsFormat = new List<int>() { 5, -50 };
                    for (int i = (pageNumber - 1) * maxNumberOfItemsPerPage; i < Math.Min(pageNumber * maxNumberOfItemsPerPage, foundAuthors.Count); i++)
                    {
                        Author author = foundAuthors[i];
                        rowsContents.Add(new List<string>() { (i + 1).ToString(), author.AuthorName });
                    }
                    PrintTable(rowsContents, columnsFormat);
                    Console.WriteLine("Previous page: {0} | Next page: {1}", "<", ">");
                    Console.WriteLine("Press Enter key to continue!");
                    button = Console.ReadKey(true).KeyChar;
                    if ((button == '<' || button == ',') && pageNumber > 1)
                    {
                        pageNumber--;
                    }
                    if ((button == '>' || button == '.') && pageNumber * maxNumberOfItemsPerPage < foundAuthors.Count)
                    {
                        pageNumber++;
                    }
                } while (button != ((char)ConsoleKey.Enter));
                Console.WriteLine();
                uint index = GetUInt32("Input author index: ");
                Console.WriteLine();
                if (index > 0 && index <= foundAuthors.Count)
                {
                    authorToFind = foundAuthors[(int)(index - 1)];
                    do
                    {
                        PrintHeader("Search Book by author");
                        Console.WriteLine();
                        PrintLine("Author's name: " + authorToFind.AuthorName, ConsoleColor.Yellow);
                        Console.WriteLine();
                        BookBL bookBL = new BookBL();
                        var foundBooks = new List<Book>();
                        foundBooks = bookBL.FindBooksByAuthor(authorToFind.AuthorID, out ex);
                        if (foundBooks == null || foundBooks.Count == 0)
                        {
                            Print("No book found!", ConsoleColor.DarkGray);
                            break;
                        }
                        pageNumber = 1;
                        do
                        {
                            PrintHeader("Search Book by author");
                            Console.WriteLine();
                            PrintLine("Author's name: " + authorToFind.AuthorName, ConsoleColor.Yellow);
                            Console.WriteLine();
                            button = PrintListOfBooks(foundBooks, ref pageNumber, maxNumberOfItemsPerPage);
                        } while (button != ((char)ConsoleKey.Enter));
                        Console.WriteLine();
                        index = GetUInt32("Input book index: ");
                        Console.WriteLine();
                        if (index > 0 && index <= foundBooks.Count)
                        {
                            Book book = foundBooks[(int)index - 1];
                            ShowBookInfo(book);
                        }
                        else
                            PrintLine("Invalid Book index!", ConsoleColor.Red);
                        Console.WriteLine();
                        choice = GetChoice("Choose another book (y - YES, n - NO): ", 'y', 'n');
                    } while (choice == 'y');
                }
                else
                    PrintLine("Invalid index!", ConsoleColor.Red);
            }
            return status;
        }
        private static int SearchBookByPublisher()
        {
            Publisher publisherToFind = null;
            int status = 0;
            char choice;
            PublisherBL publisherBL = new PublisherBL();
            PrintHeader("Search Book by publisher");
            Console.WriteLine();
            string pattern = GetString("Search for Publisher's name: ");
            List<Publisher> foundPublishers = publisherBL.FindPublisherByPatternMatching(pattern, out Exception ex);
            Console.WriteLine();
            if (foundPublishers.Count == 0)
                PrintLine("Found nothing.", ConsoleColor.DarkGray);
            else
            {
                int maxNumberOfItemsPerPage = 10;
                int pageNumber = 1;
                char button;
                do
                {
                    PrintHeader("Search Book by publisher");
                    Console.WriteLine();
                    PrintLine("Search for Publisher: " + pattern, ConsoleColor.Yellow);
                    Console.WriteLine();
                    Console.WriteLine("Page {0,3}/{1,3}", pageNumber, Math.Ceiling((double)foundPublishers.Count / maxNumberOfItemsPerPage));
                    List<List<string>> rowsContents = new List<List<string>>() { new List<string>() { "Index", "Publisher Name" } };
                    List<int> columnsFormat = new List<int>() { 5, -50 };
                    for (int i = (pageNumber - 1) * maxNumberOfItemsPerPage; i < Math.Min(pageNumber * maxNumberOfItemsPerPage, foundPublishers.Count); i++)
                    {
                        Publisher publisher = foundPublishers[i];
                        rowsContents.Add(new List<string>() { (i + 1).ToString(), publisher.PublisherName });
                    }
                    PrintTable(rowsContents, columnsFormat);
                    Console.WriteLine("Previous page: {0} | Next page: {1}", "<", ">");
                    Console.WriteLine("Press Enter key to continue!");
                    button = Console.ReadKey(true).KeyChar;
                    if ((button == '<' || button == ',') && pageNumber > 1)
                    {
                        pageNumber--;
                    }
                    if ((button == '>' || button == '.') && pageNumber * maxNumberOfItemsPerPage < foundPublishers.Count)
                    {
                        pageNumber++;
                    }
                } while (button != ((char)ConsoleKey.Enter));
                Console.WriteLine();
                uint index = GetUInt32("Input publisher index: ");
                Console.WriteLine();
                if (index > 0 && index <= foundPublishers.Count)
                {
                    publisherToFind = foundPublishers[(int)(index - 1)];
                    do
                    {
                        PrintHeader("Search Book by publisher");
                        Console.WriteLine();
                        PrintLine("Publisher's name: " + publisherToFind.PublisherName, ConsoleColor.Yellow);
                        Console.WriteLine();
                        BookBL bookBL = new BookBL();
                        var foundBooks = new List<Book>();
                        foundBooks = bookBL.FindBooksByPublisher(publisherToFind.PublisherID, out ex);
                        if (foundBooks == null || foundBooks.Count == 0)
                        {
                            Print("No book found!", ConsoleColor.DarkGray);
                            Wait();
                            break;
                        }
                        pageNumber = 1;
                        do
                        {
                            PrintHeader("Search Book by publisher");
                            Console.WriteLine();
                            PrintLine("Publisher's name: " + publisherToFind.PublisherName, ConsoleColor.Yellow);
                            Console.WriteLine();
                            button = PrintListOfBooks(foundBooks, ref pageNumber, maxNumberOfItemsPerPage);
                        } while (button != ((char)ConsoleKey.Enter));
                        Console.WriteLine();
                        index = GetUInt32("Input book index: ");
                        Console.WriteLine();
                        if (index > 0 && index <= foundBooks.Count)
                        {
                            Book book = foundBooks[(int)index - 1];
                            ShowBookInfo(book);
                        }
                        else
                            PrintLine("Invalid Book index!", ConsoleColor.Red);
                        Console.WriteLine();
                        choice = GetChoice("Choose another book (y - YES, n - NO): ", 'y', 'n');
                    } while (choice == 'y');
                }
                else
                    PrintLine("Invalid index!", ConsoleColor.Red);
            }
            return status;
        }
        private static int SearchBookByTitle()
        {
            int status = 0;
            char choice;
            BookBL bookBL = new BookBL();
            PrintHeader("Search Book by Title");
            Console.WriteLine();
            string pattern = GetString("Search for Book's Title: ");
            List<Book> foundBooks = bookBL.FindBookByTitle(pattern, out Exception ex);
            Console.WriteLine();
            if (foundBooks.Count == 0)
            {
                PrintLine("Found nothing!", ConsoleColor.DarkGray);
            }
            else
            {
                int maxNumberOfBooksPerPage = 10;
                do
                {
                    int pageNumber = 1;
                    char button;
                    do
                    {
                        PrintHeader("Search Book by Title");
                        PrintLine("Search for Book's Title: " + pattern, ConsoleColor.Yellow);
                        Console.WriteLine();
                        button = PrintListOfBooks(foundBooks, ref pageNumber, maxNumberOfBooksPerPage);
                    } while (button != ((char)ConsoleKey.Enter));
                    Console.WriteLine();
                    uint index = GetUInt32("Input book index: ");
                    Console.WriteLine();
                    if (index > 0 && index <= foundBooks.Count)
                    {
                        var book = foundBooks[(int)index - 1];
                        ShowBookInfo(book);
                    }
                    else
                    {
                        PrintLine("Invalid index!", ConsoleColor.Red);
                    }
                    Console.WriteLine();
                    choice = GetChoice("Choose another book (y - YES, n - NO): ", 'y', 'n');
                } while (choice == 'y');
            }
            Console.WriteLine();
            return status;
        }
        static char PrintListOfBooks(List<Book> listOfBooks, ref int pageNumber, int maxNumberOfItemsPerPage)
        {
            char button;
            List<List<string>> rowsContents = new List<List<string>>() { new List<string>() { "Index", "Book Title", "ISBN13", "Price (VND)" } };
            List<int> columnsFormat = new List<int>() { 5, -90, -13, 12 };
            for (int i = (pageNumber - 1) * maxNumberOfItemsPerPage; i < Math.Min(pageNumber * maxNumberOfItemsPerPage, listOfBooks.Count); i++)
            {
                Book book = listOfBooks[i];
                rowsContents.Add(new List<string>() { (i + 1).ToString(), book.Title, book.ISBN13, String.Format("{0:0,0}", book.Price) });
            }
            Console.WriteLine("Page {0,3}/{1,3}", pageNumber, Math.Ceiling((double)listOfBooks.Count / maxNumberOfItemsPerPage));
            PrintTable(rowsContents, columnsFormat);
            Console.WriteLine("Goto previous page press: ',' OR '<'");
            Console.WriteLine("Goto next page press:     '.' OR '>'");
            Console.WriteLine("Press Enter key to choose item.");
            button = Console.ReadKey(true).KeyChar;
            if ((button == '<' || button == ',') && pageNumber > 1)
                pageNumber--;
            else if ((button == '>' || button == '.') && pageNumber * maxNumberOfItemsPerPage < listOfBooks.Count)
                pageNumber++;
            return button;
        }
        static void PrintHeader(string menuTitle)
        {
            string border = $" {new string('-', pageWidth - 2)} ";
            var header = new StringBuilder();
            header.AppendLine(border.CenterLine(pageWidth));
            header.AppendLine("|" + "BookStore Management Software".CenterLine(pageWidth - 2) + "|");
            header.Append(border.CenterLine(pageWidth));
            if (menuTitle != null)
            {
                header.AppendLine();
                header.Append($"[{menuTitle}]".CenterLine(pageWidth));
            }
            Console.Clear();
            Console.WriteLine(header);
        }
        static void ShowBookInfo(Book book)
        {
            List<int> columnsFormat = new List<int>() { -15, -111 };
            List<List<string>> rowsContents = new List<List<string>>()
            {
                new List<string>() { "BookID", book.BookID.ToString() },
                new List<string>() { "Title", book.Title },
                new List<string>() { "Price", string.Format("{0:0,0} VND", book.Price) },
                new List<string>() { "ISBN13", book.ISBN13 + ""},
                new List<string>() { "ISBN10", book.ISBN10 + ""},
                new List<string>() { "AuthorName", book.AuthorName + ""},
                new List<string>() { "PublisherName", book.PublisherName + ""},
                new List<string>() { "Language", book.Language + ""},
                new List<string>() { "PublicationDate", book.PublicationDate.HasValue ? book.PublicationDate.Value.ToShortDateString() : "" },
                new List<string>() { "NumberOfPages", book.NumberOfPages.Value.ToString() },
                new List<string>() { "Dimension", book.Dimensions + ""},
                new List<string>() { "Quantity", book.Quantity.ToString() },
                new List<string>() { "Status", book.Status.ToString() },
                new List<string>() { "Categories", String.Join(", ", book.CategoriesName) },
                new List<string>() { "Description", book.Description + "" }
            };
            PrintTable(rowsContents, columnsFormat, "BOOK INFO", false);
        }
    }
}
