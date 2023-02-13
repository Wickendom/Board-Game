using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumbersAnimate : MonoBehaviour
{
    [SerializeField]
    //float moveYSpeed = 1.75f;
    private Vector3 moveVector;

    private const float DISAPPEAR_TIMER_MAX = 1f;

    private float disappearTimer;
    Color textColor;
    TextMeshPro TMP;

    private void Start()
    {

        TMP= GetComponent<TextMeshPro>();
        textColor = TMP.color;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        moveVector = new Vector3(.2f, .35f) * 20f;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            // First half of the popup lifetime
            float increaseScaleAmount = .5f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;

        }
        else
        {
            // second half of the popup lifetime
            float decreaseScaleAmount = .5f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;

        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // start disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;

            TMP.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }

        }
    }
}
