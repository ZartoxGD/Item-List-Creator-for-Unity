using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemListCreator
{
    public class Player : MonoBehaviour
    {
        [Header("Inventory")][Space(10)]
        public Inventory playerInventory;
        [Header("Reference")][Space(10)]
        public ItemManager referenceToItemManager;

    // Start is called before the first frame update
        private void Start()
        {
           List<Item> newInventoryItems = new List<Item>();

            playerInventory = new Inventory(newInventoryItems);

            playerInventory.DebugInventory();
        }

        // Update is called once per frame
        void Update()
        {
            //If left Click is pressed add to inventory
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerInventory.AddItem(referenceToItemManager.allItemsInGame[1]);
            }

            //If right click is pressed remove from the inventory
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                playerInventory.RemoveItem(referenceToItemManager.allItemsInGame[1]);
            }

            //If Space is pressed print the whole inventory to the console
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerInventory.DebugInventory();
            }            
        }
    }
}

