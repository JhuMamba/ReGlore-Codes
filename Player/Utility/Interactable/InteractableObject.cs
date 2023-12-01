using JhutenFPP.Inventory;
using JhutenFPP.PlayerControl;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableObject : MonoBehaviour,IInteractable
{
    public List<Item> Items;
    public void InteractWithObj()
    {
        InventoryManager.Instance.OpenChest(Items);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) PlayerController.Instance.InteractableObj = this;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) PlayerController.Instance.InteractableObj = null;
    }
}
