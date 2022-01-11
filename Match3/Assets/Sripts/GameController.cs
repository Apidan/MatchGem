using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class GameController : MonoBehaviour {

    public static GameController Instance;
    public Text GameOverText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameOverText.text = "";
    }

    public void GameOver()
    {
        GameOverText.text = "Game Over";
       
    }
}
