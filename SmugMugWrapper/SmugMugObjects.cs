using System;
using System.Collections.Generic;

namespace SmugMugWrapper.V1
{
    [Serializable]
    public class Response
    {
        public string stat { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    [Serializable]
    public class AlbumsResponse : Response
    {
        public List<Album> Albums { get; set; }
    }

    [Serializable]
    public class AlbumResponse : Response
    {
        public Album Album { get; set; }
    }

    [Serializable]
    public class CategoriesResponse : Response
    {
        public List<Category> Categories { get; set; }
    }

    [Serializable]
    public class CategoryResponse : Response
    {
        public Category Category { get; set; }
    }

    [Serializable]
    public class SubCategoriesResponse : Response
    {
        public List<SubCategory> SubCategories { get; set; }
    }

    [Serializable]
    public class SubCategoryResponse : Response
    {
        public SubCategory SubCategory { get; set; }
    }

    [Serializable]
    public class ImageResponse : Response
    {
        public Image Image { get; set; }
    }

    [Serializable]
    public class Album
    {
        public string id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public Category Category { get; set; }
        public SubCategory SubCategory { get; set; }
    }

    [Serializable]
    public class Category
    {
        public Category() { }

        public Category(Category original)
        {
            id = original.id;
            Name = original.Name;
            NiceName = original.NiceName;
        }

        public string id { get; set; }
        public string Name { get; set; }
        public string NiceName { get; set; }
    }

    [Serializable]
    public class SubCategory
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string NiceName { get; set; }
    }

    [Serializable]
    public class Image
    {
        public string id { get; set; }
        public string Key { get; set; }
        public Album Album { get; set; }
        public DateTime Date { get; set; }
        public string FileName { get; set; }
        public bool Hidden { get; set; }
    }

}
