using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Explode : NetworkBehaviour
{
    void Start()
    {
        Invoke("CmdDestroyBomb", 0.5f);
    }

    [Command]
    public void CmdDestroyBomb()
    {
        NetworkServer.Destroy(gameObject);
    }
}