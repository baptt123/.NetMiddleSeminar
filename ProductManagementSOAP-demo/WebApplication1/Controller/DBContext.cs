using Dapper;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using WebApplication1.SoapBuilder;

public class DBContext
{
    private string _connectionString;

    public DBContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Phương thức lấy tất cả các sản phẩm dưới định dạng XML
    public string GetAllProducts()
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM product";
            List<Product> products = connection.Query<Product>(query).AsList();

            // Tạo XML cho danh sách sản phẩm
            XmlDocument productsXml = CreateProductsXml(products);

            // Wrap in SOAP envelope
            SoapEnvelopeBuilder envelopeBuilder = new SoapEnvelopeBuilder();
            string responseXml = envelopeBuilder.WrapInSoapEnvelope("GetAllProductsResponse", productsXml);

            // Print response XML for debugging purposes
            Console.WriteLine("Response XML:");
            Console.WriteLine(responseXml);

            return responseXml;
        }
    }

    // Phương thức thêm sản phẩm mới
    public string AddProduct(string name, decimal price, string mota)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            // Truy vấn để thêm sản phẩm mới
            string query = "INSERT INTO product (name, price, mota) VALUES (@name, @price, @mota);";

            // Tạo đối tượng parameters với các giá trị truyền vào
            var parameters = new
            {
                name = name,
                price = price,
                mota = mota
            };

            // Thực hiện truy vấn để thêm sản phẩm mới vào cơ sở dữ liệu
            int rowsAffected = connection.Execute(query, parameters);

            // Kiểm tra số hàng bị ảnh hưởng để xác nhận thêm thành công
            if (rowsAffected > 0)
                return "Add success";
            else
                return "Failed to add product";
        }
    }


    // Phương thức cập nhật thông tin sản phẩm
    public string UpdateProduct(int id, string name, decimal price, string mota)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            // Truy vấn để cập nhật sản phẩm
            string query = "UPDATE product SET name = @name, price = @price, mota = @mota WHERE id = @id;";

            // Tạo đối tượng parameters với các giá trị truyền vào
            var parameters = new
            {
                id = id,
                name = name,
                price = price,
                mota = mota
            };

            // Thực hiện truy vấn cập nhật
            int rowsAffected = connection.Execute(query, parameters);

            // Kiểm tra số lượng bản ghi bị ảnh hưởng để xác định việc cập nhật thành công hay không
            if (rowsAffected > 0)
            {
                // Trả về phản hồi thành công với thông tin sản phẩm đã cập nhật
                // Chú ý: Bạn có thể cần lấy thông tin sản phẩm cập nhật lại từ cơ sở dữ liệu để đảm bảo tính chính xác
                var updatedProduct = new Product
                {
                    Id = id,
                    Name = name,
                    Price = price,
                    Mota = mota
                };
                return "Cập nhật thành công";
            }

            // Trả về phản hồi lỗi nếu không có sản phẩm nào với ID tương ứng được tìm thấy
            return "Cập nhật thất bại";
        }
    }

    // Phương thức tìm sản phẩm theo tên
    public List<Product> GetProduct(string productName)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            string queryFindByName = "SELECT * FROM product WHERE name LIKE @name;";
            string queryFindAll = "SELECT * FROM product";

            // Xác định truy vấn và tham số
            string query = string.IsNullOrEmpty(productName) ? queryFindAll : queryFindByName;
            var parameters = string.IsNullOrEmpty(productName) ? null : new { name = $"%{productName}%" };

            // Truy vấn cơ sở dữ liệu
            List<Product> products = connection.Query<Product>(query, parameters).ToList();

            return products;
        }
    }

    // Phương thức xóa sản phẩm theo ID
    public string DeleteProduct(int productId)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "DELETE FROM product WHERE id = @id;";
            var parameters = new { id = productId };

            int rowsAffected = connection.Execute(query, parameters);

            if (rowsAffected > 0) return "Delete  success";
            return "No product with corresponding ID was found to delete.";
        }
    }
    public List<Product> GetProductById(int productId)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            // Truy vấn để lấy một sản phẩm dựa trên productId
            string query = "SELECT * FROM product WHERE id = @id";
            Product product = connection.QuerySingleOrDefault<Product>(query, new { id = productId });

            // Trả về danh sách sản phẩm (danh sách có thể rỗng nếu không tìm thấy sản phẩm)
            return product != null ? new List<Product> { product } : new List<Product>();
        }
    }

    // Phương thức lấy sản phẩm theo tên
    public List<Product> GetProductByName(string productName)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM product WHERE name LIKE @name";
            var products = connection.Query<Product>(query, new { name = $"%{productName}%" }).ToList();

            return products;
        }
    }


    private XmlDocument CreateProductsXml(List<Product> products)
    {
        XmlDocument xmlDoc = new XmlDocument();

        // Tạo phần gốc
        XmlElement root = xmlDoc.CreateElement("Products");
        xmlDoc.AppendChild(root);

        // Thêm từng sản phẩm vào XML
        foreach (var product in products)
        {
            XmlElement productElement = xmlDoc.CreateElement("Product");

            XmlElement idElement = xmlDoc.CreateElement("Id");
            idElement.InnerText = product.Id.ToString();
            productElement.AppendChild(idElement);

            XmlElement nameElement = xmlDoc.CreateElement("Name");
            nameElement.InnerText = product.Name;
            productElement.AppendChild(nameElement);

            XmlElement priceElement = xmlDoc.CreateElement("Price");
            priceElement.InnerText = product.Price.ToString("F2"); // Định dạng giá
            productElement.AppendChild(priceElement);
            XmlElement description = xmlDoc.CreateElement("Description");
            description.InnerText = product.Mota.ToString();
            productElement.AppendChild(description);
            root.AppendChild(productElement);
        }

        return xmlDoc;
    }
}
