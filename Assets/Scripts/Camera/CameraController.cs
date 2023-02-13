using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField]
    public float moveSpeed;
    
    public Transform playerObject;

    [SerializeField]
    private Vector3 cameraOffset;

    public float cameraRotationPowerX = 2, cameraRotationPowerY = 2;
    // Update is called once per frame

    private float mouseX;

    [SerializeField]
    private float maxYPos, minYPos;

    [SerializeField]
    private float scrollSpeed;

    [SerializeField]
    private bool centerOnPlayerOnStart = false;

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

    public void CenterOnStart(Transform playerTransform)
    {
        if(centerOnPlayerOnStart)
        {
            Vector3 pos = playerTransform.position;
            pos.y = transform.position.y;
            pos.z -= 20;

            transform.position = pos;
        }
    }

    private void Update()
    {
        if (playerObject != null)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                mouseX = Input.GetAxis("Mouse X");
                //mouseY = Input.GetAxis("Mouse Y");

                transform.rotation *= Quaternion.AngleAxis(mouseX * cameraRotationPowerX, Vector3.up);
                //transform.rotation *= Quaternion.AngleAxis(mouseY * cameraRotationPowerY, Vector3.right);

                var angles = transform.localEulerAngles;
                angles.z = 0;

                var angle = transform.localEulerAngles.x;

                if (angle > 180 && angle < 340)
                {
                    angles.x = 340;
                }
                else if (angle < 180 && angle > 40)
                {
                    angles.x = 40;
                }

                transform.localEulerAngles = angles;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if(scroll != 0)
            {
                transform.Translate(new Vector3(0, scroll * scrollSpeed, 0), Space.World);

                if(transform.position.y > maxYPos)
                {
                    transform.position = new Vector3(transform.position.x, maxYPos, transform.position.z);
                }
                else if (transform.position.y < minYPos)
                {
                    transform.position = new Vector3(transform.position.x, minYPos, transform.position.z);
                }
            }
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.Self);

        /*if (playerObject != null)
        {
            transform.position = playerObject.position + cameraOffset;
        
        }*/
    }
}
