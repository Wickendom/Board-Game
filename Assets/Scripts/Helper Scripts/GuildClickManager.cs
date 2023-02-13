using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildClickManager : MonoBehaviour
{
    [SerializeField]
    private LayerMask leftClickMask;
    [SerializeField]
    private LayerMask rightClickMask;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Click(leftClickMask);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            Click(rightClickMask);
        }
    }

    private void Click(LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray.origin,ray.direction,out hit,layerMask))
        {
            if(hit.collider.GetComponent<IInteractable>() != null)
                hit.collider.GetComponent<IInteractable>().Interact();
        }
    }
}
