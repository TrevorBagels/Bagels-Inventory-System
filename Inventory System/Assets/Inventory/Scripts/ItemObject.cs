using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace BagelsInventory
{
    [System.Serializable]
    public class Item
    {
        public string Name;
        public bool Stackable;
        public string GUID; //Note: When an item is stackable, the GUID will be equal to the item's name.
        public int Amount;
        public List<Property> Properties = new List<Property>();
        Dictionary<string, Property> propertyDict = new Dictionary<string, Property>(); //used to quickly access properties

        /// <summary>
        /// Gets a string value from a specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <returns></returns>
        public string GetPropertyTxt(string propertyName)
        {
            if (propertyDict.ContainsKey(propertyName))
                return propertyDict[propertyName].StringValue;
            return "";
        }
        /// <summary>
        /// Gets a float from a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns></returns>
        public float GetPropertyValue(string propertyName)
        {
            if (propertyDict.ContainsKey(propertyName))
                return propertyDict[propertyName].Value;
            return 0;
        }
        void UpdatePropertyDict()
        {
            propertyDict = new Dictionary<string, Property>();
            foreach (Property p in Properties)
            {
                propertyDict.Add(p.Name, p);
            }
        }
        /// <summary>
        /// Fixes GUID issues, amount issues, and property issues. 
        /// </summary>
        public void FixIssues()
        {
            UpdatePropertyDict();
            if (Amount == 0)
                Amount = 1;
            if (GUID.Length < 1)
                RegenerateGUID();
        }
        /// <summary>
        /// Regenerates the GUID for this item. If stackable, the GUID will be the same as the item's name.
        /// </summary>
        public void RegenerateGUID()
        {
            if (!Stackable)
                GUID = System.Guid.NewGuid().ToString();
            else
                GUID = Name;
        }
    }
    [System.Serializable]
    public class Property
    {
        public string Name;
        public float Value;
        public string StringValue;
    }

    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
    public class ItemObject : ScriptableObject
    {
        public Item ThisItem;



        //Since this is a scriptable object, we don't want to give a pointer to the pre defined settings.
        /// <summary>
        /// This method should be used when adding ThisItem to the inventory. This will also auto generate the GUID of the item
        /// </summary>
        /// <returns></returns>
        public Item GetItem()
        {
            var newitem = ThisItem.DeepClone();
            newitem.FixIssues();
            return newitem;
        }
    }

}
