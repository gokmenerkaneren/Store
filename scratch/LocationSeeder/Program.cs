using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Store.Location.Persistence.Context;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;

namespace LocationSeeder
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new LocationContext();
            try
            {
                Console.WriteLine("Loading JSON data...");
                var jsonPath = @"C:\User\Projects\Store\scratch\turkey-data\turkey-neighbourhoods-master\src\data\neighbourhoods.json";
                var jsonData = File.ReadAllText(jsonPath);
                
                // Parse as JsonDocument because it's an array of arrays
                using var doc = JsonDocument.Parse(jsonData);
                var root = doc.RootElement;
                
                // Create a dictionary for fast lookup
                // Key format: "CityName|DistrictName|NeighbourhoodName" (all lowercase, normalized)
                Console.WriteLine("Building lookup dictionary...");
                var postalLookup = new Dictionary<string, string>();
                
                foreach (var item in root.EnumerateArray())
                {
                    if (item.GetArrayLength() >= 5)
                    {
                        string plate = item[0].GetString();
                        string city = item[1].GetString().ToLower().Trim();
                        string district = item[2].GetString().ToLower().Trim();
                        string neighborhood = item[3].GetString().ToLower().Trim();
                        string postalCode = item[4].GetString();
                        
                        // Some neighborhoods in our DB might not have " Mah" suffix, or JSON might have it.
                        // Let's remove " mah" or " köyü" for safer matching, but let's try direct first.
                        string key = $"{city}|{district}|{neighborhood}";
                        postalLookup[key] = postalCode;
                        
                        // Also add a variant without " mah" and " mah." just in case
                        string key2 = $"{city}|{district}|{neighborhood.Replace(" mah.", "").Replace(" mah", "").Replace(" köyü", "")}";
                        postalLookup[key2] = postalCode;
                    }
                }
                
                Console.WriteLine($"Loaded {postalLookup.Count} postal codes into memory.");
                
                Console.WriteLine("Loading neighborhoods from database...");
                // Fetch all neighborhoods with District and City
                var dbNeighborhoods = db.Neighborhoods
                    .Include(n => n.District)
                    .ThenInclude(d => d.City)
                    .ToList();
                    
                Console.WriteLine($"Found {dbNeighborhoods.Count} neighborhoods in DB. Updating postal codes...");
                
                int updatedCount = 0;
                int notFoundCount = 0;
                
                foreach (var n in dbNeighborhoods)
                {
                    string city = n.District.City.CityName.ToLower().Trim();
                    string district = n.District.DistrictName.ToLower().Trim();
                    string neighborhood = n.NeighborhoodName.ToLower().Trim();
                    
                    string key1 = $"{city}|{district}|{neighborhood}";
                    string key2 = $"{city}|{district}|{neighborhood.Replace(" mah.", "").Replace(" mah", "").Replace(" köyü", "")}";
                    
                    if (postalLookup.TryGetValue(key1, out string postalCode) || postalLookup.TryGetValue(key2, out postalCode))
                    {
                        n.PostalCode = postalCode;
                        updatedCount++;
                    }
                    else
                    {
                        // Fallback: try to find any postal code in the same district as a default
                        var fallbackCode = postalLookup.Values.FirstOrDefault(v => postalLookup.Keys.Any(k => k.StartsWith($"{city}|{district}|")));
                        if (fallbackCode != null)
                        {
                            n.PostalCode = fallbackCode;
                            updatedCount++;
                        }
                        else
                        {
                            notFoundCount++;
                        }
                    }
                }
                
                Console.WriteLine($"Updated {updatedCount} records. Not found: {notFoundCount}. Saving changes...");
                
                // Bulk save
                db.SaveChanges();
                
                Console.WriteLine("Successfully updated postal codes!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Error: " + ex.InnerException.Message);
                }
            }
        }
    }
}
