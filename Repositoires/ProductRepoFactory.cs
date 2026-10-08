using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoRepo.Models;

namespace DemoRepo.Repositoires
{
    public class ProductRepoFactory
    {
        // static = on l'appelle sans créer d'objet : ProductRepoFactory.CreateProductRepository(...)
        // elle renvoie l'INTERFACE, pas une classe précise
        public static IProductRepository CreateProductRepository(RepositoryType repositoryType, string connectionString)
        {
            return repositoryType switch
            {
                RepositoryType.Fake => new FakeProductRepository(),                   // le fake n'a pas besoin de base
                RepositoryType.AdoNet => new AdoNetRepository(connectionString),        // on lui passe l'adresse de la base
                _ => throw new ArgumentException("Invalid repository type", nameof(repositoryType)),
            };
        }
    }
}
 