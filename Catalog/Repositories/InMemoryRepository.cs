﻿using Catalog.Models;
using System;

namespace Catalog.Repositories
{
    public class InMemoryRepository : IItemsRepository
    {
        private readonly List<Item> items = new()
        {
            new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreationDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Iron Sword", Price = 20, CreationDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 18, CreationDate = DateTimeOffset.UtcNow }
        };

        public IEnumerable<Item> GetItems()
            => items;

        public Item GetItem(Guid id)
            => items.Where(item => item.Id == id).FirstOrDefault();

        public void CreateItem(Item item)
            => items.Add(item);

        public void UpdateItem(Item item)
        {
            var index = items.FindIndex(existingItem => existingItem.Id == item.Id);
            items[index] = item;
        }

        public void DeleteItem(Guid id)
        {
            var index = items.FindIndex(item => item.Id == id);
            items.RemoveAt(index);
        }
    }
}
