using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class NetworkMenu : MonoBehaviour {

    private string ip;
    private GameObject old_obj;
	// Use this for initialization
	void Start () {

        old_obj = EventSystem.current.currentSelectedGameObject;

        ip = "127.0.0.1";
	}

	// Update is called once per frame
	void Update () {

	}

    public void HostGame()
    {
        hideUI();
        HostManager.HostGame();
    }

    public void JoinGame()
    {
        hideUI();
        HostManager.JoinGame(this.ip);
    }

    public void SetIp(string ip)
    {
        this.ip = ip;
    }


    private void hideUI()
    {
        EventSystem.current.SetSelectedGameObject(old_obj);
        this.gameObject.SetActive(false);
    }
}
