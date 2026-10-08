using DemoRepo.Models;
using DemoRepo.Repositoires;
using Microsoft.Data.SqlClient;
using System.Data;

public class AdoNetRepository : IProductRepository   // même interface que le FakeRepository
{
    private SqlConnection _connection;
         // l'adresse de la base
    

    public AdoNetRepository(string? connectionString)
    {
        _connection =  new SqlConnection(connectionString);    // on la reçoit de l'extérieur, on ne l'écrit pas en dur ici
    }

    IEnumerable<Product> IProductRepository.GetAllProducts()
    {
        var products = new List<Product>();   // liste vide qu'on va remplir

        var command = new SqlCommand("SELECT Id, Name, Price FROM Products", _connection);
        command.CommandType = CommandType.Text;   // c'est une requête SQL écrite en texte (pas une procédure stockée)

        _connection.Open();   // ouvre la connexion

        using (var reader = command.ExecuteReader())   // lance la requête et récupère un lecteur
        {
            while (reader.Read())   // avance ligne par ligne, renvoie false quand il n'y en a plus
            {
                var product = new Product
                {
                    Id = reader.GetInt32("Id"),         // lit la colonne "Id" comme un int
                    Name = reader.GetString("Name"),    // lit la colonne "Name" comme un string
                    Price = reader.GetDecimal("Price")  // lit la colonne "Price" comme un decimal
                };
                products.Add(product);   // ⬅️ ajoute le produit à la liste
            }
        }

        _connection.Close();   // ⬅️ ferme la connexion
        return products;       // 
    }

    Product? IProductRepository.GetProductById(int productId)
    {
        // @Id est un emplacement réservé : la valeur sera  envoyée À PART, jamais collée dans le texte
        var command = new SqlCommand("SELECT Id, Name, Price FROM Products WHERE Id = @Id", _connection);
        command.CommandType = CommandType.Text;

        // on dit à SQL Server : "@Id vaut productId"
        command.Parameters.AddWithValue("@Id", productId);

        _connection.Open();

        // Product? = peut valoir null. On part de null : "pas encore trouvé"
        Product? product = null;

        using (var reader = command.ExecuteReader())
        {
            // IF et pas WHILE : on cherche UN SEUL produit (l'Id est unique)
            if (reader.Read())   // true s'il y a une ligne, false si aucun produit n'a cet Id
            {
                product = new Product
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Price = reader.GetDecimal("Price")
                };
            }
        }

        _connection.Close();
        return product;   // le produit trouvé, ou null s'il n'existe pas
    }

    int IProductRepository.AddProduct(Product product)
    {
        // OUTPUT INSERTED.Id : "après l'insertion, renvoie-moi l'Id que SQL Server vient de créer"
        var command = new SqlCommand(
            "INSERT INTO Products (Name, Price) OUTPUT INSERTED.Id VALUES (@Name, @Price)", _connection);
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Name", product.Name);    // valeur du nom
        command.Parameters.AddWithValue("@Price", product.Price);  // valeur du prix

        _connection.Open();

        // ExecuteScalar : récupère UNE seule valeur (ici le nouvel Id)
        // il renvoie un "object", donc on le convertit en int avec (int)
        int newProductId = (int)command.ExecuteScalar();

        _connection.Close();
        return newProductId;   // on renvoie l'Id généré
    }

    bool IProductRepository.UpdateProduct(Product product)
    {
        // UPDATE ... SET : modifie les colonnes de la ligne dont l'Id correspond
        // ⚠️ sans le WHERE, TOUTES les lignes de la table seraient modifiées
        var command = new SqlCommand("UPDATE Products SET Name = @Name, Price = @Price WHERE Id = @Id", _connection);
        command.CommandType = CommandType.Text;   // c'est une requête SQL en texte

        // les valeurs sont envoyées à part (protection contre l'injection SQL)
        command.Parameters.AddWithValue("@Id", product.Id);        // quelle ligne modifier
        command.Parameters.AddWithValue("@Name", product.Name);    // nouveau nom
        command.Parameters.AddWithValue("@Price", product.Price);  // nouveau prix

        _connection.Open();   // ouvre la connexion

        // ExecuteNonQuery renvoie le nombre de lignes modifiées
        if (command.ExecuteNonQuery() > 0)   // au moins une ligne modifiée ?
        {
            _connection.Close();
            return true;                     // oui : le produit existait et a été modifié
        }

        _connection.Close();
        return false;                        // non : aucun produit avec cet Id
    }

    bool IProductRepository.DeleteProduct(int productId)
    {
        var command = new SqlCommand("DELETE FROM Products WHERE Id = @Id", _connection);
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Id", productId);   // @Id = la valeur reçue

        _connection.Open();

        if (command.ExecuteNonQuery() > 0)   // au moins une ligne supprimée ?
        {
            _connection.Close();
            return true;                     // oui → succès
        }

        _connection.Close();
        return false;                        // non → l'Id n'existait pas
    }
}