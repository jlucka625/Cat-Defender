using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HostManager : NetworkManager {

    public Vector3 player1Postion = new Vector3(0f, 0f, 0f);
    public Vector3 player2Postion = new Vector3(0f, 0f, 0f);

    private GameObject player1Object = null;
    private GameObject player2Object = null;

    public AudioSource BGM;

    public static bool GameOver = false;

    private bool ready = false;

    private static HostManager instance = null;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = null;
        if (player1Object == null)
        {
            player = (GameObject)Instantiate(playerPrefab, player1Postion, Quaternion.identity);
            player1Object = player;
        } else
        {
            player = (GameObject)Instantiate(playerPrefab, player2Postion, Quaternion.identity);
            player2Object = player;
            ready = true;

        }
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        if (ready)
        {
            GameManager.SetPlayerObjs(player1Object, player2Object);
            GameManager.StartGame();
            BGM.Play();
        }
    }
    void Awake()
    {
        instance = this;
    }

    public static void HostGame()
    {
        instance.StartHost();
    }

    public static void JoinGame(string ip)
    {
        instance.networkAddress = ip;
        instance.StartClient();
    }
}
