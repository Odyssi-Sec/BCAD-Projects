using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DeweyDecimalApplication
{
    [Serializable]
    [XmlRoot("DeweyDecimalSystem")]
    public class DeweyDecimalData
    {
        [XmlElement("Category")]
        public List<Category> Categories { get; set; } = new List<Category>();

        public void AddCategory(Category category)
        {
            Add(Categories, category);
        }

        private void Add<T>(List<T> list, T item)
        {
            list.Add(item);
        }
    }

    [Serializable]
    [XmlType("Category")]
    public class Category
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Subcategory")]
        public List<Subcategory> Subcategories { get; set; } = new List<Subcategory>();

        public Category(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public void AddSubcategory(Subcategory subcategory)
        {
            Add(Subcategories, subcategory);
        }

        private void Add<T>(List<T> list, T item)
        {
            list.Add(item);
        }
    }

    [Serializable]
    [XmlType("Subcategory")]
    public class Subcategory
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("SubcategoryTitle")]
        public List<SubcategoryTitle> SubcategoryTitles { get; set; } = new List<SubcategoryTitle>();

        public Subcategory(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public void AddSubcategoryTitle(SubcategoryTitle title)
        {
            Add(SubcategoryTitles, title);
        }

        private void Add<T>(List<T> list, T item)
        {
            list.Add(item);
        }
    }

    [Serializable]
    [XmlType("SubcategoryTitle")]
    public class SubcategoryTitle
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        public object ParentCategory { get; internal set; }
        public object ParentSubcategory { get; internal set; }
        public string Tag { get; internal set; }

        public SubcategoryTitle(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}