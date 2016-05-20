using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Shield : NetworkBehaviour {

    public int RegenerateShieldSeconds = 3;

    void OnTriggerEnter(Collider hit)
    {
        if (!isServer)
            return;

        ItemDrop item = hit.gameObject.GetComponent<ItemDrop>();
        if(item != null && item.getType() == ItemType.BOMB)
        {
            NetworkServer.Destroy(item.gameObject);
            RpcSetActive(false);
            Invoke("regenerateShield", RegenerateShieldSeconds);
            Debug.Log("Test");
        }
    }

    [ClientRpc]
    void RpcSetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    void regenerateShield()
    {
        Debug.Log("regenerateShield");
        RpcSetActive(true);
    }
}
