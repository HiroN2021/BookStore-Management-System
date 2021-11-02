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
    public enum BookFormat
    {
        PaperBack = 1,
        HardBack,
        BoardBook,
        Cards
    }
    public class Book
    {
        List<Author> _authors = new List<Author>();
        List<Language> _languages = new List<Language>();
        Publisher _publisher;
        List<Category> _categories = new List<Category>();
        // 
        public uint BookID { get; set; }
        public string Title { get; set; }
        public int? Price { get; set; } = null;
        public BookFormat Format;
        public string ISBN13 { get; set; }
        public string ISBN10 { get; set; }
        public List<Author> Authors { get => this._authors; set => this._authors = value; }
        public Publisher Publisher { get => this._publisher; set => this._publisher = value; }
        public string PublisherName { get => _publisher?.PublisherName; }
        public List<Language> Languages { get => this._languages; set => this._languages = value; }
        public DateTime? PublicationDate { get; set; } = null;
        public uint? NumberOfPages { get; set; }
        public string Dimensions { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; } = null;
        public BookStatus Status { get; set; } = BookStatus.Available;
        public List<Category> Categories { get => this._categories; set => this._categories = value; }
        // 
        public void AddAuthors(uint authorID, string authorName) => _authors.Add(new Author() { AuthorID = authorID, AuthorName = authorName });
        public void AddLanguages(uint languageID, string languageName) => _languages.Add(new Language() { LanguageID = languageID, LanguageName = languageName });
        public void AddCategories(uint categoryID, string categoryName) => _categories.Add(new Category() { CategoryID = categoryID, CategoryName = categoryName });
        // 
        public List<string> GetStringAuthors()
        {
            List<string> authorsName = new List<string>();
            foreach (var item in _authors)
            {
                authorsName.Add(item.AuthorName);
            }
            return authorsName;
        }

        public List<string> GetStringLanguages()
        {
            List<string> languagesName = new List<string>();
            foreach (var item in _languages)
            {
                languagesName.Add(item.LanguageName);
            }
            return languagesName;
        }

        public List<string> GetStringCategories()
        {
            List<string> categoriesName = new List<string>();
            foreach (var item in _categories)
            {
                categoriesName.Add(item.CategoryName);
            }
            return categoriesName;
        }
    }
}