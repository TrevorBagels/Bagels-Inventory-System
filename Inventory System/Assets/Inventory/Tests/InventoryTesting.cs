using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelsInventory;
public class InventoryTesting : MonoBehaviour
{
    public ItemPreset ItemRef;
    public ItemPreset ItemRefToStackable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Inventory.AddItem(ItemRef.GetItem());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Inventory.AddItem(ItemRefToStackable.GetItem());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Inventory.RemoveItem(ItemRefToStackable.ThisItem.Name);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            Inventory.Instance.UpdateUI();
        }
    }
}
