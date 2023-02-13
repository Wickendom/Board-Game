using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlight : MonoBehaviour
{
    [SerializeField]
    private bool highlighted = false;
    private GameObject highlightObject;

    public void Highlight(Color color)
    {
        highlighted = true;

        highlightObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        highlightObject.transform.localPosition = transform.localPosition;
        highlightObject.transform.SetParent(GetComponent<TileData>().GetDecorationsTransform()) ;
        highlightObject.GetComponent<MeshRenderer>().material.color = color;
    }

    public void UnHighlight()
    {
        Destroy(highlightObject);
        highlighted = false;
    }
}
