using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {

    public bool isQuickBar;

    public int itemsInInventory = 0;

    public List<InventoryCell> cells;

    public RectTransform backgroundTransform;

    [SerializeField]
    private int cellColumnAmount, cellRowAmount;

    [SerializeField]
    private GameObject cellObject;

    [SerializeField]
    private float cellWidth, cellHeight;
    [SerializeField]
    private float cellPadding;


    public void Start()
    {
        CreateInventoryList();
    }

    private void OnEnable()
    {
        //SaveLists.SaveEvent += SaveFunction;
    }

    private void OnDisable()
    {
        //SaveLists.SaveEvent -= SaveFunction;
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < cells.Count - 1; i++)
        {
            if(cells[i].item == null)
            {
                cells[i].AddItem(item);
                cells[i].itemsInStack = 1;
                cells[i].UpdateCellUI(item, cells[i].item.isStackable ? true:false);
                break;
            }
            else if(cells[i].item == item && item.isStackable == true && cells[i].itemsInStack < 32)
            {
                cells[i].itemsInStack++;
                cells[i].UpdateCellUI(item, true);
                break;
            }
        }
    }

    public void RemoveItem(int itemIndex)
    {

    }

    public void CreateInventoryList()
    {
        cells = new List<InventoryCell>();

        float yStartPos = transform.position.y;

        float xPos = transform.position.x;
        float yPos = transform.position.y;

        //backgroundTransform = UIController.Instance.UIBackground;
        //RectTransform quickBarBackgroundTransform = GetComponent<RectTransform>();

        for (int x = 0; x < cellColumnAmount; x++)
        {
            for (int y = 0; y < cellRowAmount; y++)
            {
                GameObject cell = Instantiate(cellObject, transform.position, Quaternion.identity, transform);

                cell.transform.localPosition = new Vector3(xPos, yPos, 0);

                RectTransform rectTemp = cell.GetComponent<RectTransform>();
                //rectTemp.sizeDelta = new Vector2(cellWidth, cellHeight);

                cells.Add(cell.GetComponent<InventoryCell>());

                yPos -= cellWidth + cellPadding;
            }
            xPos += cellHeight + cellPadding;
            yPos = yStartPos;            
        }

        //SetTabPositions(playerInventory.transform);

        /*int cellIndex = 0;
        for (int x = 0; x < cellRowAmount; x++)
        {
            for (int y = 0; y < cellColumnAmount; y++)
            {
                Transform cell = cells[cellIndex].transform;
                cell.localPosition = cellUIPos;
                cellUIPos.y -= cellHeight + cellPadding;
                cellIndex++;
            }
            cellUIPos.x += cellWidth + cellPadding;
            cellUIPos.y = (bgHeight / 2) - (cellHeight / 2) - cellPadding;
        }*/
    }

    public void UpdateCellUIFromCellData(int cellIndex)
    {
        cells[cellIndex].UpdateCellUI(cells[cellIndex].item, true);
    }

    /*(public void SaveFunction(object sender, System.EventArgs args)
    {
        print("Player Inventory saved. This one is the quickbar ? " + isQuickBar);
        SavedInventory inventory = new SavedInventory();
        for (int i = 0; i < cells.Count; i++)
        {
            inventory.cellID.Add(i);
            inventory.itemsInStack.Add(cells[i].itemsInStack);
            inventory.item.Add((cells[i].item != null) ? cells[i].item.ID:null);
        }

        SaveLists.Instance.GetInventoryLists().savedInventories.Add(inventory);
    }*/

    public void DisableCells()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].DisableCell();
        }
    }

    public void EnableCells()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].EnableCell();
        }
    }
}
