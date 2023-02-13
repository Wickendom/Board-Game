using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryCell : MonoBehaviour {
    public Item item; //the item this cell is currently holding. THIS NEEDS SETTING PRIVATE AND ONLY NEEDS ACCESSING THROUGH FUNCTIONS TO STOP UI UPDATE ISSUES.
    private Image image;
    private Button button;
    private BoxCollider2D col;
    public Text buttonText;
    public TextMeshProUGUI stackCountUI;
    public int itemsInStack = 0;
    public bool hasItem;

    private void Awake()
    {
        buttonText = GetComponentInChildren<Text>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        col = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        if(stackCountUI)
            stackCountUI.enabled = false;
        
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void UpdateCellUI(Item item, bool updateStackUI)
    {
        //button.onClick.AddListener(useableItem.Use);
        if(item != null)
        {
            buttonText.text = item.itemName;

            if (updateStackUI)
            {
                if (stackCountUI.enabled == false)
                    stackCountUI.enabled = true;
                stackCountUI.text = itemsInStack.ToString();
            }

            if(itemsInStack <= 1)
            {
                stackCountUI.enabled = false;
            }
        }
        else
        {
            buttonText.text = "Empty";

            if (updateStackUI)
            {
                if(stackCountUI)
                {
                    if (stackCountUI.enabled == true)
                        stackCountUI.enabled = false;
                    stackCountUI.text = itemsInStack.ToString();
                }
                else
                {
                    Debug.Log("There is no Stack Count UI in inventory slot");
                }
            }
        }
    }

    public virtual void AddItem(Item item)
    {
        if (this.item == null)
        {
            this.item = item;
            itemsInStack = 1;
            UpdateCellUI(this.item, false);
        }
        else if(this.item == item)
        {
            itemsInStack++;
            UpdateCellUI(this.item, true);
        }
        else if(this.item != item)
        {
            Debug.Log("Items do not match");
        }
        hasItem = true;
    }

    public void AddItem(Item item, int itemAmount)
    {
        if (this.item == null)
        {
            this.item = item;
            itemsInStack = itemAmount;
            UpdateCellUI(this.item, (itemAmount >= 2)?true:false);
        }
        else if (this.item != item)
        {
            Debug.Log("Items do not match");
        }
        hasItem = true;
    }

    public bool CheckIfItemIsACompatibleInputItem(InventoryCell swapWithCell)
    {
        SwapItems(swapWithCell);
        return true;
    }
    
    private void SwapItems(InventoryCell swapWithCell)
    {
        Item tempItem = swapWithCell.item;
        int tempItemStack = swapWithCell.itemsInStack;

        swapWithCell.SetItem(item, itemsInStack);
        SetItem(tempItem, tempItemStack);
    }

    public void SetItem(Item item, int itemAmount)
    {
        this.item = item;
        itemsInStack = itemAmount;
        hasItem = (this.item != null) ? true : false;
        UpdateCellUI(this.item, true);
    }

    public void RemoveItem()
    {
        if(itemsInStack > 1)
        {
            itemsInStack--;
            UpdateCellUI(item,true);
        }
        else if(itemsInStack == 1)
        {
            item = null;
            UpdateCellUI(item, true);
            hasItem = false;
        }
    }

    public void RemoveItemCompletely()
    {
        item = null;
        UpdateCellUI(item, true);
        hasItem = false;
    }

    public void DisableCell()
    {
        //image.enabled = false;
        //button.enabled = false;
        //col.enabled = false;
        //buttonText.enabled = false;
    }

    public void EnableCell()
    {
        //image.enabled = true;
        //button.enabled = true;
        //col.enabled = true;
        //buttonText.enabled = true;
    }
}
