using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public GameObject gameMenuParent;
    [SerializeField] GameObject gameMenuPage;
    [SerializeField] GameObject settingsPage;
    [SerializeField] GameObject leaderboardPage;
    [SerializeField] GameObject nameTagPage;
    [SerializeField] GameManager gameManager;
    [SerializeField] private Transform cameraTransform;
    public InputActionProperty showMenuButton;
    bool showMenu = true;
    float distance = 2.0f;
    Vector2 menuSize;

    private void Start()
    {
        gameMenuParent.SetActive(showMenu);
        RectTransform gameMenuTransform = gameMenuParent.GetComponent<RectTransform>();
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

            gameMenuParent.transform.position = spawnPosition;
            gameMenuParent.transform.LookAt(new Vector3(cameraTransform.position.x, gameMenuParent.transform.position.y, cameraTransform.position.z));
            gameMenuParent.transform.forward *= -1;
        }
        else
        {
            gameMenuPage.SetActive(true);
            settingsPage.SetActive(false);
            leaderboardPage.SetActive(false);
            nameTagPage.SetActive(false);
        }

        showMenu = !showMenu;
        gameMenuParent.SetActive(showMenu);
    }

    public void ShowEndClimbPage(string time)
    {
        ToggleMenuVisibility();
        gameMenuPage.SetActive(false);
        nameTagPage.SetActive(true);
        nameTagPage.GetComponent<NameTagPage>().SetEndTime(time);
    }
}
