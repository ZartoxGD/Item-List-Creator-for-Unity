using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemListCreator
{
    [System.Serializable]
    public class Item
    {
        #region Vars

        [SerializeField]
        private int id; //unique id of this item
        [SerializeField]
        private string name;
        [SerializeField]
        private string description;
        [SerializeField]
        private bool canCraft; //whether the item can be crafted or not
        [SerializeField]
        private ItemCategory category; //The category in which the item is classified
        [SerializeField]
        private int maxStackable; // The number of times an item can be piled up in a stack
        [SerializeField]
        private Texture2D icon;

        #endregion

        #region Properties

        public int Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Description { get { return description; } set { description = value; } }
        public bool CanCraft { get { return canCraft; } set { canCraft = value; } }
        public ItemCategory Category { get { return category; } set { category = value; } }
        public int MaxStackable { get { return maxStackable; } set { maxStackable = value; } }
        public Texture2D Icon { get { return icon; } set { icon = value; } }

        #endregion

        #region Constructors

        public Item(int _id, string _nom, string _description, bool _craft, ItemCategory _category, int _maxStackable)
        {
            this.id = _id;
            this.name = _nom;
            this.description = _description;
            this.canCraft = _craft;
            this.category = _category;
            this.maxStackable = _maxStackable;
        }

        public Item(int _id, string _nom, string _description, bool _craft, ItemCategory _category, int _maxStackable, Texture2D _icon)
        {
            this.id = _id;
            this.name = _nom;
            this.description = _description;
            this.canCraft = _craft;
            this.category = _category;
            this.maxStackable = _maxStackable;
            this.icon = _icon;
        }

        #endregion

    }
}
