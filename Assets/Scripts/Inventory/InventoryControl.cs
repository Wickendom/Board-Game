using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryControl : MonoBehaviour {

    public static InventoryControl Instance;

    private PlayerInventory inventory;

    InventoryCell transferFromCell;

    GameObject UIObjectToFollowMouse;

    [SerializeField]
    private LayerMask UIButtonMask;

    [SerializeField]
    private bool isDraggingItem;

    public RectTransform canvas;

    //public SaveLists saveLists;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        inventory = GetComponentInChildren<PlayerInventory>();
        //saveLists = SaveLists.Instance;
    }

    private void OnEnable()
    {
        //SaveLists.LoadEvent += LoadInventories;
    }

    private void OnDisable()
    {
        //SaveLists.LoadEvent -= LoadInventories;
    }

    private void Update()
    {
        if(true)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(UIController.Instance.UICamera.ScreenToWorldPoint(Input.mousePosition).x, UIController.Instance.UICamera.ScreenToWorldPoint(Input.mousePosition).y), new Vector2(0, 0), 0f, UIButtonMask);

            Debug.DrawRay(new Vector2(UIController.Instance.UICamera.ScreenToWorldPoint(Input.mousePosition).x, UIController.Instance.UICamera.ScreenToWorldPoint(Input.mousePosition).y), new Vector2(0, 0));


            if (hit && Input.GetMouseButtonDown(0))
            {
                isDraggingItem = true;
                transferFromCell = hit.collider.GetComponent<InventoryCell>();
                UIObjectToFollowMouse = Instantiate(hit.collider.gameObject, new Vector3(Input.mousePosition.x,Input.mousePosition.y, 4.1f), Quaternion.identity, canvas);
                InventoryCell tempCell = UIObjectToFollowMouse.GetComponent<InventoryCell>();
                InventoryCell hitCell = hit.collider.GetComponent<InventoryCell>();
                tempCell.item = hitCell.item;
                tempCell.itemsInStack = hitCell.itemsInStack;
            }
        }

        if (isDraggingItem && UIObjectToFollowMouse != null)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 4.1f;
            UIObjectToFollowMouse.transform.position = UIController.Instance.UICamera.ScreenToWorldPoint(pos);
        }

        if(Input.GetMouseButtonUp(0) && isDraggingItem)
        {
            isDraggingItem = false;

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(UIController.Instance.UICamera.ScreenToWorldPoint(Input.mousePosition).x, UIController.Instance.UICamera.ScreenToWorldPoint(Input.mousePosition).y), new Vector2(0, 0), 0f, UIButtonMask);

            if (hit)
            {
                InventoryCell transferToCell = hit.collider.GetComponent<InventoryCell>();
                transferToCell.CheckIfItemIsACompatibleInputItem(transferFromCell);

                if(transferFromCell.item != null)
                {
                    transferToCell.AddItem(transferFromCell.item);
                    transferFromCell.RemoveItemCompletely();
                }
                else
                {
                    transferToCell.UpdateCellUI(transferToCell.item, true);
                    transferFromCell.UpdateCellUI(null, true);
                }
                
            }

            Destroy(UIObjectToFollowMouse.gameObject);
            UIObjectToFollowMouse = null;
            transferFromCell = null;
        }
    }

    public void AddItemToInventory(Item item)
    {
        if(inventory.itemsInInventory < inventory.cells.Count)
        {
           // print(inventory.name);
            inventory.AddItem(item);
        }
    }

    public void AddItemToInventory(Item item, int amount)
    {
        if (inventory.itemsInInventory < inventory.cells.Count)
        {
            // print(inventory.name);
            for (int i = 0; i < amount; i++)
            {
                inventory.AddItem(item);
            }
        }
    }

    public bool AddItemToUnbuiltBuilding(Item item)
    {

        for (int i = 0; i < inventory.cells.Count; i++)
        {
            if (inventory.cells[i].item == item)
            {
                RemoveItem(i);
                return true;
            }
        }
        

        return false;
    }

    public bool CheckPlayerHasItems(Item item, int itemAmount)
    {
        //TO DO: Change this script so it can get the same resource from multiple cells.
        for (int j = 0; j < inventory.cells.Count; j++)
        {
            if (inventory.cells[j].item == item && inventory.cells[j].itemsInStack >= itemAmount)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveItem(int cellIndex)
    {
        if(inventory.cells[cellIndex].itemsInStack > 1)
        {
            inventory.cells[cellIndex].itemsInStack--;
            inventory.cells[cellIndex].stackCountUI.text = inventory.cells[cellIndex].itemsInStack.ToString();
        }
        else
        {
            inventory.cells[cellIndex].itemsInStack--;
            inventory.cells[cellIndex].item = null;
            inventory.cells[cellIndex].stackCountUI.text = "";
            inventory.cells[cellIndex].buttonText.text = "Empty";
        }
    }

    public void RemoveItem(Item item, int amountToRemove)
    {
        int amountLeftToRemove = amountToRemove;

        for (int i = 0; i < inventory.cells.Count; i++)
        {
            if (inventory.cells[i].item == item)
            {
                int itemscurrentlyInStack = inventory.cells[i].itemsInStack;

                for (int z = 0; z < itemscurrentlyInStack; z++)
                {
                    if (amountLeftToRemove > 0)
                    {
                        amountLeftToRemove--;
                        RemoveItem(i);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    /*public void LoadInventories(object reciever, System.EventArgs args)
    {
        for (int i = 0; i < inventories.Length; i++)
        {
            for (int j = 0; j < inventory.cells.Count; j++)
            {
                inventory.cells[j].item = ItemManager.Instance.FindItemInInventoryByID(saveLists.savedInventoryList.savedinventory.item[j]);
                inventory.cells[j].itemsInStack = saveLists.savedInventoryList.savedinventory.itemsInStack[j];
                inventory.UpdateCellUIFromCellData(j);
            } 
        }
    }*/
}
