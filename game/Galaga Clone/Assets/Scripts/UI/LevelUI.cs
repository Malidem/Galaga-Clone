using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUI : BaseUI
{
    public GameManager gameManager;

    public void StartButton()
    {
        gameManager.gameStarted = true;
        gameManager.StartWaves();
        gameManager.CallUpdateDialogue();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
