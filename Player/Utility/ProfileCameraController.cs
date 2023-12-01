using JhutenFPP.Manager;
using JhutenFPP.PlayerControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileCameraController : MonoBehaviour
{
    private Camera ProfileCamera;
    private InputControl _inputControl;

    private void Awake()
    {
        _inputControl = PlayerController.Instance.GetComponent<InputControl>();

    }
    private void Update()
    {
        if (_inputControl.Inventory) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
