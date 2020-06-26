using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BagelsInventory
{
    [System.Serializable]
    /// <summary>
    /// An inventory that can be serialized and saved.
    /// </summary>
    public class InventoryData
    {
        public int MaxSize = 20;

        public List<Item> Items = new List<Item>();

        public Dictionary<string, Item> ItemDict = new Dictionary<string, Item>();
    }
    [System.Serializable]
    public class InventoryUISettings
    {
        public GameObject ItemButton;
        public Transform Container;
    }
    public class Inventory : Singleton<Inventory>
    {
        [SerializeField] public InventoryData ThisInv;
        [SerializeField] public InventoryUISettings UISettings;

        #region UI
        public void UpdateUI()
        {
            //clear the children
            foreach(Transform c in UISettings.Container)
            {
                Destroy(c.gameObject);
            }
            //add the updated children
            foreach(Item i in ThisInv.Items)
            {
                var a = Instantiate(UISettings.ItemButton, UISettings.Container);

                string n = i.Name;
                if (i.Stackable)
                    n += "\n(" + i.Amount + ")";
                a.GetComponentInChildren<UnityEngine.UI.Text>().text = n;
                //a.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = n; //for when using text mesh pro
                UnityEngine.UI.Button button = a.GetComponent<UnityEngine.UI.Button>();
                button.onClick.AddListener(delegate { Instance.SelectItem(i.GUID); });
            }
        }
        void SelectItem(string GUID)
        {
            Debug.Log("You've selected " + GUID);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Adds a stackable to the inventory.
        /// </summary>
        /// <param name="itm"></param>
        /// <returns>True if the item was added, false if something went wrong.</returns>
        private bool AddStackedItem(Item itm)
        {
            //check if there is an index in the dictionary for this item yet. remember, stackables use the Name property as their GUID.
            itm.GUID = itm.Name; //in case we make any mistakes with updating the GUID. 
            if (ThisInv.ItemDict.ContainsKey(itm.GUID))
            {
                //this item is already in the dictionary. Just add a value to its amount.
                GetItemByGUID(itm.GUID).Amount += itm.Amount;
                return true;
            }
            else
            {
                //add the item
                if (!InventoryFull())
                {
                    AddItemToThisInv(itm);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Adds item to items list and itemdict
        /// </summary>
        private void AddItemToThisInv(Item itm)
        {
            ThisInv.Items.Add(itm);
            ThisInv.ItemDict.Add(itm.GUID, itm);
        }
        /// <summary>
        /// removes item from items list and itemdict
        /// </summary>
        /// <param name="itm"></param>
        private void RemoveItemFromThisInv(Item itm)
        {
            ThisInv.Items.Remove(itm);
            ThisInv.ItemDict.Remove(itm.GUID);
        }
        #region Private Instance methods
        /// <summary>
        /// Returns true if the inventory is full.
        /// </summary>
        /// 
        private bool inventoryFull()
        {
            if (ThisInv.Items.Count >= ThisInv.MaxSize)
                return true;
            return false;
        }
        /// <summary>
        /// Adds an item to the inventory. The item added will NOT be cloned, which means the added item will be a pointer.
        /// </summary>
        /// <param name="itm">The item to add</param>
        /// <returns>True if the item was added, false if there was a problem. </returns>
        private bool addItem(Item itm)
        {
            //in case someone forgets to set the amount. 
            if (itm.Amount == 0)
                itm.Amount = 1;
            //first, check if the item is stackable.
            if (itm.Stackable)
            {
                return AddStackedItem(itm);
            }
            else
            {
                if (!inventoryFull())
                {
                    AddItemToThisInv(itm);
                    return true;
                }
            }
            return false; //if we haven't returned by now, something went wrong.
        }
        /// <summary>
        /// Adds a cloned item to the inventory. The added item will be cloned, meaning the other item will not be linked to the added item.
        /// </summary>
        /// <param name="itm">The item to add</param>
        private bool addClonedItem(Item itm)
        {
            return addItem(itm.DeepClone());
        }
        /// <summary>
        /// Removes an item by its GUID. Returns the remaining amount of that item.
        /// </summary>
        /// <param name="GUID">The GUID of the item. If the item is stackable, the GUID will be the same as the item's name.</param>
        /// <param name="amt">The amount to remove. Default = 1. </param>
        private int removeItem(string GUID, int amt)
        {
            if (ThisInv.ItemDict.ContainsKey(GUID))
            {
                Item i = GetItemByGUID(GUID);
                i.Amount -= amt;
                //remove item if amt is less than or equal to zero
                if (i.Amount <= 0)
                    RemoveItemFromThisInv(i);
                return i.Amount;
            }
            //item was not found
            return 0;
        }
        /// <summary>
        /// Gets an item by it's GUID. This method is faster than getting it by name.
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        private Item getItemByGUID(string GUID)
        {
            if (ThisInv.ItemDict.ContainsKey(GUID))
                return ThisInv.ItemDict[GUID];
            return null;
        }
        /// <summary>
        /// Gets an item by its name. Will return the first item in the inventory that is found with a matching name. 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private Item getItemByName(string Name) //not case sensitive
        {
            Item found = null;
            foreach (Item i in ThisInv.Items)
            {
                if (i.Name.ToLower() == Name.ToLower() && found == null)
                {
                    found = i;
                }
            }
            return found;
        }
        #endregion
        #region Static Methods
        /// <summary>
        /// Gets an item by its name. Will return the first item in the inventory that is found with a matching name. 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static Item GetItemByName(string Name)
        {
            return Instance.getItemByName(Name);
        }
        /// <summary>
        /// Gets an item by it's GUID. This method is faster than getting it by name.
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public static Item GetItemByGUID(string GUID)
        {
            return Instance.getItemByGUID(GUID);
        }
        /// <summary>
        /// Removes an item by its GUID. Returns the remaining amount of that item.
        /// </summary>
        /// <param name="GUID">The GUID of the item. If the item is stackable, the GUID will be the same as the item's name.</param>
        public static int RemoveItem(string GUID)
        {
            return Instance.removeItem(GUID, 1);
        }
        /// <summary>
        /// Removes an item by its GUID. Returns the remaining amount of that item.
        /// </summary>
        /// <param name="GUID">The GUID of the item. If the item is stackable, the GUID will be the same as the item's name.</param>
        /// <param name="amt">The amount to remove. Default = 1. </param>
        public static int RemoveItem(string GUID, int amt)
        {
            return Instance.removeItem(GUID, amt);
        }

        public static bool AddClonedItem(Item itm)
        {
            return Instance.addClonedItem(itm);
        }
        public static bool AddItem(Item itm)
        {
            return Instance.addItem(itm);
        }
        public static bool InventoryFull()
        {
            return Instance.inventoryFull();
        }
        #endregion
        #endregion
    }

}
