using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace MovieRecommendationSystem.Utilities
{
    public static class FileManager
    {
        public static void SaveToFile<T>(string path, List<T> data)
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    data,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(path, json);

                Console.WriteLine("Data Saved Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Saving File: {ex.Message}");
            }
        }

        public static List<T> LoadFromFile<T>(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return new List<T>();
                }

                string json = File.ReadAllText(path);

                return JsonSerializer.Deserialize<List<T>>(json)
                       ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Loading File: {ex.Message}");
                return new List<T>();
            }
        }
    }
}