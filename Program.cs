using DemoRepo.Models;
using DemoRepo.Repositoires;   // ⚠️ chez toi : DemoRepo.Repositoires (le nom de ton dossier)

Console.WriteLine("Hello, World!");

// Créer un produit "à la main", sans repository
Product product = new Product
{
    Id = 1,
    Name = "Sample Product",
    Price = 19.99m
};

Console.WriteLine(product.ToString());


// On déclare la variable avec le type de l'INTERFACE, et on crée l'implémentation Fake.
// Le reste du programme ne connaît que IProductRepository : demain on pourra
// mettre un vrai repository SQL à la place sans changer une seule autre ligne.
IProductRepository productRepository = new FakeProductRepository();

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