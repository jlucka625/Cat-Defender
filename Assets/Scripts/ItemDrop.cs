using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
public enum ItemType
{
    NORMAL,
    MEGA,
    BOMB,
}

public class ItemDrop : NetworkBehaviour {
    private Rigidbody rigidbody;

    [SyncVar]
    private ItemType type;

    [SyncVar]
    private int supplyValue;

    [SyncVar]
    private int detonateCount = 0;

    [SyncVar]
    private int maxTaps = 2;

    [SyncVar]
    private bool megaSwipeable = false;


    private Vector3 swipeForce = Vector3.zero;

    public float downwardVelocity = 0.0f;
    public Sprite[] itemSprites;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        GetComponent<SpriteRenderer>().sprite = itemSprites[(int)type];
    }

    public void randomizeItem()
    {
        int random = (int)Random.Range(0.0f, 20.0f);
        if (random <= 14.0f) type = ItemType.NORMAL;
        else if (random <= 18.0f) type = ItemType.BOMB;
        else type = ItemType.MEGA;

        switch (type)
        {
            case (ItemType.NORMAL):
                supplyValue = 100;
                break;
            case (ItemType.MEGA):
                supplyValue = 300;
                break;
            case (ItemType.BOMB):
                supplyValue = -250;
                break;
            default:
                supplyValue = 100;
                break;
        }
    }

    void FixedUpdate()
    {
        Vector3 newVelocity = rigidbody.velocity;
        newVelocity.y = downwardVelocity;
        rigidbody.velocity = newVelocity;

        Quaternion targetRotation = Quaternion.Euler(swipeForce);
        rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, targetRotation, Time.deltaTime);
    }

    public void UpdateSwipeForce(Vector3 force)
    {
        force.z = 30 * Mathf.Sign(force.x);
        force.x = 0;
        swipeForce = force;
    }

    public int getSupplyValue()
    {
        return supplyValue;
    }

    public ItemType getType()
    {
        return type;
    }

    public bool incrementDetonateCount()
    {
        if (detonateCount++ < maxTaps)
        {
            return false;
        }
        return true;
    }
    public void toggleMegaswipe()
    {
        megaSwipeable = !megaSwipeable;
    }

    public bool getMegaSwipeable()
    {
        return megaSwipeable;
    }
}