using System;
using System.Collections.Generic;

namespace Persistence
{
    public enum BookStatus
    {
        Available = 1,
        Comming_Soon,
        Out_Of_Stock,
        Discontinued
    }
    public class Book
    {
        Author _author;
        Language _language;
        Publisher _publisher;
        List<Category> _categories = new List<Category>();
        // 
        public uint BookID { get; set; }
        public string Title { get; set; }
        public int? Price { get; set; } = null;
        public string ISBN13 { get; set; }
        public string ISBN10 { get; set; }
        public string AuthorName { get => _author?.AuthorName; }
        public string PublisherName { get => _publisher?.PublisherName; }
        public string Language { get => _language?.LanguageName; }
        public DateTime? PublicationDate { get; set; } = null;
        public uint? NumberOfPages { get; set; }
        public string Dimensions { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; } = null;
        public BookStatus Status { get; set; }
        public List<string> CategoriesName
        {
            get
            {
                // if (_categories == null)
                //     return null;
                List<string> categoriesName = new List<string>();
                foreach (var item in _categories)
                {
                    categoriesName.Add(item.CategoryName);
                }
                return categoriesName;
            }
        }
        // 
        public void SetAuthor(uint authorID, string authorName) => _author = new Author() { AuthorID = authorID, AuthorName = authorName };
        public void SetLanguage(uint languageID, string languageName) => _language = new Language() { LanguageID = languageID, LanguageName = languageName };
        public void SetPublisher(uint publisherID, string publisherName) => _publisher = new Publisher() { PublisherID = publisherID, PublisherName = publisherName };
        // public void SetCategories(List<Category> categories) => _categories = categories;
        public void AddCategories(uint categoryID, string categoryName) => _categories.Add(new Category() { CategoryID = categoryID, CategoryName = categoryName });
        // public void ShowInfo()
        // {
        //     string[] headers = { "BookID", "Title", "Price", "ISBN13", "ISBN10", "AuthorName",
        //     "PublisherName", "Language", "PublicationDate", "NumberOfPages", "Dimension", "Quantity",
        //     "Status", "Categories", "Description"};
        //     string[] values = { this.BookID.ToString(), this.Title,
        //     string.Format("{0:0,0} VND", this.Price),
        //     this.ISBN13, this.ISBN10,
        //     this.AuthorName, this.PublisherName, this.Language,
        //     this.PublicationDate != null ? this.PublicationDate.Value.ToShortDateString() : null,
        //     this.NumberOfPages.Value.ToString(), this.Dimensions,
        //     this.Quantity.ToString(),
        //     this.Status.ToString(), String.Join(", ", CategoriesName), this.Description+""};
        //     int count = values.Length;
        //     var sb = new System.Text.StringBuilder();
        //     string lineSeparator = "+" + new string('-', 131) + "+";
        //     Console.WriteLine(lineSeparator);
        //     Console.WriteLine("|{0}|", "Book's Info".PadLeft(71).PadRight(131));
        //     Console.WriteLine(lineSeparator);
        //     string lt = "| {0,-15} | {1,-111} |";
        //     for (int i = 0; i < count; i++)
        //     {
        //         if (headers[i] == "Title" || headers[i] == "Description")
        //         {
        //             Console.WriteLine(lt, headers[i], values[i].Substring(0, Math.Min(111, values[i].Length)));
        //             for (int j = 1; j * 111 < values[i].Length; j++)
        //             {
        //                 Console.WriteLine(lt, "", values[i].Substring(111 * j, Math.Min(111, values[i].Length - 111 * j)));
        //             }
        //         }
        //         else
        //             Console.WriteLine(lt, headers[i], values[i]);
        //     }
        //     Console.WriteLine(lineSeparator);
        // }
    }
}