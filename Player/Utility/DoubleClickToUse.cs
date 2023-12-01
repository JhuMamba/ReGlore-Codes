using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DoubleClickToUse : MonoBehaviour
{
    private InventoryItemController _inventoryItemController;
    private float _lastClickedTime;
    private float _doubleClickTreshold = 0.3f;
    private void Awake()
    {
        _inventoryItemController = GetComponent<InventoryItemController>();
    }
    public void OnClick()
    {
        if (_inventoryItemController == null) return;
        float currentTime = Time.time;
        if (currentTime - _lastClickedTime > _doubleClickTreshold) { _lastClickedTime = currentTime; return; }
        _inventoryItemController.Use();
        _lastClickedTime = currentTime;
    }
}
