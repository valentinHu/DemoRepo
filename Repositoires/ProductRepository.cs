using DemoRepo.Models;

namespace DemoRepo.Repositoires
{
    // "Fake" = faux repository : les données sont en mémoire (une List),
    // pas dans une vraie base. Utile pour tester sans SQL Server.
    public class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Product 1", Price = 10.0m },
            new Product { Id = 2, Name = "Product 2", Price = 20.0m },
            new Product { Id = 3, Name = "Product 3", Price = 30.0m }
        };

        IEnumerable<Product> IProductRepository.GetAllProducts()
        {
            return _products;
        }

        Product? IProductRepository.GetProductById(int productId)
        {
            // FirstOrDefault : le premier produit dont l'Id correspond,
            // ou null s'il n'y en a aucun. "p => p.Id == productId" est une lambda :
            // "pour chaque produit p, est-ce que son Id est égal à productId ?"
            return _products.FirstOrDefault(p => p.Id == productId);
        }

        int IProductRepository.AddProduct(Product product)
        {
            _products.Add(product);
            return product.Id;
        }

        bool IProductRepository.UpdateProduct(Product product)
        {
            Product? existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing == null)
            {
                return false;              // rien à modifier
            }

            existing.Name = product.Name;
            existing.Price = product.Price;
            return true;
        }

        bool IProductRepository.DeleteProduct(int productId)
        {
            Product? existing = _products.FirstOrDefault(p => p.Id == productId);
            if (existing == null)
            {
                return false;
            }

            // Remove renvoie déjà true/false selon que la suppression a réussi
            return _products.Remove(existing);
        }
    }
}