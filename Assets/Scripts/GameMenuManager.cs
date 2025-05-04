using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public GameObject gameMenu;
    [SerializeField] GameManager gameManager;
    [SerializeField] private Transform cameraTransform;
    public InputActionProperty showMenuButton;
    bool showMenu = true;
    float distance = 1.0f;
    Vector2 menuSize;

    private void Start()
    {
        gameMenu.SetActive(showMenu);
        RectTransform gameMenuTransform = gameMenu.GetComponent<RectTransform>();
        menuSize = gameMenuTransform.rect.size;
        menuSize.x *= gameMenuTransform.lossyScale.x;
        menuSize.y *= gameMenuTransform.lossyScale.y;
    }
    // Update is called once per frame
    void Update()
    {

        if (showMenuButton.action.WasPressedThisFrame() && gameManager.grounded)
        {
           ToggleMenuVisibility();
        }
    }

    public void ToggleMenuVisibility()
    { 
        if (!showMenu)
        {
            // Will show menu
            // Check if the menu would collide with scene items
            Vector3 spawnPosition = cameraTransform.position + new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized * distance;
            Vector3 lookDirection = new Vector3(cameraTransform.position.x, 0, cameraTransform.position.z) - new Vector3(spawnPosition.x, 0, spawnPosition.z).normalized;
            Collider[] colliders = Physics.OverlapBox(spawnPosition, new Vector3(menuSize.x / 2, menuSize.y / 2, 0.1f), Quaternion.LookRotation(lookDirection));

            if (colliders.Length > 0)
            {
                return;
            }

            gameMenu.transform.position = spawnPosition;
            gameMenu.transform.LookAt(new Vector3(cameraTransform.position.x, gameMenu.transform.position.y, cameraTransform.position.z));
            gameMenu.transform.forward *= -1;
        }

        showMenu = !showMenu;
        gameMenu.SetActive(showMenu);
    }
}
