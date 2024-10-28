using System.Collections.Generic;
using System.Web.Services;

namespace WebApplication1
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    
    public class ProductManagement : System.Web.Services.WebService
    {
        private DBContext _dbHelper;

        public ProductManagement()
        {
            _dbHelper = new DBContext("Server=localhost;User ID=root;Password=;Database=product");
        }
        [WebMethod]
        public string GetAllProduct() => _dbHelper.GetAllProducts();
        [WebMethod]
                public string AddProduct(string name, decimal price, string mota)
                {
                    return _dbHelper.AddProduct(name,price,mota);
                }

                [WebMethod]
                public string UpdateProduct(int id, string name, decimal price, string mota)
                {
                    return _dbHelper.UpdateProduct( id, name, price, mota);
                }

                [WebMethod]
                public string DeleteProduct(int productId)
                {
                    return _dbHelper.DeleteProduct(productId);
                }
        [WebMethod]
        public List<Product> GetProductById(int productId)
        {
            return _dbHelper.GetProductById(productId);
        }

        [WebMethod]
        public List<Product> GetProductByName(string productName)
        {
            return _dbHelper.GetProductByName(productName);
        }
    }
        
    
}
