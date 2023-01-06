using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemListCreator
{
    [System.Serializable]
    public class Inventory
    {
        //This is a really simple inventory class 

        public List<Item> items;

        public Inventory(List<Item> _items)
        {
            this.items = _items;
        }   

        //Add an item into the inventory
        public void AddItem(Item item)
        {
            items.Add(item);
            Debug.Log($"Added new item: {item.Name}(ID:{item.Id})");
        }

        //Remove an item in the inventory
        public void RemoveItem(Item item)
        {
            items.Remove(item);
            Debug.Log($"Removed new item: {item.Name}(ID:{item.Id})");
        }

        //Print all the objects contained by the inventory
        public void DebugInventory()
        {
            Debug.Log("The inventory contains: ");

            //If the inventory is empty print it
            if(items.Count <= 0)
            {
                Debug.Log("The inventory is empty");
                return;
            }

            foreach (Item item in items)
            {
                Debug.Log($"{item.Name}(ID:{item.Id})");
            }          
        }
    }
}

