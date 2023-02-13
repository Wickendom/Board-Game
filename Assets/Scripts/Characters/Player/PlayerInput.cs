using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Vector3 MovementValues;

    private bool openCharacterSheet = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovementValues = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        openCharacterSheet = Input.GetKeyDown(KeyCode.C);

        if (openCharacterSheet)
            UIController.Instance.ToggleUI();

    }
}
