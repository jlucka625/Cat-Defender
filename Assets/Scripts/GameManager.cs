using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    private static GameManager instance = null;

    private GameObject player1Object = null;
    private GameObject player2Object = null;

    // Use this for initialization
    void Start () {
        instance = this;
	}

    public static void SetPlayerObjs (GameObject player1Object, GameObject player2Object)
    {
        if (!instance.isServer) return;
        instance.player1Object = player1Object;
        instance.player2Object = player2Object;
        instance.RpcSyncPlayerObjs(instance.player1Object.GetComponent<NetworkIdentity>().netId, instance.player2Object.GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    public void RpcSyncPlayerObjs(NetworkInstanceId p1, NetworkInstanceId p2)
    {
        player1Object = ClientScene.FindLocalObject(p1);
        player2Object = ClientScene.FindLocalObject(p2);
    }

    public static GameObject GetPlayer1()
    {
        if (instance == null) return null;
        return instance.player1Object;
    }

    public static GameObject GetPlayer2()
    {
        if (instance == null) return null;
        return instance.player2Object;
    }

    public static GameObject GetLocalPlayer()
    {
        if (instance == null) return null;
        if (instance.player1Object.GetComponent<NetworkIdentity>().isLocalPlayer)
            return instance.player1Object;
        else
            return instance.player2Object;
    }

    public static void StartGame()
    {
        if (!instance.isServer) return;
        instance.player1Object.GetComponent<Player>().StartPlayer();
        instance.player2Object.GetComponent<Player>().StartPlayer();
    }
}
