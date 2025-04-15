using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public GameObject gameMenu;
    [SerializeField] private Transform cameraTransform;
    public InputActionProperty showMenuButton;
    bool showMenu = true;
    float menuDistance = 2.0f;

    private void Start()
    {
        gameMenu.SetActive(showMenu);
    }
    // Update is called once per frame
    void Update()
    {
        if (showMenuButton.action.WasPressedThisFrame())
        {
            ToggleMenuVisibility();
        }

        if (showMenu)
        {
            gameMenu.transform.LookAt(new Vector3(cameraTransform.position.x, gameMenu.transform.position.y, cameraTransform.position.z));
            gameMenu.transform.forward *= -1;
        }
    }

    public void ToggleMenuVisibility()
    {
        showMenu = !showMenu;

        if (showMenu)
            gameMenu.transform.position = cameraTransform.position + new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized * menuDistance;

        gameMenu.SetActive(showMenu);
    }
}
