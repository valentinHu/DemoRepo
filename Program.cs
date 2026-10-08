using DemoRepo.Models;
using DemoRepo.Repositoires;   // ⚠️ chez toi : DemoRepo.Repositoires (le nom de ton dossier)
using Microsoft.Extensions.Configuration;
Console.WriteLine("Hello, World!");


// Créer un produit "à la main", sans repository

Product product = new Product
{
    Id = 1,
    Name = "Sample Product",
    Price = 19.99m
};

Console.WriteLine(product.ToString());


// on construit un objet "configuration" qui lit appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())          // cherche le fichier dans le dossier du programme
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    // optional: false   → plante tout de suite si le fichier manque (mieux que de planter plus loin)
    // reloadOnChange    → relit le fichier s'il est modifié pendant l'exécution
    .Build();

// va chercher "ConnectionStrings" → "DefaultConnection" dans le JSON
// ?? throw : si GetConnectionString renvoie null (clé absente du JSON), on lance une erreur claire
string connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// GetSection("A:B") : va dans la section "A", puis prend la clé "B"
string repositoryTypeString = configuration.GetSection("RepositorySettings:RepositoryType").Value
    ?? throw new InvalidOperationException("Repository type not found.");

RepositoryType repositoryType = Enum.Parse<RepositoryType>(repositoryTypeString,ignoreCase: true);   // convertit la chaîne en enum


IProductRepository productRepository;

// on donne la chaîne à la factory, comme avant
productRepository = ProductRepoFactory.CreateProductRepository(repositoryType, connectionString);

// --- Lire tous les produits ---
IEnumerable<Product> products = productRepository.GetAllProducts();

foreach (var p in products)
{
    Console.WriteLine(p.ToString());
}

// --- Chercher un produit qui n'existe pas (Id 10) ---
// Product? : la variable peut contenir null, donc on DOIT tester avant de l'utiliser.
Product? productById = productRepository.GetProductById(10);
if (productById != null)
{
    Console.WriteLine(productById.ToString());
}
else
{
    Console.WriteLine("Product not found.");
}

Product newProduct = new Product
{
    Id = 4,
    Name = "New Product",
    Price = 49.99m
};

int newProductId = productRepository.AddProduct(newProduct);

// $"..." = interpolation : {newProductId} est remplacé par sa valeur
Console.WriteLine($"Added Product with Id: {newProductId}");

// Afficher toute la liste en UNE ligne :
// GetAllProducts() -> ToList() pour avoir une List -> ForEach exécute la lambda sur chaque produit
productRepository.GetAllProducts().ToList().ForEach(p => Console.WriteLine(p.ToString()));

// --- Retrouver le produit ajouté ---
var foundProduct = productRepository.GetProductById(newProduct.Id);

if (foundProduct != null)
{
    // --- Modifier ---
    foundProduct.Name = "Updated Product";
    var result = productRepository.UpdateProduct(foundProduct);   // true si la modif a réussi

    if (result)
    {
        Console.WriteLine($"Updated Product: {foundProduct.ToString()}");
        productRepository.GetAllProducts().ToList().ForEach(p => Console.WriteLine(p.ToString()));
    }
    else
    {
        Console.WriteLine("Failed to update product.");
    }

    // --- Supprimer ---
    Console.WriteLine($"Deleting Product with Id: {foundProduct.Id}");
    productRepository.DeleteProduct(foundProduct.Id);

    productRepository.GetAllProducts().ToList().ForEach(p => Console.WriteLine(p.ToString()));
}
else
{
    Console.WriteLine("Failed to add product.");
}