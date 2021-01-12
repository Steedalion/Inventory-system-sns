using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventory : MonoBehaviour
{
    #region Variable Declarations
    public static CharacterInventory instance;

    public CharacterStats charStats;
    GameObject foundStats;

    public Image[] hotBarDisplayHolders = new Image[4];
    public GameObject InventoryDisplayHolder;
    public Image[] inventoryDisplaySlots = new Image[30];

    int inventoryItemCap = 20;
    int idCount = 1;
    bool addedItem = true;

    public Dictionary<int, InventoryEntry> itemsInInventory = new Dictionary<int, InventoryEntry>();
    public InventoryEntry itemEntry;
    #endregion

    #region Initializations
    void Start()
    {
        instance = this;
        itemEntry = new InventoryEntry(0, null, null);
        itemsInInventory.Clear();

        inventoryDisplaySlots = InventoryDisplayHolder.GetComponentsInChildren<Image>();

        foundStats = GameObject.FindGameObjectWithTag("Player");
        charStats = foundStats.GetComponent<CharacterStats>();
    }
    #endregion

    void Update()
    {
        #region Watch for Hotbar Keypresses - Called by Character Controller Later
        //Checking for a hotbar key to be pressed
        // TODO: Add some keypresses
        #endregion

        //Check to see if the item has already been added - Prevent duplicate adds for 1 item
        if (!addedItem)
        {
            // TODO: try to pick up
        }
    }

    // TODO: Add functions
    
    public void StoreItem(ItemPickUp itemToStore)
    {

    }

    void TryPickUp()
    {

    }

    bool AddItemToInv(bool finishedAdding)
    {
        return true;
    }

    private void AddItemToHotBar(InventoryEntry itemForHotBar)
    {

    }

    void DisplayInventory()
    {

    }

    void FillInventoryDisplay()
    {

    }

    public void TriggerItemUse(int itemToUseID)
    {

    }
}