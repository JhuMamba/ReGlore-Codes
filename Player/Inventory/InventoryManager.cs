using JhutenFPP.Manager;
using JhutenFPP.PlayerControl;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace JhutenFPP.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance;

        public Camera ProfileCamera;

        public List<Item> Items;
        public Transform ItemContent;
        public GameObject InventoryItem;

        public List<Item> ChestItems;
        public Transform ChestItemContent;
        
        public GameObject InventoryPanel;
        public GameObject ChestPanel;

        public InputControl InputControl;
        public Toggle EnableRemove;

        public InventoryItemController[] InventoryItems;
        public InventoryItemController[] ChestItemControllers;

        private bool _isInitStarter = false;
        public bool IsChestOpen { get; private set; }
        public List<ItemController> Swords;
        public List<ItemController> Armours;
        public List<Item> EquippedItems;
        public List<Item> StarterItems;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            IsChestOpen = false;
        }
        private void Start()
        {
            CheckEquippedItems();
        }
        private void Update()
        {
            if (_isInitStarter) return;
            else InitStarterItems();
        }
        public void Add(Item item)
        {
            Items.Add(item);
        }
        public void Remove(Item item) 
        {
            List<Item> newItems = Items;
            newItems.Remove(item);
            Items = newItems;
            ListItems();
        }

        public void ListItems()
        {
            foreach (Transform item in ItemContent)
            {
                Destroy(item.gameObject);
            }
            foreach (Item item in Items)
            {
                GameObject obj = Instantiate(InventoryItem, ItemContent);
                var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
                var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                var equippedIcon = obj.transform.Find("Equipped");
                var removeButton = obj.transform.Find("RemoveButton").gameObject;

                itemName.text = item.itemName;
                itemIcon.sprite = item.icon;
                if (EquippedItems.Contains(item)) equippedIcon.gameObject.SetActive(true);
                obj.GetComponent<InventoryItemController>().AddItem(item);

                if (EnableRemove.isOn) removeButton.SetActive(true);
            }
        }
        public void ListChestItems()
        {
            foreach (Transform item in ChestItemContent)
            {
                Destroy(item.gameObject);
            }
            foreach (Item item in ChestItems)
            {
                GameObject obj = Instantiate(InventoryItem, ChestItemContent);
                var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
                var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                var removeButton = obj.transform.Find("RemoveButton").gameObject;

                itemName.text = item.itemName;
                itemIcon.sprite = item.icon;
                obj.GetComponent<InventoryItemController>().AddItem(item);

                if (EnableRemove.isOn) removeButton.SetActive(true);
            }
        }
        public void OpenChest(List<Item> newChestItems)
        {
            ChestItems = newChestItems;
            if (InputControl.Interact)
            {
                IsChestOpen = true;
                ChestPanel.SetActive(true);
                InventoryPanel.SetActive(true);
                ListItems();
                ListChestItems();
                ProfileCamera.gameObject.SetActive(true);
                InputControl.EnableCursor();
            }
        }
        public void CloseChest()
        {
            IsChestOpen = false;
            ChestPanel.SetActive(false);
            ProfileCamera.gameObject.SetActive(false);
            InputControl.HideCursor();
        }
        public void OpenInventory()
        {
            if (InputControl.Inventory)
            {
                InventoryPanel.SetActive(true);
                ListItems();
                InputControl.EnableCursor();
                ProfileCamera.gameObject.SetActive(true);
            }
            else
            {
                InventoryPanel.SetActive(false);
                InputControl.HideCursor();
                ProfileCamera.gameObject.SetActive(false);
            } 
        }
        public void TakeChestItems()
        {
            foreach(Item itm in ChestItems)
            {
                Add(itm);
            }
            ChestItems.Clear();
            ListChestItems();
            ListItems();
        }
        public void EnableItemsRemove()
        {
            if (EnableRemove.isOn)
            {
                foreach (Transform item in ItemContent)
                {
                    item.Find("RemoveButton").gameObject.SetActive(true);
                }
            }
            else
            { 
                foreach(Transform item in ItemContent)
                {
                    item.Find("RemoveButton").gameObject.SetActive(false);
                }
            }
        }
        public void SetInventoryItems()
        {
            InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();
            for (int i  = 0; i < Items.Count; i++)
            {
                InventoryItems[i].AddItem(Items[i]);
            }
        }
        public void InitStarterItems()
        {
            foreach (Item item in StarterItems)
            {
                Add(item);
                if (item.itemType == ItemType.ITEM_WEAPON) EquipSword(item);
            }
            _isInitStarter = true;
            CheckEquippedItems();
        }
        public void CheckEquippedItems()
        {
            EquippedItems.Clear();
            for (int i = 0; i < Swords.Count; i++) if (Swords[i].gameObject.GetComponent<SwordAttack>().isEquipped) EquippedItems.Add(Swords[i].item);
            for (int i = 0; i < Armours.Count; i++) if (Armours[i].gameObject.activeInHierarchy) EquippedItems.Add(Armours[i].item);
        }
        public void UseItem(Item item)
        {
            switch (item.itemType)
            {
                case ItemType.ITEM_WEAPON:
                    EquipSword(item);
                    break;
                case ItemType.ITEM_ARMOR:
                    EquipArmor(item);
                    break;
                case ItemType.ITEM_CONSUMABLE:
                    break;
            }
            CheckEquippedItems();
            ListItems();
        }
        public void EquipSword(Item item)
        {
            foreach (ItemController sword in Swords)
            {
                if (sword.item.id == item.id)
                {
                    PlayerController.Instance.SetWeapon(sword.gameObject);
                }
            } 
        }
        public void EquipArmor(Item item)
        {
            int armorIndex = item.id % 10;
            int oldArmorId = -1;
            int oldArmorIndex = -1;
            foreach (Item itm in EquippedItems)
            {
                if (itm.itemType == ItemType.ITEM_ARMOR && itm.id % 10 == armorIndex) oldArmorId = itm.id;
            }
            foreach (ItemController armor in Armours)
            {
                if (armor.item.id == oldArmorId) 
                { 
                    armor.gameObject.SetActive(false);
                    oldArmorIndex = Armours.IndexOf(armor);
                }
                if (armor.item.id == item.id)
                {
                    EquippedItems.Add(item);
                    armor.gameObject.SetActive(true);
                    if (oldArmorIndex != -1) EquippedItems.RemoveAt(oldArmorIndex);
                }
            }
        }
        bool isTested = false;
        public void TestInventory()
        {
            if (isTested) return;
            isTested = true;
            Add(Swords[1].GetComponent<ItemController>().item);
            UseItem(Swords[1].GetComponent<ItemController>().item);
        }
    }
}