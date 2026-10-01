using DemoRepo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRepo.Repositoires
{
    public interface IProductRepository
    {
            IEnumerable<Product> GetAllProducts();
            Product? GetProductById(int productId);   // ? = peut renvoyer null si pas trouvé
            int AddProduct(Product product);          // renvoie l'Id du produit ajouté
            bool UpdateProduct(Product product);
           bool DeleteProduct(int productId);
        
    }
}
