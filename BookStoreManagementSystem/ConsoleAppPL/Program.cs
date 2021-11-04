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
        const int pageWidth = 158;
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
                PrintHeader("___  _    ____ ____ ____ ____    _    ____ ____ _ _  _ \r\n|__] |    |___ |__| [__  |___    |    |  | | __ | |\\ | \r\n|    |___ |___ |  | ___] |___    |___ |__| |__] | | \\| ");
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
                PrintHeader("_  _ ____ _ _  _    _  _ ____ _  _ _  _ \r\n|\\/| |__| | |\\ |    |\\/| |___ |\\ | |  | \r\n|  | |  | | | \\|    |  | |___ | \\| |__| ");
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
            string menuTitle = "____ ____ ____ ____ ___ ____    _ _  _ _  _ ____ _ ____ ____\r\n|    |__/ |___ |__|  |  |___    | |\\ | |  | |  | | |    |___\r\n|___ |  \\ |___ |  |  |  |___    | | \\|  \\/  |__| | |___ |___";
            PrintHeader(menuTitle);
            Console.WriteLine();
            PrintLine("Input books to buy: \n", ConsoleColor.Yellow);
            var invoiceDetails = new List<InvoiceDetail>();
            BookBL bookBL = new BookBL();
            do
            {
                uint bookIDToFind = GetUInt32("Input book ID: ");
                Console.WriteLine();
                Book bookToBuy = bookBL.GetBookByID(bookIDToFind, out Exception ex);
                if (bookToBuy == null)
                {
                    PrintLine("Book does not exist!", ConsoleColor.DarkGray);
                }
                else
                {
                    ShowBookInfo(bookToBuy);
                    Console.WriteLine();
                    if (bookToBuy.Status != BookStatus.Available)
                    {
                        PrintLine("Book is not available (see book's status)!", ConsoleColor.DarkRed);
                    }
                    else if (bookToBuy.Quantity == 0)
                    {
                        PrintLine("Book is out of stock!", ConsoleColor.DarkRed);
                    }
                    else if (invoiceDetails.Find(o => o.Book_Info.BookID == bookToBuy.BookID) != null)
                    {
                        var od = invoiceDetails.Find(o => o.Book_Info.BookID == bookToBuy.BookID);
                        PrintLine($"You already added {od.ItemQuantity} items of this book to invoice", ConsoleColor.Blue);
                        uint itemQuantity = GetUInt32("Input number of book to buy more or input 0 to skip: ");
                        if (itemQuantity > 0)
                        {
                            while (itemQuantity + od.ItemQuantity > bookToBuy.Quantity)
                            {
                                PrintLine($"Book has only {bookToBuy.Quantity} item left, but you want to add {itemQuantity} more book so total book to buy = ({od.ItemQuantity} + {itemQuantity} = {od.ItemQuantity + itemQuantity}) > {bookToBuy.Quantity}", ConsoleColor.Red);
                                PrintLine($"Already added {od.ItemQuantity} items of this book to invoice", ConsoleColor.Blue);
                                itemQuantity = GetUInt32("Input number of book to buy more or input 0 to skip: ");
                            }
                            if (itemQuantity == 0)
                            {
                                PrintLine("Book is skipped!", ConsoleColor.DarkRed);
                            }
                            else
                            {
                                od.ItemQuantity += itemQuantity;
                                od.Amount = (uint)bookToBuy.Price.Value * od.ItemQuantity;
                                if (od.Book_Info.Quantity == od.ItemQuantity) od.Book_Info.Status = BookStatus.Out_Of_Stock;
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
                            while (itemQuantity > bookToBuy.Quantity)
                            {
                                PrintLine($"Book has {bookToBuy.Quantity} item available, but you input {itemQuantity}", ConsoleColor.DarkRed);
                                itemQuantity = GetUInt32("Input number of book to buy or input 0 to skip the book: ");
                            }
                            if (itemQuantity == 0)
                            {
                                PrintLine("Book is skipped!", ConsoleColor.DarkRed);
                            }
                            else
                            {
                                if (bookToBuy.Quantity == itemQuantity) bookToBuy.Status = BookStatus.Out_Of_Stock;
                                invoiceDetails.Add(new InvoiceDetail() { Book_Info = bookToBuy, ItemQuantity = itemQuantity, Amount = (uint)bookToBuy.Price.Value * itemQuantity });
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
            PrintHeader(menuTitle);
            Console.WriteLine();
            PrintInvoiceDetails(invoiceDetails);
            // Get Customer Info
            Console.WriteLine();
            Customer customer = GetNewCustomer();
            Console.WriteLine();
            Wait();
            // 
            PrintHeader(menuTitle);
            Console.WriteLine();

            // PRINT INVOICE DETAILS
            // PrintInvoiceDetails(invoiceDetails);
            Invoice invoice = new Invoice() { Customer_Info = customer, Employee_Info = employee, CreatedTime = DateTime.Now, InvoiceDetails = invoiceDetails };
            PrintInvoice(invoice);

            Console.WriteLine();
            choice = GetChoice("Do you want to commit payment (y - YES, n - NO): ", 'y', 'n');
            if (choice == 'y')
            {
                InvoiceBL invoiceBL = new InvoiceBL();
                invoice.CreatedTime = DateTime.Now;
                if (invoiceBL.AddToDataBase(invoice, out Exception ex))
                {
                    PrintHeader(menuTitle);
                    Console.WriteLine();
                    // PrintInvoiceDetails(invoiceDetails);
                    PrintInvoice(invoice);
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
            // var lf = "{0,41} : ";
            var lf = "{0,-10} : ";
            PrintLine("Input customer information: \n", ConsoleColor.Yellow);
            customer.Phone = GetStringOrNull(string.Format(lf, "Phone"));
            Customer customerExist = null;
            if (customer.Phone != null)
            {
                CustomerBL customerBL = new CustomerBL();
                customerExist = customerBL.GetCustomerByPhone(customer.Phone, out Exception ex);
            }
            if (customerExist != null)
            {
                customer = customerExist;
                PrintLine("Customer with this phone number is already existed:", ConsoleColor.Blue);
                Console.WriteLine();
                PrintLine(string.Format("{0,-10} : {1}", "CustomerID", customer.CustomerID), ConsoleColor.Cyan);
                PrintLine(string.Format("{0,-10} : {1}", "First Name", customer.FirstName), ConsoleColor.Cyan);
                PrintLine(string.Format("{0,-10} : {1}", "Last Name", customer.LastName), ConsoleColor.Cyan);
                PrintLine(string.Format("{0,-10} : {1}", "Phone", customer.Phone), ConsoleColor.Cyan);
                Console.WriteLine();
                char choice = GetChoice("Do you want to change name (y - YES, n - NO) : ", 'y', 'n');
                if (choice == 'y')
                {
                    Console.WriteLine();
                    PrintLine("Input new name:", ConsoleColor.Yellow);
                    customer.FirstName = GetStringTrim(string.Format(lf, "First Name"));
                    customer.LastName = GetStringOrNull(string.Format(lf, "Last Name"));
                }
            }
            else
            {
                customer.FirstName = GetStringTrim(string.Format(lf, "First Name"));
                customer.LastName = GetStringOrNull(string.Format(lf, "Last Name"));
            }
            // customer.LastName = GetString(string.Format(lf, "Last Name"));
            // customer.ContactTitle = GetStringOrNull(string.Format(lf, "Contact Title"));
            // char choice = GetChoice(string.Format(lf, "Gender (m - male, f - female, u - unknow)"), 'm', 'f', 'u');
            // string gender = choice switch
            // {
            //     'm' => "male",
            //     'f' => "female",
            //     'u' => "unknow",
            //     _ => null
            // };
            // customer.Gender = Enum.Parse<PersonGender>(gender, true);
            // customer.Address = GetStringOrNull(string.Format(lf, "Address"));
            // customer.City = GetStringOrNull(string.Format(lf, "City"));
            // customer.Fax = GetStringOrNull(string.Format(lf, "Fax"));
            // customer.Email = GetStringOrNull(string.Format(lf, "Email"));
            // customer.Note = GetStringOrNull(string.Format(lf, "Note"));
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
            List<int> columnsFormat = new List<int>() { 5, 0, 4, 12, 8, 12 };
            columnsFormat[1] = -(pageWidth - columnsFormat.Sum<int>(x => Math.Abs(x)) - columnsFormat.Count * 3 - 1);
            List<List<string>> rowsContents = new List<List<string>>()
            {
                new List<string>(){"Index", "Book Title".CenterLine(-columnsFormat[1]), "Unit", "Price (VND)", "Quantity", "Amount (VND)"},
            };
            for (int i = 0; i < invoiceDetails.Count; i++)
            {
                Book book = invoiceDetails[i].Book_Info;
                rowsContents.Add(new List<string>()
                {
                    (i+1).ToString(),  book.Title + (book.ISBN13!=null? $" - ISBN13: {book.ISBN13}":book.ISBN10!=null? $" - ISBN10: {book.ISBN10}":""),
                    "book", string.Format("{0:0,0}", book.Price),
                    invoiceDetails[i].ItemQuantity.ToString(),
                    string.Format("{0:0,0}",book.Price * invoiceDetails[i].ItemQuantity)
                });
            }
            PrintTable(rowsContents, columnsFormat, "INVOICE DETAILS");
            Console.WriteLine("| {0} | {1} |", "Total".CenterLine(139), string.Format("{0:0,0}", invoiceDetails.Sum(x => x.Amount)).PadLeft(12));
            Console.WriteLine("+{0}+", new string('-', 156));
        }
        static void PrintInvoice(Invoice invoice)
        {
            List<InvoiceDetail> invoiceDetails = invoice.InvoiceDetails;
            Console.WriteLine("LMH BOOKSTORE, INC.".CenterLine(pageWidth));
            Console.WriteLine("18 Tam Trinh, Mai Đong, Hai Ba Trung, Ha Noi".CenterLine(pageWidth));
            Console.WriteLine("012-345-6789".CenterLine(pageWidth));
            Console.WriteLine();
            Console.WriteLine("INVOICE".CenterLine(pageWidth));
            Console.WriteLine();

            Console.WriteLine(string.Format("{0,-9} {1}", "Customer:", string.Join(' ', invoice.Customer_Info.FirstName, invoice.Customer_Info.LastName)).PadRight(60).CenterLine(pageWidth));
            Console.WriteLine(string.Format("{0,-9} {1}", "Phone:", invoice.Customer_Info.Phone).PadRight(60).CenterLine(pageWidth));

            List<int> columnsFormat = new List<int>() { 8, -30, 12 };
            List<List<string>> rowsContents = new List<List<string>>()
            {
                new List<string>(){"Quantity", "Product".CenterLine(-columnsFormat[1]), "Amount (VND)"},
            };
            for (int i = 0; i < invoiceDetails.Count; i++)
            {
                Book book = invoiceDetails[i].Book_Info;
                rowsContents.Add(new List<string>()
                {
                    invoiceDetails[i].ItemQuantity.ToString(),
                    book.Title + (book.ISBN13!=null? $" - ISBN13: {book.ISBN13}":book.ISBN10!=null? $" - ISBN10: {book.ISBN10}":""),
                    string.Format("{0:0,0}",book.Price * invoiceDetails[i].ItemQuantity)
                });
            }
            PrintTable(rowsContents, columnsFormat, null, hasCenterBorder: true, pageWidth: pageWidth);
            Console.WriteLine(string.Format("| {0} | {1} |", "Total".CenterLine(41), string.Format("{0:0,0}", invoiceDetails.Sum(x => x.Amount)).PadLeft(12)).CenterLine(pageWidth));
            Console.WriteLine(string.Format("+{0}+", new string('-', 58)).CenterLine(pageWidth));

            Console.WriteLine();
            Console.WriteLine(string.Format("{0,-13} {1}", "InvoiceID:", invoice.InvoiceID).PadRight(60).CenterLine(pageWidth));
            Console.WriteLine(string.Format("{0,-13} {1}", "Date Created:", invoice.CreatedTime.ToShortDateString()).PadRight(60).CenterLine(pageWidth));
            Console.WriteLine(string.Format("{0,-13} {1}", "Cashier:", string.Join(' ', invoice.Employee_Info.FirstName, invoice.Employee_Info.LastName)).PadRight(60).CenterLine(pageWidth));
            Console.WriteLine();
            Console.WriteLine(string.Format("{0}", new string('-', 60)).CenterLine(pageWidth));
            Console.WriteLine();
            Console.WriteLine("Thank you very much".CenterLine(pageWidth));
            Console.WriteLine("See you a gain".CenterLine(pageWidth));
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
            string[] menu = { "Search by ID", "Search by ISBN", "Search by TITLE", "Search by CATEGORY", "Search by AUTHOR", "Search by PUBLISHER", "Return to Main Menu" };
            do
            {
                Console.Clear();
                PrintHeader("____ ____ ____ ____ ____ _  _    ___  ____ ____ _  _ \r\n[__  |___ |__| |__/ |    |__|    |__] |  | |  | |_/  \r\n___] |___ |  | |  \\ |___ |  |    |__] |__| |__| | \\_ ");
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
                        status = SearchBookByBookID();
                        break;
                    case 2:
                        status = SearchBookByISBN();
                        break;
                    case 3:
                        status = SearchBookByTitle();
                        break;
                    case 4:
                        status = SearchBookByCategory();
                        break;
                    case 5:
                        status = SearchBookByAuthor();
                        break;
                    case 6:
                        status = SearchBookByPublisher();
                        break;
                    case 7:
                        break;
                    default:
                        Wait("Invalid choice!", ConsoleColor.Red);
                        break;
                }
                if (status <= -100)
                    return status;
                Console.Clear();
            } while (choice != menu.Length);
            return 0;
        }
        private static int SearchBookByBookID()
        {
            int status = 0;
            string menuTitle = "____ ____ ____ ____ ____ _  _    ___  ____ ____ _  _    ___  _   _    _ ___  \r\n[__  |___ |__| |__/ |    |__|    |__] |  | |  | |_/     |__]  \\_/     | |  \\ \r\n___] |___ |  | |  \\ |___ |  |    |__] |__| |__| | \\_    |__]   |      | |__/ ";
            PrintHeader(menuTitle);
            Console.WriteLine();
            uint bookID = GetUInt32("Input Book ID: ");
            BookBL bookBL = new BookBL();
            Book book = bookBL.GetBookByID(bookID, out Exception e);
            Console.WriteLine();
            if (book == null)
            {
                PrintLine("Found nothing.", ConsoleColor.DarkGray);
                Console.WriteLine();
            }
            else
                ShowBookInfo(book);
            Wait();
            return status;
        }
        private static int SearchBookByISBN()
        {
            int status = 0;
            string menuTitle = "____ ____ ____ ____ ____ _  _    ___  ____ ____ _  _    ___  _   _    _ ____ ___  _  _ \r\n[__  |___ |__| |__/ |    |__|    |__] |  | |  | |_/     |__]  \\_/     | [__  |__] |\\ | \r\n___] |___ |  | |  \\ |___ |  |    |__] |__| |__| | \\_    |__]   |      | ___] |__] | \\| ";
            PrintHeader(menuTitle);
            Console.WriteLine();
            string isbn = GetString("Input ISBN10 or ISBN13: ");
            BookBL bookBL = new BookBL();
            Book book = bookBL.GetBookByISBN(isbn, out Exception e);
            Console.WriteLine();
            if (book == null)
            {
                PrintLine("Found nothing.", ConsoleColor.DarkGray);
                Console.WriteLine();
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
            string menuTitle = "____ ____ ____ ____ ____ _  _    ___  ____ ____ _  _    ___  _   _    ____ ____ ___ ____ ____ ____ ____ _   _ \r\n[__  |___ |__| |__/ |    |__|    |__] |  | |  | |_/     |__]  \\_/     |    |__|  |  |___ | __ |  | |__/  \\_/  \r\n___] |___ |  | |  \\ |___ |  |    |__] |__| |__| | \\_    |__]   |      |___ |  |  |  |___ |__] |__| |  \\   |   ";
            PrintHeader(menuTitle);
            string pattern = GetString("\nInput category to find: ");
            List<Category> foundCategories = categoryBL.FindCategories(pattern, out Exception ex);
            Console.WriteLine();
            if (foundCategories.Count == 0)
            {
                PrintLine("Found nothing.", ConsoleColor.DarkGray);
                Console.WriteLine();
                Wait();
            }
            else
            {
                int maxNumberOfItemsPerPage = 10;
                int pageNumber = 1;
                char button;
                do
                {
                    PrintHeader(menuTitle);
                    Console.WriteLine();
                    PrintLine("Categories filter: " + pattern, ConsoleColor.Yellow);
                    Console.WriteLine();
                    List<List<string>> rowsContents = new List<List<string>>() { new List<string>() { "Index", "Category Name".CenterLine(97) } };
                    List<int> columnsFormat = new List<int>() { 5, -97 };
                    for (int i = (pageNumber - 1) * maxNumberOfItemsPerPage; i < Math.Min(pageNumber * maxNumberOfItemsPerPage, foundCategories.Count); i++)
                    {
                        Category category = foundCategories[i];
                        rowsContents.Add(new List<string>() { (i + 1).ToString(), category.CategoryName });
                    }
                    Console.WriteLine("Page {0,3}/{1,3}".CenterLine(pageWidth), pageNumber, Math.Ceiling((double)foundCategories.Count / maxNumberOfItemsPerPage));
                    PrintTable(rowsContents, columnsFormat, pageWidth: pageWidth);
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
                uint index = GetUInt32("Input category index (input 0 to go back): ");
                if (index == 0)
                    return status;
                Console.WriteLine();
                if (index > 0 && index <= foundCategories.Count)
                {
                    categoryToFind = foundCategories[(int)(index - 1)];
                    do
                    {
                        PrintHeader(menuTitle);
                        Console.WriteLine();
                        PrintLine("Category : " + categoryToFind.CategoryName, ConsoleColor.Yellow);
                        Console.WriteLine();
                        BookBL bookBL = new BookBL();
                        var foundBooks = new List<Book>();
                        foundBooks = bookBL.FindBooksByCategory(categoryToFind.CategoryID, out ex);
                        if (foundBooks == null || foundBooks.Count == 0)
                        {
                            Print("No book found!", ConsoleColor.DarkGray);
                            Console.WriteLine();
                            Wait();
                            break;
                        }
                        pageNumber = 1;
                        do
                        {
                            PrintHeader(menuTitle);
                            Console.WriteLine();
                            PrintLine("Category : " + categoryToFind.CategoryName, ConsoleColor.Yellow);
                            Console.WriteLine();
                            button = PrintListOfBooks(foundBooks, ref pageNumber, maxNumberOfItemsPerPage);
                        } while (button != ((char)ConsoleKey.Enter));
                        Console.WriteLine();
                        index = GetUInt32("Input book index (input 0 to go back): ");
                        if (index == 0)
                            return status;
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
                {
                    PrintLine("Invalid index!", ConsoleColor.Red);
                    Console.WriteLine();
                    Wait();
                }
            }
            return status;
        }
        private static int SearchBookByAuthor()
        {
            Author authorToFind = null;
            int status = 0;
            char choice;
            AuthorBL authorBL = new AuthorBL();
            string menuTitle = "____ ____ ____ ____ ____ _  _    ___  ____ ____ _  _    ___  _   _    ____ _  _ ___ _  _ ____ ____\r\n[__  |___ |__| |__/ |    |__|    |__] |  | |  | |_/     |__]  \\_/     |__| |  |  |  |__| |  | |__/\r\n___] |___ |  | |  \\ |___ |  |    |__] |__| |__| | \\_    |__]   |      |  | |__|  |  |  | |__| |  \\";
            PrintHeader(menuTitle);
            string pattern = GetString("\nInput author to find: ");
            List<Author> foundAuthors = authorBL.FindAuthorByPatternMatching(pattern, out Exception ex);
            Console.WriteLine();
            if (foundAuthors.Count == 0)
            {
                PrintLine("Found nothing.", ConsoleColor.DarkGray);
                Console.WriteLine();
                Wait();
            }
            else
            {
                int maxNumberOfItemsPerPage = 10;
                int pageNumber = 1;
                char button;
                do
                {
                    PrintHeader(menuTitle);
                    Console.WriteLine();
                    PrintLine("Author filter: " + pattern, ConsoleColor.Yellow);
                    Console.WriteLine();
                    List<List<string>> rowsContents = new List<List<string>>() { new List<string>() { "Index", "Author Name".CenterLine(86) } };
                    List<int> columnsFormat = new List<int>() { 5, -86 };
                    for (int i = (pageNumber - 1) * maxNumberOfItemsPerPage; i < Math.Min(pageNumber * maxNumberOfItemsPerPage, foundAuthors.Count); i++)
                    {
                        Author author = foundAuthors[i];
                        rowsContents.Add(new List<string>() { (i + 1).ToString(), author.AuthorName });
                    }
                    Console.WriteLine("Page {0,3}/{1,3}".CenterLine(pageWidth), pageNumber, Math.Ceiling((double)foundAuthors.Count / maxNumberOfItemsPerPage));
                    PrintTable(rowsContents, columnsFormat, pageWidth: pageWidth);
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
                uint index = GetUInt32("Input author index (input 0 to go back): ");
                if (index == 0)
                    return status;
                Console.WriteLine();
                if (index > 0 && index <= foundAuthors.Count)
                {
                    authorToFind = foundAuthors[(int)(index - 1)];
                    do
                    {
                        PrintHeader(menuTitle);
                        Console.WriteLine();
                        PrintLine("Author : " + authorToFind.AuthorName, ConsoleColor.Yellow);
                        Console.WriteLine();
                        BookBL bookBL = new BookBL();
                        var foundBooks = new List<Book>();
                        foundBooks = bookBL.FindBooksByAuthor(authorToFind.AuthorID, out ex);
                        if (foundBooks == null || foundBooks.Count == 0)
                        {
                            Print("No book found!", ConsoleColor.DarkGray);
                            Console.WriteLine();
                            Wait();
                            break;
                        }
                        pageNumber = 1;
                        do
                        {
                            PrintHeader(menuTitle);
                            Console.WriteLine();
                            PrintLine("Author : " + authorToFind.AuthorName, ConsoleColor.Yellow);
                            Console.WriteLine();
                            button = PrintListOfBooks(foundBooks, ref pageNumber, maxNumberOfItemsPerPage);
                        } while (button != ((char)ConsoleKey.Enter));
                        Console.WriteLine();
                        index = GetUInt32("Input book index (input 0 to go back): ");
                        if (index == 0)
                            return status;
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
                {
                    PrintLine("Invalid index!", ConsoleColor.Red);
                    Console.WriteLine();
                    Wait();
                }
            }
            return status;
        }
        private static int SearchBookByPublisher()
        {
            Publisher publisherToFind = null;
            int status = 0;
            char choice;
            PublisherBL publisherBL = new PublisherBL();
            string menuTitle = "____ ____ ____ ____ ____ _  _    ___  ____ ____ _  _    ___  _   _    ___  _  _ ___  _    _ ____ _  _ ____ ____\r\n[__  |___ |__| |__/ |    |__|    |__] |  | |  | |_/     |__]  \\_/     |__] |  | |__] |    | [__  |__| |___ |__/\r\n___] |___ |  | |  \\ |___ |  |    |__] |__| |__| | \\_    |__]   |      |    |__| |__] |___ | ___] |  | |___ |  \\";
            PrintHeader(menuTitle);
            Console.WriteLine();
            string pattern = GetString("Search for Publisher's name : ");
            List<Publisher> foundPublishers = publisherBL.FindPublisherByPatternMatching(pattern, out Exception ex);
            Console.WriteLine();
            if (foundPublishers.Count == 0)
            {
                PrintLine("Found nothing.", ConsoleColor.DarkGray);
                Console.WriteLine();
                Wait();
            }
            else
            {
                int maxNumberOfItemsPerPage = 10;
                int pageNumber = 1;
                char button;
                do
                {
                    PrintHeader(menuTitle);
                    Console.WriteLine();
                    PrintLine("Publisher filter : " + pattern, ConsoleColor.Yellow);
                    Console.WriteLine();
                    Console.WriteLine("Page {0,3}/{1,3}".CenterLine(pageWidth), pageNumber, Math.Ceiling((double)foundPublishers.Count / maxNumberOfItemsPerPage));
                    List<List<string>> rowsContents = new List<List<string>>() { new List<string>() { "Index", "Publisher Name".CenterLine(99) } };
                    List<int> columnsFormat = new List<int>() { 5, -99 };
                    for (int i = (pageNumber - 1) * maxNumberOfItemsPerPage; i < Math.Min(pageNumber * maxNumberOfItemsPerPage, foundPublishers.Count); i++)
                    {
                        Publisher publisher = foundPublishers[i];
                        rowsContents.Add(new List<string>() { (i + 1).ToString(), publisher.PublisherName });
                    }
                    PrintTable(rowsContents, columnsFormat, pageWidth: pageWidth);
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
                uint index = GetUInt32("Input publisher index (input 0 to go back): ");
                if (index == 0)
                    return status;
                Console.WriteLine();
                if (index > 0 && index <= foundPublishers.Count)
                {
                    publisherToFind = foundPublishers[(int)(index - 1)];
                    do
                    {
                        PrintHeader(menuTitle);
                        Console.WriteLine();
                        PrintLine("Publisher : " + publisherToFind.PublisherName, ConsoleColor.Yellow);
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
                            PrintHeader(menuTitle);
                            Console.WriteLine();
                            PrintLine("Publisher : " + publisherToFind.PublisherName, ConsoleColor.Yellow);
                            Console.WriteLine();
                            button = PrintListOfBooks(foundBooks, ref pageNumber, maxNumberOfItemsPerPage);
                        } while (button != ((char)ConsoleKey.Enter));
                        Console.WriteLine();
                        index = GetUInt32("Input book index (input 0 to go back): ");
                        if (index == 0)
                            return status;
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
                {
                    PrintLine("Invalid index!", ConsoleColor.Red);
                    Console.WriteLine();
                    Wait();
                }
            }
            return status;
        }
        private static int SearchBookByTitle()
        {
            int status = 0;
            char choice;
            BookBL bookBL = new BookBL();
            string menuTitle = "____ ____ ____ ____ ____ _  _    ___  ____ ____ _  _    ___  _   _    ___ _ ___ _    ____ \r\n[__  |___ |__| |__/ |    |__|    |__] |  | |  | |_/     |__]  \\_/      |  |  |  |    |___ \r\n___] |___ |  | |  \\ |___ |  |    |__] |__| |__| | \\_    |__]   |       |  |  |  |___ |___ ";
            PrintHeader(menuTitle);
            Console.WriteLine();
            string pattern = GetString("Search for Book's Title: ");
            List<Book> foundBooks = bookBL.FindBooksByTitle(pattern, out Exception ex);
            Console.WriteLine();
            if (foundBooks.Count == 0)
            {
                PrintLine("Found nothing!", ConsoleColor.DarkGray);
                Console.WriteLine();
                Wait();
            }
            else
            {
                int maxNumberOfBooksPerPage = 10;
                int pageNumber = 1;
                do
                {
                    char button;
                    do
                    {
                        PrintHeader(menuTitle);
                        Console.WriteLine();
                        PrintLine("Search for Book's Title: " + pattern, ConsoleColor.Yellow);
                        Console.WriteLine();
                        button = PrintListOfBooks(foundBooks, ref pageNumber, maxNumberOfBooksPerPage);
                    } while (button != ((char)ConsoleKey.Enter));
                    Console.WriteLine();
                    uint index = GetUInt32("Input book index (input 0 to go back): ");
                    if (index == 0)
                        return status;
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
            return status;
        }
        static char PrintListOfBooks(List<Book> listOfBooks, ref int pageNumber, int maxNumberOfItemsPerPage)
        {
            char button;
            List<List<string>> rowsContents = new List<List<string>>() { new List<string>() { "Index", "Book Title", "Format", "Price (VND)" } };
            List<int> columnsFormat = new List<int>() { 5, -120, -9, 11 };
            for (int i = (pageNumber - 1) * maxNumberOfItemsPerPage; i < Math.Min(pageNumber * maxNumberOfItemsPerPage, listOfBooks.Count); i++)
            {
                Book book = listOfBooks[i];
                rowsContents.Add(new List<string>() { (i + 1).ToString(), book.Title, book.Format.ToString(), String.Format("{0:0,0}", book.Price) });
            }
            Console.WriteLine("Page {0,3}/{1,3}".CenterLine(pageWidth), pageNumber, Math.Ceiling((double)listOfBooks.Count / maxNumberOfItemsPerPage));
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
            Console.Clear();
            string header = " ______              _        _                            ______                                                             _                               \r\n(____  \\            | |      | |   _                      |  ___ \\                                                 _         | |              _               \r\n ____)  ) ___   ___ | |  _    \\ \\ | |_  ___   ____ ____   | | _ | | ____ ____   ____  ____  ____ ____   ____ ____ | |_        \\ \\  _   _  ___| |_  ____ ____  \r\n|  __  ( / _ \\ / _ \\| | / )    \\ \\|  _)/ _ \\ / ___) _  )  | || || |/ _  |  _ \\ / _  |/ _  |/ _  )    \\ / _  )  _ \\|  _)        \\ \\| | | |/___)  _)/ _  )    \\ \r\n| |__)  ) |_| | |_| | |< ( _____) ) |_| |_| | |  ( (/ /   | || || ( ( | | | | ( ( | ( ( | ( (/ /| | | ( (/ /| | | | |__    _____) ) |_| |___ | |_( (/ /| | | |\r\n|______/ \\___/ \\___/|_| \\_|______/ \\___)___/|_|   \\____)  |_||_||_|\\_||_|_| |_|\\_||_|\\_|| |\\____)_|_|_|\\____)_| |_|\\___)  (______/ \\__  (___/ \\___)____)_|_|_|\r\n                                                                                    (_____|                                       (____/    ";
            Console.WriteLine(header);
            if (menuTitle != null)
            {
                Console.WriteLine($"{menuTitle}".CenterParagraph(pageWidth));
            }
        }
        static void ShowBookInfo(Book book)
        {
            List<int> columnsFormat = new List<int>() { -15, -136 };
            List<List<string>> rowsContents = new List<List<string>>()
            {
                new List<string>() { "BookID", book.BookID.ToString() },
                new List<string>() { "Title", book.Title },
                new List<string>() { "Price", string.Format("{0:0,0} VND", book.Price) },
                new List<string>() { "Format", book.Format.ToString() + ""},
                new List<string>() { "ISBN13", book.ISBN13 + ""},
                new List<string>() { "ISBN10", book.ISBN10 + ""},
                new List<string>() { "Author", String.Join(", ", book.GetStringAuthors()) },
                new List<string>() { "Publisher", book.PublisherName + ""},
                new List<string>() { "Language", String.Join(", ", book.GetStringLanguages()) },
                new List<string>() { "PublicationDate", book.PublicationDate.HasValue ? book.PublicationDate.Value.ToLongDateString() : "" },
                new List<string>() { "NumberOfPages", book.NumberOfPages.ToString() + ""},
                new List<string>() { "Dimension", book.Dimensions + ""},
                new List<string>() { "Quantity", book.Quantity.ToString() },
                new List<string>() { "Status", book.Status.ToString() },
                new List<string>() { "Categories", String.Join(", ", book.GetStringCategories()) },
                new List<string>() { "Description", book.Description + "" }
            };
            PrintTable(rowsContents, columnsFormat, "BOOK INFO", false);
        }
    }
}
