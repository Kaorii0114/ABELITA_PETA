using System;
using System.Collections.Generic;
using System.IO;

namespace PetaInventorySystem
{
    internal class InventoryManager
    {
        enum Category
        {
            Electronics, 
            Groceries, 
            Clothing, 
            Stationery, 
            Miscellaneous
        }

        struct Product
        {
            public string Name;
            public int Quantity;
            public Category CategoryType;

            public Product(string name, int quantity, Category categoryType)
            {
                Name = name;
                Quantity = quantity;
                CategoryType = categoryType;
            }

            public override string ToString()
            {
                return $"Product Name: {Name}, Quantity: {Quantity}, Category: {CategoryType}";
            }

            public string ToFileFormat()
            {
                return $"{Name},{Quantity},{CategoryType}";
            }

            public static Product FromFileFormat(string line)
            {
                var parts = line.Split(',');
                if (parts.Length != 3)
                    throw new FormatException("Invalid line format");

                string name = parts[0];
                int quantity = int.Parse(parts[1]);
                Category categoryType = (Category)Enum.Parse(typeof(Category), parts[2]);

                return new Product(name, quantity, categoryType);
            }
        }

        static void Main(string[] args)
        {
            string filePath = @"C:\Klark\c#\abelita-peta\inventory.txt";

            Console.WriteLine("Inventory of Patrick");
           
            List<Product> productList = LoadProducts(filePath);

            string input;

            do
            {
                Console.Write("Enter product name (or type 'exit' to finish): ");
                input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    break;
                }

                string productName = input;

                Console.Write("Enter product quantity: ");
                int productQuantity;
                while (!int.TryParse(Console.ReadLine(), out productQuantity) || productQuantity < 0)
                {
                    Console.WriteLine("Invalid input. Please enter a valid quantity (non-negative integer):");
                }

                Console.WriteLine("Select product category:");
                foreach (var category in Enum.GetValues(typeof(Category)))
                {
                    Console.WriteLine($"{(int)category}. {category}");
                }

                int categorySelection;
                while (!int.TryParse(Console.ReadLine(), out categorySelection) || categorySelection < 0 || categorySelection >= Enum.GetValues(typeof(Category)).Length)
                {
                    Console.WriteLine("Invalid choice. Please select a valid category number:");
                }

                Category selectedCategory = (Category)categorySelection;

                Product newProduct = new Product(productName, productQuantity, selectedCategory);
                productList.Add(newProduct);

                Console.WriteLine("Product added successfully!");

            } while (true);

            Console.WriteLine("\nCurrent Inventory:");
            foreach (var product in productList)
            {
                Console.WriteLine(product);
            }

            SaveProducts(filePath, productList);
        }

        static List<Product> LoadProducts(string filePath)
        {
            List<Product> productList = new List<Product>();
            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        try
                        {
                            Product product = Product.FromFileFormat(line);
                            productList.Add(product);
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"Error loading product: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No previous inventory found.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading the inventory file: {ex.Message}");
            }

            return productList;
        }

        static void SaveProducts(string filePath, List<Product> productList)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var product in productList)
                    {
                        writer.WriteLine(product.ToFileFormat());
                    }
                }
                Console.WriteLine("Inventory saved successfully!");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving the inventory file: {ex.Message}");
            }
        }
    }
}