using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreZone : NetworkBehaviour {

    void OnTriggerEnter(Collider item)
    {
        if (!isServer)
            return;

        GameObject player1 = GameManager.GetPlayer1();
        GameObject player2 = GameManager.GetPlayer2();
        ItemDrop itemDrop = item.gameObject.GetComponent<ItemDrop>();
        if (itemDrop != null)
        {
            switch(itemDrop.getType())
            {
                case ItemType.BOMB:
                    GetComponent<TouchControl>().getCatManager().playCatSounds(SoundNames.BOMB);
                    break;
                case ItemType.MEGA:
                    GetComponent<TouchControl>().getCatManager().playCatSounds(SoundNames.CATNIP_CATCH);
                    break;
                case ItemType.NORMAL:
                    GetComponent<TouchControl>().getCatManager().playCatSounds(SoundNames.TUNA_CATCH);
                    break;
            }
            if(player1 != null && player2 != null)
                GetComponent<Player>().CmdAddScore(itemDrop.getSupplyValue());

            NetworkServer.Destroy(item.gameObject);
        }
    }
}
