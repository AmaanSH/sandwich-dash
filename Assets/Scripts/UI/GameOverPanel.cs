using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public TMP_Text completedText;

    public void SetGameOverScreen(int goodOrders, int badOrders)
    {
        completedText.text = $"You completed <b>{goodOrders}</b> orders!";

        gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
