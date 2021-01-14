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
	    if(Input.GetKeyDown("i"))
	    {
	    	DisplayInventory();
	    }
        #endregion

        //Check to see if the item has already been added - Prevent duplicate adds for 1 item
        if (!addedItem)
        {
	        TryPickUp();
        }
    }

    // TODO: Add functions
    
    public void StoreItem(ItemPickUp itemToStore)
	{
		addedItem = false;
		
		bool canCarry = (charStats.characterDefinition.currentEncumbrance + itemToStore.itemDefinition.itemWeight)
			<= charStats.characterDefinition.maxEncumbrance;
		if(canCarry)
		{
			itemEntry.invEntry = itemToStore;
			itemEntry.stackSize = 1;
			itemEntry.hbSprite = itemToStore.itemDefinition.itemIcon;
			
			itemToStore.gameObject.SetActive(false);
		}

    }

    void TryPickUp()
	{
		addedItem = AddItemToInv(addedItem);
	
		bool isPickedUp = false;
		if(itemEntry.invEntry)
		{
			bool isInventoryEmpty = (itemsInInventory.Count == 0);
			if (isInventoryEmpty)
			{
				addedItem = AddItemToInv(addedItem);
			}
			else
			{
				if(itemEntry.invEntry.itemDefinition.isStackable)
				{
					foreach (KeyValuePair<int, InventoryEntry> item in itemsInInventory)
					{
						if(itemEntry.invEntry.itemDefinition == item.Value.invEntry.itemDefinition)
						{
							item.Value.stackSize++;
							AddItemToHotBar(item.Value);
							isPickedUp = true;
							DestroyObject(itemEntry.invEntry.gameObject);
							break;
						}
						else
						{
							isPickedUp = false;
						}
					}
					
				}	else //not stackable
				{
					isPickedUp = false;
					if(itemsInInventory.Count == inventoryItemCap)
					{
						itemEntry.invEntry.gameObject.SetActive(true);
						Debug.Log("Inventor is full");
					}
				}
				//if item is not stackable then continue here???????
				if(!isPickedUp)
				{
					addedItem = AddItemToInv(addedItem);
					isPickedUp = true;
				}
				
			}
		}

    }

    bool AddItemToInv(bool finishedAdding)
	{
		InventoryEntry inventoryCopy = new InventoryEntry(itemEntry.stackSize,
			Instantiate(itemEntry.invEntry),
			itemEntry.hbSprite);
		itemsInInventory.Add(idCount, inventoryCopy);
		
		DestroyObject(itemEntry.invEntry.gameObject);
		FillInventoryDisplay();
		AddItemToHotBar(itemsInInventory[idCount]);
		idCount = FindSmallestUnusedID();
		
        return true;
	}
	int FindSmallestUnusedID()
	{
		int newID = 1;
		
		for (int itemCount = 1; itemCount <= itemsInInventory.Count; itemCount++) {
			if(itemsInInventory.ContainsKey(newID))
			{
				newID++;
			} else
			{
				return newID;
			}
		}
		return newID;
	}

    private void AddItemToHotBar(InventoryEntry itemForHotBar)
	{
		int hotBarCounter = 0;
		bool increaseCount = false;
		
		foreach (Image image in hotBarDisplayHolders)
		{
			if (image.sprite == null)
			{
				itemForHotBar.hotBarSlot = hotBarCounter;
				image.sprite = itemForHotBar.hbSprite;
				increaseCount = true;
				break;
			} else if (itemForHotBar.invEntry.itemDefinition.isStackable)
			{
				increaseCount = true;
				
			}
		}
		if(increaseCount)
		{
			hotBarDisplayHolders[itemForHotBar.hotBarSlot - 1].GetComponentInChildren<Text>().text = itemForHotBar.stackSize.ToString();
		}
		
		increaseCount = false;
    }

    void DisplayInventory()
	{
		if(InventoryDisplayHolder.activeSelf == true)
		{
			InventoryDisplayHolder.SetActive(false);
		} else{
			InventoryDisplayHolder.SetActive(true);
		}
    }

    void FillInventoryDisplay()
	{
		int slotCounter = 9;
		foreach (var item in itemsInInventory)
		{
			slotCounter++;
			inventoryDisplaySlots[slotCounter].sprite = item.Value.hbSprite;
			item.Value.inventorySlot = slotCounter - 9;
		}
		
		while (slotCounter<29)
		{
			slotCounter++;
			inventoryDisplaySlots[slotCounter].sprite = null;
		}

    }

    public void TriggerItemUse(int itemToUseID)
	{
		bool triggerItem = false;
		foreach (var item in itemsInInventory)
		{
			if(itemToUseID > 100)
			{
				itemToUseID -= 100;
				if(item.Value.hotBarSlot == itemToUseID)
				{
					triggerItem = true;
				}
			}
			else
			{
				if(item.Value.inventorySlot == itemToUseID)
				{
					triggerItem = true;
				}
			}
			if(triggerItem)
			{
				if(item.Value.stackSize == 1)
				{
					if (item.Value.invEntry.itemDefinition.isStackable)
					{
						if(item.Value.hotBarSlot != 0)
						{
							hotBarDisplayHolders[item.Value.hotBarSlot -1].sprite =null;
							hotBarDisplayHolders[item.Value.hotBarSlot -1].GetComponent<Text>().text ="0";
						}
						
						item.Value.invEntry.UseItem();
						itemsInInventory.Remove(item.Key);
						break;
					}
					else
					{
						item.Value.invEntry.UseItem();
						if(!item.Value.invEntry.itemDefinition.isIndestructable)
						{
							itemsInInventory.Remove(item.Key);
							break;
						}
					}
				}
				else
				{
					item.Value.invEntry.UseItem();
					item.Value.stackSize --;
					hotBarDisplayHolders[item.Value.hotBarSlot -1].GetComponent<Text>().text =item.Value.stackSize.ToString();
					break;
				}
			}
		}
		FillInventoryDisplay();
    }
}