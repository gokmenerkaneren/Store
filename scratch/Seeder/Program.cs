using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Store.Catalog.Entities;
using Store.Shared.Enums;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting seeder...");
#pragma warning disable CS0618
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
#pragma warning restore CS0618

        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("StoreMD");

        var colorsCollection = database.GetCollection<Color>("Colors");
        var sizesCollection = database.GetCollection<Size>("Sizes");

        // Clear existing just in case (optional, but good for idempotent seed)
        // colorsCollection.DeleteMany("{}");
        // sizesCollection.DeleteMany("{}");

        var colors = new List<Color>
        {
            new Color { Name = "Siyah", HexCode = "#000000", DisplayOrder = 1, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Beyaz", HexCode = "#FFFFFF", DisplayOrder = 2, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Kırmızı", HexCode = "#FF0000", DisplayOrder = 3, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Mavi", HexCode = "#0000FF", DisplayOrder = 4, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Yeşil", HexCode = "#008000", DisplayOrder = 5, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Sarı", HexCode = "#FFFF00", DisplayOrder = 6, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Lacivert", HexCode = "#000080", DisplayOrder = 7, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Gri", HexCode = "#808080", DisplayOrder = 8, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Bordo", HexCode = "#800020", DisplayOrder = 9, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Pembe", HexCode = "#FFC0CB", DisplayOrder = 10, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Turuncu", HexCode = "#FFA500", DisplayOrder = 11, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Kahverengi", HexCode = "#A52A2A", DisplayOrder = 12, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Bej", HexCode = "#F5F5DC", DisplayOrder = 13, IsActive = true, InsertUserId = Guid.Empty },
            new Color { Name = "Mor", HexCode = "#800080", DisplayOrder = 14, IsActive = true, InsertUserId = Guid.Empty }
        };

        var sizes = new List<Size>
        {
            new Size { Name = "XS", SizeType = SizeType.Clothing, PrimaryValue = "XS", DisplayOrder = 1, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "S", SizeType = SizeType.Clothing, PrimaryValue = "S", DisplayOrder = 2, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "M", SizeType = SizeType.Clothing, PrimaryValue = "M", DisplayOrder = 3, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "L", SizeType = SizeType.Clothing, PrimaryValue = "L", DisplayOrder = 4, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "XL", SizeType = SizeType.Clothing, PrimaryValue = "XL", DisplayOrder = 5, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "XXL", SizeType = SizeType.Clothing, PrimaryValue = "XXL", DisplayOrder = 6, IsActive = true, InsertUserId = Guid.Empty },
            
            new Size { Name = "36", SizeType = SizeType.Number, PrimaryValue = "36", DisplayOrder = 7, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "38", SizeType = SizeType.Number, PrimaryValue = "38", DisplayOrder = 8, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "40", SizeType = SizeType.Number, PrimaryValue = "40", DisplayOrder = 9, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "42", SizeType = SizeType.Number, PrimaryValue = "42", DisplayOrder = 10, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "44", SizeType = SizeType.Number, PrimaryValue = "44", DisplayOrder = 11, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "46", SizeType = SizeType.Number, PrimaryValue = "46", DisplayOrder = 12, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "48", SizeType = SizeType.Number, PrimaryValue = "48", DisplayOrder = 13, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "50", SizeType = SizeType.Number, PrimaryValue = "50", DisplayOrder = 14, IsActive = true, InsertUserId = Guid.Empty },
            
            new Size { Name = "30/30", SizeType = SizeType.Denim, PrimaryValue = "30", SecondaryValue = "30", DisplayOrder = 15, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "32/30", SizeType = SizeType.Denim, PrimaryValue = "32", SecondaryValue = "30", DisplayOrder = 16, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "32/32", SizeType = SizeType.Denim, PrimaryValue = "32", SecondaryValue = "32", DisplayOrder = 17, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "34/32", SizeType = SizeType.Denim, PrimaryValue = "34", SecondaryValue = "32", DisplayOrder = 18, IsActive = true, InsertUserId = Guid.Empty },
            
            new Size { Name = "36", SizeType = SizeType.Shoe, PrimaryValue = "36", DisplayOrder = 19, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "37", SizeType = SizeType.Shoe, PrimaryValue = "37", DisplayOrder = 20, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "38", SizeType = SizeType.Shoe, PrimaryValue = "38", DisplayOrder = 21, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "39", SizeType = SizeType.Shoe, PrimaryValue = "39", DisplayOrder = 22, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "40", SizeType = SizeType.Shoe, PrimaryValue = "40", DisplayOrder = 23, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "41", SizeType = SizeType.Shoe, PrimaryValue = "41", DisplayOrder = 24, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "42", SizeType = SizeType.Shoe, PrimaryValue = "42", DisplayOrder = 25, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "43", SizeType = SizeType.Shoe, PrimaryValue = "43", DisplayOrder = 26, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "44", SizeType = SizeType.Shoe, PrimaryValue = "44", DisplayOrder = 27, IsActive = true, InsertUserId = Guid.Empty },
            new Size { Name = "45", SizeType = SizeType.Shoe, PrimaryValue = "45", DisplayOrder = 28, IsActive = true, InsertUserId = Guid.Empty }
        };

        colorsCollection.InsertMany(colors);
        sizesCollection.InsertMany(sizes);

        Console.WriteLine($"Inserted {colors.Count} colors and {sizes.Count} sizes.");
    }
}
