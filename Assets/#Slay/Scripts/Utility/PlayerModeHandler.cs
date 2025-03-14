using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModeHandler : MonoBehaviour
{
    public Camera mainCamera;

    public BoolReference playerIsSinger;
    public Transform singerCameraTransform;
    public Transform defenderCameraTransform;


    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        UpdateCameraPosition();
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.S))
        if(InputHandler.Instance.SwitchCameraTrigger)
        {
            playerIsSinger.Value = !playerIsSinger.Value;
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        mainCamera.transform.SetParent(playerIsSinger.Value ? singerCameraTransform : defenderCameraTransform);
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.localRotation = Quaternion.identity;

        mainCamera.GetComponent<ShakeBehaviour>().SetNewOriginalPosition();
    }
}
