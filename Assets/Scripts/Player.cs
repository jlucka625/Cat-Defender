using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {
    private int DefaultEnergy = 0;
    private int maxEnergy = 100;

    [SyncVar]
    private int EnergyRegenratedPerSecond = 1;

    [SyncVar]
    private int energy = 0;

    [SyncVar]
    private int score = 0;

    // Use this for initialization
    void Start()
    {
        if (isServer)
        {
            energy = DefaultEnergy;
        }
    }

    [Command]
    public void CmdAddEnergy(int value)
    {
        energy += value;
    }

    [Command]
    public void CmdAddScore(int value)
    {
        score += value;
        if (score < 0) score = 0;
    }

    [Command]
    public void CmdIncreaseRegenSpeed()
    {
        EnergyRegenratedPerSecond = 2;
    }

    public int GetScore()
    {
        return score;
    }

    public int GetEnergy()
    {
        return energy;
    }

    public void endGame(bool won)
    {
        if(won)
        {
            int playerPurrency = PlayerPrefs.GetInt("Purrency");
            playerPurrency += 100;
            PlayerPrefs.SetInt("Purrency", playerPurrency);
            GetComponent<TouchControl>().getCatManager().playCatSounds(SoundNames.VICTORY);
        }
        else
        {
            GetComponent<TouchControl>().getCatManager().playCatSounds(SoundNames.DEFEAT);
        }
        Invoke("restart", 5.0f);
    }

    public void restart()
    {
        Application.LoadLevel("StartMenu");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartPlayer()
    {
        StartCoroutine(regenerateEnergy());
    }

    IEnumerator regenerateEnergy()
    {
        while (true)
        {
            Debug.Log("OHAI");
            if (energy < maxEnergy)
            {
                energy += EnergyRegenratedPerSecond;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
