    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JhutenFPP.Inventory
{
    public enum ItemType
    {
        ITEM_WEAPON,
        ITEM_ARMOR,
        ITEM_CONSUMABLE
    }
    public enum ItemUseType
    {
        ITEM_CONSUMABLE,
        ITEM_EQUIPPABLE
    }
    public class ItemController : MonoBehaviour
    {
        public Item item;
        private bool _itemUsed = false;
        public bool ItemUsed { get { return _itemUsed; } set { _itemUsed = value; } }
    }
}