using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DeleteZone : NetworkBehaviour {
    void OnTriggerEnter(Collider item)
    {
        if (!isServer)
            return;

        ItemDrop itemDrop = item.gameObject.GetComponent<ItemDrop>();
        if (itemDrop != null)
        {
            NetworkServer.Destroy(item.gameObject);
        }
    }
}
