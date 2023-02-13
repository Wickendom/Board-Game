using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUp 
{
    private TextMeshPro TextMesh;
    private Color textColor;

    [SerializeField]
    GameObject DamageNumberPopUp;

    public DamagePopUp(Character unit, int damageAmount, GameObject damageNumberPrefab)
    {
        Vector3 spawnPos = new Vector3(unit.transform.position.x, 2.6f, unit.transform.position.z);
        GameObject damagePopupGO = MonoBehaviour.Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity);
        damagePopupGO.transform.SetParent(UIController.Instance.gameObject.transform);
        TextMesh = damagePopupGO.GetComponent<TextMeshPro>();
        Setup(damageAmount);
    }

    public void Setup(int damageAmount)
    {
        TextMesh.SetText(damageAmount.ToString());
        textColor = TextMesh.color;
    }
}