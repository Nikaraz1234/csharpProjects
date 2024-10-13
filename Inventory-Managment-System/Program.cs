using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp15
{

    public interface IStroable
    {
        string Name { get; set; }
        int Quantity { get; set; }
        double Price { get; set; }

        void Restock(int quantity);
        void Sell(int quantity);
       
    }

    public class Item<T> : IStroable
    {
        private static int _Index = 1;

        public int ItemID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public T Category { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public Item(string name, int quantity, double price, T category, DateTime expirationDate)
        {
            ItemID = _Index++;
            Name = name;
            Quantity = quantity;
            Price = price;
            Category = category;
            ExpirationDate = expirationDate;
        }

        public void Restock(int quantity)
        {
            Quantity += quantity;
            Console.WriteLine("Item Restocked.");
        }
        public void Sell(int quantity)
        {
            if(Quantity >= quantity)
            {
                Quantity -= quantity;
                Console.WriteLine($"{quantity} Item Sold.");
            }
            else
            {
                Console.WriteLine("Not Enough Product.");
            }
        }



    }
    public class Inventory<T>
    {
        public List<Item<T>> Items { get; set; }

        public Inventory()
        {
            Items = new List<Item<T>>();
        }

        public void AddItem(Item<T> item)
        {
            Items.Add(item);
            Console.WriteLine("Item Added successfuly.");
        }
        public void RemoveItem(int id)
        {
            Item<T> itemToRemove = Items.Find(i => i.ItemID == id);
            if(itemToRemove != null)
            {
                Items.Remove(itemToRemove);
                Console.WriteLine("Item removed successfuly.");
            }
            else
            {
                Console.WriteLine("Invalid ID.");
            }
        }
        public void GetItemByID(int id)
        {
            Item<T> itemToFind = Items.Find(i => i.ItemID == id);
            if(itemToFind != null)
            {
                Console.WriteLine($"Item - {itemToFind.Name}, With ID - {itemToFind.ItemID}, Price - {itemToFind.Price}, Quantity - {itemToFind.Quantity}.");
            }
            else
            {
                Console.WriteLine("Invalid ID.");
            }
        }
        public List<Item<T>> FilterByCategory(T category)
        {
            return Items.Where(item => item.Category.Equals(category)).ToList();
        }
        public List<Item<T>> FilterByPriceRange(double minPrice, double maxPrice)
        {
            return Items.Where(item => item.Price >= minPrice && item.Price <= maxPrice).ToList();
        }
        public List<Item<T>> FilterByExpiration(DateTime date)
        {
            return Items.Where(item => item.ExpirationDate.HasValue && item.ExpirationDate.Value < date).ToList();
        }

        public async Task RestockMultipleItemsAsync(Dictionary<int, int> restockData)
        {
            foreach(var item in restockData)
            {
                var itemToFound = Items.FirstOrDefault(i => i.ItemID == item.Key);
                if (itemToFound != null)
                {
                    await Task.Run(() =>
                    {
                        itemToFound.Restock(item.Value);
                        GetItemByID(item.Key);
                    });
                }
            }
        }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            Inventory<string> inventory = new Inventory<string>();

            
            inventory.AddItem(new Item<string>("Apple", 50, 0.50, "Fruit", DateTime.Now.AddDays(10)));
            inventory.AddItem(new Item<string>("Banana", 30, 0.30, "Fruit", DateTime.Now.AddDays(5)));
            inventory.AddItem(new Item<string>("Bread", 20, 1.00, "Bakery", DateTime.Now.AddDays(-1))); 

            
            inventory.GetItemByID(1);
            inventory.GetItemByID(3); 
         
            var fruits = inventory.FilterByCategory("Fruit");
            Console.WriteLine("Fruits in inventory:");
            foreach (var item in fruits)
            {
                Console.WriteLine($"- {item.Name}, Quantity: {item.Quantity}");
            }

            
            var affordableItems = inventory.FilterByPriceRange(0.40, 0.60);
            Console.WriteLine("Items priced between $0.40 and $0.60:");
            foreach (var item in affordableItems)
            {
                Console.WriteLine($"- {item.Name}, Price: {item.Price}");
            }

            
            var expiredItems = inventory.FilterByExpiration(DateTime.Now);
            Console.WriteLine("Expired items:");
            foreach (var item in expiredItems)
            {
                Console.WriteLine($"- {item.Name}, Expired on: {item.ExpirationDate}");
            }

            
            var restockData = new Dictionary<int, int>
        {
            { 1, 20 }, 
            { 2, 15 }  
        };
            await inventory.RestockMultipleItemsAsync(restockData);

        }
    }
}
