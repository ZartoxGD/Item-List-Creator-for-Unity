using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ItemListCreator
{
    public class ItemManager : MonoBehaviour
    {
        //The file i made with the Item List Creator tool
        public TextAsset jsonFile;
        //The dictionary containing every items of the game
        public Dictionary<int, Item> allItemsInGame;

        // Start is called before the first frame update
        private void Awake()
        {
            //Set the dictionary up and retrieve data in the file selected
            allItemsInGame = new Dictionary<int, Item>();
            Item[] items = JsonHelper.FromJson<Item>(jsonFile.text);

            foreach (Item item in items)
            {
                AddToDictionaryByID(item);
            }
        }

        //Add an item to the dictionnary at ID index
        private void AddToDictionaryByID(Item item)
        {
            allItemsInGame.Add(item.Id, item);
        }

    }

}
