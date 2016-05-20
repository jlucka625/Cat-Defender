using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreBoard : NetworkBehaviour {

    public Text score1Text;
    public Text score2Text;
    public Text timerText;
    public Text winText;

    public Slider energyBar1;
    public Slider energyBar2;

    [SyncVar]
    private string winString = "";
    [SyncVar]
    private bool gameOver = false;

    [SyncVar]
    private float startTime;
    [SyncVar]
    private int minutes;
    [SyncVar]
    private int seconds;

    [SyncVar]
    private int score1 = 0;
    [SyncVar]
    private int score2 = 0;

    [SyncVar]
    private int energy1 = 0;
    [SyncVar]
    private int energy2 = 0;

    // Use this for initialization
    void Start () {
        startTime = 100;
	}

	// Update is called once per frame
	void Update () {
        if (isServer)
        {
            GameObject player1 = GameManager.GetPlayer1();
            GameObject player2 = GameManager.GetPlayer2();
            if (player1 != null && player2 != null)
            {
                score1 = player1.GetComponent<Player>().GetScore();
                score2 = player2.GetComponent<Player>().GetScore();

                energy1 = player1.GetComponent<Player>().GetEnergy();
                energy2 = player2.GetComponent<Player>().GetEnergy();

                if(startTime > 0)
                {
                    startTime -= Time.deltaTime;
                    winString = "";
                }

                else
                {
                    HostManager.GameOver = true;
                    if (score1 > score2)
                        winString = "Player 1 Wins!";
                    else if (score1 < score2)
                        winString = "Player 2 Wins!";
                    else
                        winString = "Draw!";

                    player1.GetComponent<Player>().endGame(score1 > score2);
                    player2.GetComponent<Player>().endGame(score2 > score1);
                }

                minutes = (int)(startTime / 60.0f);
                seconds = (int)(startTime % 60.0f);
            }
        }

        if(seconds < 10)
            timerText.text = minutes + ":0" + seconds;
        else
            timerText.text = minutes + ":" + seconds;

        score1Text.text = score1.ToString();
        score2Text.text = score2.ToString();

        energyBar1.value = energy1;
        energyBar2.value = energy2;

        winText.text = winString;
    }

    public void regen1ButtonClicked(Button powerupButton)
    {
        bool successful = GameManager.GetLocalPlayer().GetComponent<Abilities>().regenAbility();
        if (successful)
        {
            powerupButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            powerupButton.interactable = false;
        }


    }
    public void shield1ButtonClicked(Button powerupButton)
    {
        bool successful = GameManager.GetLocalPlayer().GetComponent<Abilities>().shieldAbility();
        if (successful)
        {
            powerupButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            powerupButton.interactable = false;
        }
    }
    public void swipe1ButtonClicked(Button powerupButton)
    {
        bool successful = GameManager.GetLocalPlayer().GetComponent<Abilities>().swipeRadiusAbility();
        if (successful)
        {
            powerupButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            powerupButton.interactable = false;
        }
    }
}
