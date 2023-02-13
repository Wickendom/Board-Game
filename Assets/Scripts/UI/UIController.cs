using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private ActionUI actionUI;

    public Camera UICamera;

    [SerializeField]
    private GameObject damageNumberPrefab;

    private bool uiEnabled = false;

    [SerializeField]
    private GameObject characterSheetUIGO;

    public PlayerInventory playerInventory;

    public GameObject playerInventoryUIGO;

    public bool inventoryUIOpen;

    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }

        graphicRaycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        actionUI = GetComponentInChildren<ActionUI>();
    }

    public void Start()
    {
        DisableInGameUI();
    }

    public void Initialise()
    {
        actionUI.Initialise();
    }

    public void SetLocalPlayersTurn()
    {
        actionUI.localPlayersTurn = true;
    }

    public void UnsetLocalPlayersTurn()
    {
        actionUI.localPlayersTurn = false;
    }

    public bool CheckIfHoveringOverUI()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        graphicRaycaster.Raycast(pointerData, results);

        if(results.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }    
    }

    public void CreateDamageNumbers(Character unit, int amount)
    {
        Debug.Log("Creating Damage Numbers");
        new DamagePopUp(unit, amount, damageNumberPrefab);
    }

    private void DisableInGameUI()
    {
        characterSheetUIGO.SetActive(false);
        ClosePlayerInventoryUI();
        uiEnabled = false;
    }

    private void EnableInGameUI()
    {
        characterSheetUIGO.SetActive(true);
        OpenPlayerInventoryUI();
        uiEnabled = true;
    }

    public void ToggleUI()
    {
        if(uiEnabled)
        {
            DisableInGameUI();
        }
        else
        {
            EnableInGameUI();
        }
    }

    private void Update()
    {
        /*if (tabMenuOpening)
        {
            float tabMenuSpeed = 500 * Time.deltaTime;

            tabMenuUIGO.transform.position = Vector3.MoveTowards(tabMenuUIGO.transform.position, tabMenuOpenPosition.position, tabMenuSpeed);

            if (Vector3.Distance(tabMenuUIGO.transform.position, tabMenuOpenPosition.position) < 0.001f)
            {
                tabMenuOpening = false;
            }
        }

        if (tabMenuClosing)
        {
            float tabMenuSpeed = 500 * Time.deltaTime;

            tabMenuUIGO.transform.position = Vector3.MoveTowards(tabMenuUIGO.transform.position, tabMenuClosedPosition.position, tabMenuSpeed);

            if (Vector3.Distance(tabMenuUIGO.transform.position, tabMenuClosedPosition.position) < 0.001f)
            {
                tabMenuClosing = false;
            }
        }*/
    }

    /*public void UpdatePlayerHealthBarValue(int curHealth, int maxHealth)
    {
        playerHealthBar.value = GetPlayerHealthBarValue(curHealth, maxHealth);
    }

    private float GetPlayerHealthBarValue(int curHealth, int maxHealth)
    {
        float temp = maxHealth * 0.01f;
        return (curHealth / temp) * 0.01f;
    }*/

    /*public void UpdateCraftingUI(BuildingData buildingData)
    {
        craftingUIGO.GetComponent<CraftingUI>().UpdateBuildingList(buildingData);
    }

    public void UpdateCraftingUI(ItemData itemData)
    {
        craftingUIGO.GetComponent<CraftingUI>().UpdateItemList(itemData);
    }*/

    /*private void SetTabPositions(Transform parent)
    {
        inventoryTab.sizeDelta = new Vector2(bgWidth / 3, inventoryTab.sizeDelta.y);
        inventoryTab.transform.SetParent(parent);
        inventoryTab.transform.localPosition = new Vector3(((-bgWidth / 2) + (inventoryTab.sizeDelta.x / 2)) + 5, ((bgHeight / 2) + (inventoryTab.sizeDelta.y / 2)) + 5, 0);
        inventoryTabCollider.size = inventoryTab.sizeDelta;

        /*craftingTab.sizeDelta = new Vector2(bgWidth / 3, craftingTab.sizeDelta.y);
        craftingTab.transform.SetParent(parent);
        craftingTab.transform.localPosition = new Vector3(((-bgWidth / 2) + (craftingTab.sizeDelta.x / 2)) + (craftingTab.sizeDelta.x + 10), ((bgHeight / 2) + (craftingTab.sizeDelta.y / 2)) + 5, 0);
        craftingTabCollider.size = craftingTab.sizeDelta;*/
    //}

    #region OPEN/CLOSE UI's

    public void CloseAllUI()
    {
        playerInventory.DisableCells();
    }
    public void CloseOtherUI()
    {
        playerInventory.DisableCells();
    }

    public void OpenPlayerInventoryUI()
    {
        //CloseOtherUI();
        inventoryUIOpen = true;
        //OpenEquipmentUI();
        playerInventoryUIGO.SetActive(true);
    }

    public void ClosePlayerInventoryUI()
    {
        //playerInventory.DisableCells();
        playerInventoryUIGO.SetActive(false);
        inventoryUIOpen = false;
        //CloseEquipmentGUI();
    }

    #endregion

    /*public void ToggleCraftBuildingUI()
    {
        if (buildingUIOpen)
        {
            buildingUIOpen = false;
            CloseBuildingUI();
        }
        else
        {
            buildingUIOpen = true;
            OpenBuildingUI();
        }
    }*/

    public void ToggleInventoryUI()
    {
        if (inventoryUIOpen)
        {
            inventoryUIOpen = false;
            ClosePlayerInventoryUI();
        }
        else
        {
            inventoryUIOpen = true;
            OpenPlayerInventoryUI();
        }
    }

    /*public void ToggleProcessingUI()
    {
        if (processingUIOpen)
        {
            processingUIOpen = false;
            CloseProcessingUI();
        }
        else
        {
            processingUIOpen = true;
            OpenProcesssingUI();
        }
    }

    public void ToggleAdvancedCraftingUI()
    {
        if (advancedCraftingUIOpen)
        {
            advancedCraftingUIOpen = false;
            CloseAdvancedCraftingGUI();
        }
        else
        {
            advancedCraftingUIOpen = true;
            OpenAdvancedCraftingUI();
        }
    }

    public void ToggleCraftingUI()
    {
        if (craftingUIOpen)
        {
            craftingUIOpen = false;
            CloseCraftingUI();
        }
        else
        {
            craftingUIOpen = true;
            OpenCraftingUI();
        }
    }

    public void ToggleEquipmentUI()
    {
        if (equipmentUIOpen)
        {
            equipmentUIOpen = false;
            CloseEquipmentGUI();
        }
        else
        {
            equipmentUIOpen = true;
            OpenEquipmentUI();
        }
    }*/
    
    /*public void ToggleTabUI()
    {
        if (tabMenuUIOpen)
        {
            tabMenuOpening = false;
            tabMenuUIOpen = false;
            CloseTabMenuGUI();
        }
        else
        {
            tabMenuClosing = false;
            tabMenuUIOpen = true;
            OpenTabUI();
        }
    }*/
}
