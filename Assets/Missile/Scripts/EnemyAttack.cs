using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform;
    float maxDistanceToTarget = 12f;
    float distanceToTarget;
    [SerializeField]
    float delayTimer = 2f;
    float tick;
    [SerializeField]
    float moveSpeed = 2f;
    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    private Light pointLight;
    [SerializeField]
    private float blinkDuration = 0.5f; 
    [SerializeField]
    private float blinkInterval = 1f;

    void Start()
    {
        tick = delayTimer;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>(); // Get the Light component on the same object
        }
    }
    void Update()
    {
        {
            distanceToTarget = Vector3.Distance(playerTransform.position, transform.position);

            // Detect if the player is within the camera's view
            if (IsPlayerInCameraView() && distanceToTarget <= maxDistanceToTarget)
            {
                LookAtTarget();
                if (!IsInvoking("ToggleLight"))
                {
                    InvokeRepeating("ToggleLight", 0f, blinkInterval); // Start blinking
                }
            }
            else
            {
                pointLight.enabled = false;
                CancelInvoke("ToggleLight");
            }
            
        }
    }
    void LookAtTarget()
    {
        Vector3 lookVector = playerTransform.position - transform.position;
        lookVector.y = transform.position.y;
        Quaternion rotation = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.Slerp(transform.rotation,
        rotation, 0.01f);

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

    }
    bool IsPlayerInCameraView()
    {
        // Convert the player's world position to the camera's viewport space
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(playerTransform.position);

        // Check if the player is within the camera's viewport (0 to 1 range in both X and Y axes)
        if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0)
        {
            // The player is visible in the camera's view
            return true;
        }
        return false;
    }
    void ToggleLight()
    {
        // Toggle the light's enabled state (on/off)
        pointLight.enabled = !pointLight.enabled;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }

}
