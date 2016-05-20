using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Abilities))]
public class TouchControl : NetworkBehaviour
{
    private Vector3 startSwipePos, endSwipePos;
    private float swipePower = 0.0f;
    private float sphereRadius = 1.0f;
    private float sphereScaleFactor = 10.0f;
    private float powerScaleFactor = 50.0f;
    private int towerIndex;

    public float horizontalSwipeThreshold = 1;
    public float verticalSwipeThreshold = 0.1f;
    public float maxSwipePower = 20.0f;
    public float defaultSphereRadius = 1.0f;
    public float powerupSphereRadius = 2.5f;
    public GameObject trailPrefab;
    private Vector3 towerPosition = new Vector3(-6.7f, -3.24f, 0);

    private GameObject catAvatarInstance = null;
    private GameObject towerInstance = null;
    private GameObject trail;
    private float trailStart = 0.1f, trailEnd = 0.01f;
    public GameObject catAvatarPrefab;
    public GameObject[] towerPrefabs;
    public Sprite P2zoneSprite;

    private Abilities abilites = null;


    [SyncVar]
    private NetworkInstanceId cat_netid;

    void Update()
    {
        if (!HostManager.GameOver)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            //Begin Swipe
            if (Input.GetMouseButtonDown(0))
            {
                //cache initial swipe position
                startSwipePos = Input.mousePosition;
                swipePower = Time.time;

                //start drawing line trail
                trail = Instantiate(trailPrefab) as GameObject;
                trail.GetComponent<TrailRenderer>().startWidth = trailStart;
                trail.GetComponent<TrailRenderer>().endWidth = trailEnd;
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(startSwipePos);
                newPosition.z = 0;
                trail.transform.position = newPosition;
            }
            //during swipe
            if (Input.GetMouseButton(0))
            {
                //update line trail
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newPosition.z = 0;
                trail.transform.position = newPosition;
            }

            //end swipe
            if (Input.GetMouseButtonUp(0))
            {

                endSwipePos = Input.mousePosition;
                float xDist = Mathf.Abs(endSwipePos.x - startSwipePos.x);
                float yDist = Mathf.Abs(endSwipePos.y - startSwipePos.y);

                //Player didn't actually swipe horizontally, just tapped or swiped vertically
                if (xDist < horizontalSwipeThreshold || yDist > verticalSwipeThreshold)
                {
                    detonate();
                    markForMegaSwipe();
                    return;
                }

                //Send message over network to update cat animations
                CmdSetCatAnimation(catAvatarInstance.GetComponent<NetworkIdentity>().netId, "Swipe");

                //Destroy line trail after number of seconds
                Destroy(trail, trail.GetComponent<TrailRenderer>().time);

                //Determine direction to move item drops based on swipe direction
                Vector3 direction = (int)Mathf.Sign(endSwipePos.x - startSwipePos.x) * Vector3.right;

                Ray ray = Camera.main.ScreenPointToRay(startSwipePos);
                RaycastHit hit;

                startSwipePos = Camera.main.ScreenToWorldPoint(startSwipePos);
                endSwipePos = Camera.main.ScreenToWorldPoint(endSwipePos);
                startSwipePos.z = 0;
                endSwipePos.z = 0;

                float distance = Mathf.Abs(startSwipePos.x - endSwipePos.x);
                swipePower = (Mathf.Abs(endSwipePos.x - startSwipePos.x) - (Time.time - swipePower)) * powerScaleFactor;
                swipePower = Mathf.Clamp(swipePower, 0.0f, maxSwipePower);

                //checks if player tapped directly on an item drop
                if (Physics.Raycast(ray, out hit, 100))
                {
                    ItemDrop item = hit.collider.gameObject.GetComponent<ItemDrop>();
                    if (item != null)
                    {
                        //if the item was marked to be mega swiped already, then mega swipe it across the screen
                        if (item.getMegaSwipeable())
                        {
                            Rigidbody dropItemRigidbody = hit.collider.gameObject.GetComponent<Rigidbody>();
                            Vector3 newVelocity = dropItemRigidbody.velocity;
                            newVelocity.x = 0;
                            abilites.CmdMegaSwipe(item.GetComponent<NetworkIdentity>().netId, newVelocity, direction * maxSwipePower);
                        }
                    }
                }
                else
                {
                    //checks if player swiped near an item drop, and moves the item accordingly
                    RaycastHit[] hits = Physics.SphereCastAll(startSwipePos, sphereRadius, direction, distance);
                    Debug.DrawLine(startSwipePos, endSwipePos, Color.red);
                    foreach (RaycastHit h in hits)
                    {
                        ItemDrop item = h.collider.gameObject.GetComponent<ItemDrop>();
                        if (item != null)
                        {
                            Rigidbody dropItemRigidbody = h.collider.gameObject.GetComponent<Rigidbody>();
                            Vector3 newVelocity = dropItemRigidbody.velocity;
                            newVelocity.x = 0;
                            abilites.CmdSwipe(item.GetComponent<NetworkIdentity>().netId, newVelocity, direction * swipePower);
                        }
                    }
                }
            }
        }
    }

    /*checks if the player tapped directly on an air drop, if it did, then mark it as eligable to be
    be mega swiped*/
    public void markForMegaSwipe()
    {
        if (GetComponent<Player>().GetEnergy() - GetComponent<Abilities>().MegaswipCost >= 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(startSwipePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<ItemDrop>() != null)
                {
                    if(!hit.collider.gameObject.GetComponent<ItemDrop>().getMegaSwipeable())
                    {
                        hit.collider.gameObject.GetComponent<ItemDrop>().toggleMegaswipe();
                    }
                }
            }
       }
    }

    /*Checks if player tapped directly on a bomb, if so, add to the bomb counter until
    it is tapped enough times to detonate*/
    public void detonate()
    {
        if(GetComponent<Player>().GetEnergy() - GetComponent<Abilities>().DetonateCost >= 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(startSwipePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<ItemDrop>() != null)
                {
                    if (hit.collider.gameObject.GetComponent<ItemDrop>().incrementDetonateCount())
                    {
                        GetComponent<Abilities>().CmdDetonate(hit.collider.gameObject.GetComponent<NetworkIdentity>().netId);
                    }
                }
            }
        }
    }

    [Command]
    private void CmdSetCatAnimation(NetworkInstanceId id, string name)
    {
        NetworkServer.FindLocalObject(id).GetComponent<CatLoader>().RpcSetCatAnimation(name);
    }

    public override void OnStartServer()
    {
        catAvatarInstance = Instantiate(catAvatarPrefab, transform.position, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(catAvatarInstance);
        cat_netid = catAvatarInstance.GetComponent<NetworkIdentity>().netId;
    }

    public override void OnStartClient()
    {
        sphereRadius = defaultSphereRadius;
        catAvatarInstance = ClientScene.FindLocalObject(cat_netid);
    }

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            towerIndex = PlayerPrefs.GetInt("TowerType") - 1;
            int index = (int)PlayerPrefs.GetInt("CatType") - 1;
            CmdSpawnTower(towerIndex);
            CmdSyncCatsSprite(index);
            CmdSyncFoodBowls();
            abilites = GetComponent<Abilities>();
        }
    }
    [Command]
    private void CmdSyncFoodBowls()
    {
        RpcChangeBowlSprite(GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    private void RpcChangeBowlSprite(NetworkInstanceId id)
    {
        GameObject playerInstance = ClientScene.FindLocalObject(id);
        if (gameObject == GameManager.GetPlayer2())
            playerInstance.GetComponent<SpriteRenderer>().sprite = P2zoneSprite;
    }

    [Command]
    private void CmdSpawnTower(int towerIndex)
    {
        towerInstance = Instantiate(towerPrefabs[towerIndex], towerPosition, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(towerInstance);
        RpcSyncCatAndTower(towerInstance.GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    private void CmdSyncCatsSprite(int index)
    {
        catAvatarInstance.GetComponent<CatLoader>().RpcSetCatSprites(index);
        RpcSyncCatavatar();
    }

    [ClientRpc]
    private void RpcSyncCatavatar()
    {
        if (gameObject == GameManager.GetPlayer2())
        {
            Vector3 localScale = catAvatarInstance.transform.localScale;
            localScale.x *= -1;
            catAvatarInstance.transform.localScale = localScale;
        }
    }

    [ClientRpc]
    private void RpcSyncCatAndTower(NetworkInstanceId tower_netid)
    {
        towerInstance = ClientScene.FindLocalObject(tower_netid);

        int t_index = (int)PlayerPrefs.GetInt("TowerType") - 1;
        if (t_index == 2)
            towerPosition.y += 1f;
        towerInstance.transform.position = towerPosition;

        if (gameObject == GameManager.GetPlayer2())
        {
            Vector3 localScale = towerInstance.transform.localScale;
            localScale.x *= -1;
            towerInstance.transform.localScale = localScale;

            towerPosition.x *= -1;
            towerInstance.transform.position = towerPosition;
        }
        GameObject catInstance = ClientScene.FindLocalObject(cat_netid);
        Vector3 newPos = towerInstance.GetComponent<Collider>().bounds.center;
        newPos.x = towerInstance.GetComponent<Collider>().bounds.center.x;

        //adjusted exact height because sprite bounds are off
        int index = (int)PlayerPrefs.GetInt("CatType") - 1;
        switch(index)
        {
            case 0: newPos.y -= 0.1f; break;
            case 1: newPos.y -= 0.3f; break;
            case 2: newPos.y -= 0.1f; break;
            case 3: newPos.y -= 0.5f; break;
        }
        newPos.y += towerInstance.GetComponent<Collider>().bounds.extents.y + catInstance.GetComponent<SpriteRenderer>().bounds.extents.y;
        catInstance.transform.position = newPos;
    }
    public CatLoader getCatManager()
    {
        return catAvatarInstance.GetComponent<CatLoader>();
    }

    public void UpdateSphereRadius()
    {
        sphereRadius = powerupSphereRadius;
        trailStart = 0.5f;
        trailEnd = 0.05f;
    }
}
