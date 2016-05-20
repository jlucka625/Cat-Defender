using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Player))]
public class Abilities : NetworkBehaviour
{
    public int DetonateCost = 75;
    public int MegaswipCost = 25;
    public int regenCost = 75;
    public int shieldCost = 150;
    public int radiusCost = 200;
    public GameObject shieldPrefab;
    public GameObject explosionPrefab;
    private Player player = null;

    private bool purchasedRegen = false;
    private bool purchasedShield = false;
    private bool purchasedSwipe = false;

    /*Deducts from player's energy and then destroys the falling bomb object
    the animation and energy drop is synced to both clients over the network*/
    [Command]
    public void CmdDetonate(NetworkInstanceId id)
    {
        ItemDrop item = NetworkServer.FindLocalObject(id).GetComponent<ItemDrop>();
        player.CmdAddEnergy(-1 * DetonateCost);
        NetworkServer.Destroy(item.gameObject);

        // Put animaion here by calling something like RpcSetExploadAnimation 
        if (item.getType() == ItemType.BOMB)
        {
            GameObject explosion = Instantiate(explosionPrefab, item.transform.position, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(explosion);
        }
    }

    /*Deducts from player's energy and then moves the item at a larger than normal force*/
    [Command]
    public void CmdMegaSwipe(NetworkInstanceId id, Vector3 velocity, Vector3 force)
    {
        player.CmdAddEnergy(-1 * MegaswipCost);
        moveItem(NetworkServer.FindLocalObject(id).GetComponent<ItemDrop>(), velocity, force * 2);
    }

    [Command]
    public void CmdSwipe(NetworkInstanceId id, Vector3 velocity, Vector3 force)
    {
        moveItem(NetworkServer.FindLocalObject(id).GetComponent<ItemDrop>(), velocity, force);
    }

    //Adds a force to the item that has been swiped upon
    private void moveItem(ItemDrop item, Vector3 velocity, Vector3 force)
    {
        Rigidbody dropItemRigidbody = item.GetComponent<Rigidbody>();
        dropItemRigidbody.velocity = velocity;
        dropItemRigidbody.AddForce(force);
        item.UpdateSwipeForce(force);
    }

    //The player has used points from their score to purchse the regen ability
    public bool regenAbility()
    {
        if (!isLocalPlayer || purchasedRegen) return false;

        int supplies = player.GetScore();
        int energy = player.GetEnergy();

        if(supplies - regenCost >= 0)
        {
            player.CmdAddScore(-regenCost);
            player.CmdIncreaseRegenSpeed();
            purchasedRegen = true;
            return true;
        }
        return false;
    }

    //The player has used points from their score to purchase the shield ability
    public bool shieldAbility()
    {
        if (!isLocalPlayer || purchasedShield) return false;

        int supplies = player.GetScore();
        int energy = player.GetEnergy();

        if (supplies - shieldCost >= 0)
        {
            player.CmdAddScore(-shieldCost);
            CmdSpawnShield();
            purchasedShield = true;
            return true;
        }
        return false;
    }

    //The player has used points from their score to increase the radius of influence of their swipes
    public bool swipeRadiusAbility()
    {
        if (!isLocalPlayer || purchasedSwipe) return false;

        int supplies = player.GetScore();
        int energy = player.GetEnergy();

        if (supplies - radiusCost >= 0)
        {
            player.CmdAddScore(-radiusCost);
            GetComponent<TouchControl>().UpdateSphereRadius();
            purchasedSwipe = true;
            return true;
        }
        return false;
    }

    [Command]
    public void CmdSpawnShield()
    {
        GameObject shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(shieldInstance);
    }

    public override void OnStartLocalPlayer()
    {
        player = GetComponent<Player>();
        CmdSyncPlayer(player.GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    void CmdSyncPlayer(NetworkInstanceId id)
    {
        RpcSyncPlayer(id);
    }

    [ClientRpc]
    void RpcSyncPlayer(NetworkInstanceId id)
    {
        player = ClientScene.FindLocalObject(id).GetComponent<Player>();
    }
}
