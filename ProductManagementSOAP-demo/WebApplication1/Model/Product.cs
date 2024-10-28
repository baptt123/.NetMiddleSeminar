using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[XmlRoot("Products")]
public class Product
{
    // Properties for the Product class
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Mota { get; set; } // Mota means description in Vietnamese

    // Default constructor
    public Product()
    {
    }

    // Constructor with parameters
    public Product(int id, string name, decimal price, string mota)
    {
        Id = id;
        Name = name;
        Price = price;
        Mota = mota;
    }

    // Method to display product details
    public void DisplayProductInfo()
    {
        Console.WriteLine("Product ID: " + Id);
        Console.WriteLine("Product Name: " + Name);
        Console.WriteLine("Product Price: " + Price);
        Console.WriteLine("Product Description: " + Mota);
    }

    // Convert a list of products to XML
    public static string ConvertToXml<T>(List<T> data)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<T>), new XmlRootAttribute("Products"));
        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, data);
            return writer.ToString();
        }
    }

}
