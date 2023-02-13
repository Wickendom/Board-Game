using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitiativeTrackerUI : MonoBehaviour
{
    public static InitiativeTrackerUI Instance;

    public GameObject baseCharacterUI;

    [SerializeField]
    List<InitiativeTrackerUIData> characterUIs;

    private List<Vector3> uiPositions;

    public float uiMoveSpeed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        characterUIs = new List<InitiativeTrackerUIData>();
        uiPositions = new List<Vector3>();
    }

    public void CreateUIs(List<Character> characters)
    {
        Debug.Log("Initiative tracker UIs creating...");

        float xSpawnPos = transform.position.x;

        for (int i = 0; i < characters.Count; i++)
        {
            bool addToList = true;

            for (int j = 0; j < characterUIs.Count; j++)
            {
                if (characters[i] == characterUIs[j]._char)
                {
                    addToList = false;
                    break;
                }
            }

            if (addToList)
            {
                Debug.Log("Adding unit to initiative tracker ui list");
                Transform uiTransform = Instantiate(baseCharacterUI, transform).transform;

                InitiativeTrackerUIData data = new InitiativeTrackerUIData(uiTransform);

                data.initialIndex = i;
                data.currentIndex = i;

                data.UpdateIndexDebugUI(i);

                data._char = characters[i];

                if (data._char.charData.initiativeTrackerSprite != null)
                {
                    uiTransform.GetComponent<Image>().sprite = data._char.charData.initiativeTrackerSprite;
                }

                characterUIs.Add(data);
            }
        }

        OrderUIs();
        SetUIPosiitons();
    }

    private void OrderUIs()
    {
        Vector2 initialPos = transform.position;
        RectTransform rectTrans = characterUIs[0].transform as RectTransform;
        float width = rectTrans.rect.width;

        initialPos.x -= width * 0.5f;
        initialPos.x -= width * (characterUIs.Count * 0.5f);

        uiPositions.Add(initialPos);

        Vector2 uiPos = initialPos;

        for (int i = 1; i < characterUIs.Count; i++)
        {
            uiPos.x += width;
            uiPositions.Add(uiPos);
        }
    }

    private void MoveUIToIndexPositions()
    {
        for (int i = 0; i < characterUIs.Count; i++)
        {
            MoveUIToSlot(characterUIs[i], characterUIs[i].currentIndex, false);
        }
    }

    private void SetUIPosiitons()
    {
        for (int i = 0; i < characterUIs.Count; i++)
        {
            RectTransform rectTransform = characterUIs[i].transform as RectTransform;
            rectTransform.localPosition = uiPositions[characterUIs[i].currentIndex];
        }
    }

    public void MoveToEnd(Character character)
    {
        InitiativeTrackerUIData uiData = null;
        for (int i = 0; i < characterUIs.Count; i++)
        {
            if(character == characterUIs[i]._char)
            {
                uiData = characterUIs[i];
                break;
            }
        }

        StartCoroutine(LowerImage(uiData, characterUIs.Count - 1));
        MoveAllButOneLeft1Slot(uiData);
    }

    private void MoveAllButOneLeft1Slot(InitiativeTrackerUIData data)//this is mostly to be called by the movetoend func to move all but the unit that ended their turn
    {
        List<InitiativeTrackerUIData> tempArray = characterUIs;

        //tempArray.Remove(data);

        for (int i = 0; i < tempArray.Count; i++)
        {
            if(tempArray[i].currentIndex >= 1)
                StartCoroutine(MoveUIToSlot(tempArray[i], tempArray[i].currentIndex - 1,false));
        }
    }

    public void RemoveFromTracker(Character character)
    {
        for (int i = 0; i < characterUIs.Count; i++)
        {
            if(characterUIs[i]._char.characterID == character.characterID)
            {
                Destroy(characterUIs[i].transform.gameObject);
                characterUIs.RemoveAt(i);
                break;
            }
        }

        OrderUIs();
        MoveUIToIndexPositions();
    }

    private IEnumerator LowerImage(InitiativeTrackerUIData data, int moveToSlot)//this moves the image down so it looks like it can move around the other images
    {
        RectTransform rect = data.transform as RectTransform;
        if(rect != null)
        {
            Vector3 pos = rect.localPosition;
            pos.y -= rect.rect.height;
            while (Vector3.Distance(rect.localPosition, pos) > 0.01)
            {
                rect.localPosition = Vector3.MoveTowards(rect.localPosition, pos, uiMoveSpeed * Time.deltaTime);
                yield return null;
            }

            Debug.Log("Start Moving Image");
            yield return StartCoroutine(MoveUIToSlot(data, moveToSlot, true));
            
            
        }
        
    }

    private IEnumerator RaiseImage(InitiativeTrackerUIData data)
    {
        Debug.Log("Raising Image");
        RectTransform rect = data.transform as RectTransform;

        if(rect != null)
        {
            while (Vector3.Distance(rect.localPosition, uiPositions[data.currentIndex]) > 0.01)
            {
                rect.localPosition = Vector3.MoveTowards(rect.localPosition, uiPositions[data.currentIndex], uiMoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    private IEnumerator MoveUIToSlot(InitiativeTrackerUIData data, int moveToSlot, bool raiseImage)
    {
        //Vector3 pos = new Vector3(characterUIs[0].transform.position.x + (rect.sizeDelta.x * moveToSlot), rect.position.y, rect.position.z);
        RectTransform rect = data.transform as RectTransform;
        if(rect != null)
        {
            data.currentIndex = moveToSlot;
            data.UpdateIndexDebugUI(moveToSlot);

            //Debug.Log(moveToSlot);

            Vector2 moveToPos = rect.localPosition;
            moveToPos.x = uiPositions[moveToSlot].x;

            while (Vector3.Distance(rect.localPosition, moveToPos) > 0.01)
            {
                //Debug.Log("Moving");
                rect.localPosition = Vector3.MoveTowards(rect.localPosition, moveToPos, uiMoveSpeed * Time.deltaTime);
                yield return null;
            }

            rect.localPosition = moveToPos;

            Debug.Log(raiseImage);
            if (raiseImage)
                yield return StartCoroutine(RaiseImage(data));
        }
        
    }

    public void TurnOffInitiativeTracker()
    {
        for (int i = 0; i < characterUIs.Count; i++)
        {
            Destroy(characterUIs[i].transform.gameObject);
        }

        characterUIs.Clear();
    }
}

class InitiativeTrackerUIData
{
    public Transform transform;
    public int initialIndex;
    public int currentIndex;

    public Character _char;

    private UnityEngine.UI.Text indexUI;

    public InitiativeTrackerUIData(Transform uiTransform)
    {
        transform = uiTransform;
        indexUI = transform.GetComponentInChildren<UnityEngine.UI.Text>();
    }

    public void UpdateIndexDebugUI(int value)
    {
        indexUI.text = value.ToString();
    }
}