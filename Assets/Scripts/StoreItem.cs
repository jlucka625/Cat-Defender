using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum StoreItemType
{
    SONG,
    CAT,
    TOWER,
}

public class StoreItem : MonoBehaviour {
    public Ownership defaultOwnership;
    public StoreItemType type;
    public Text priceTag;
    public int price;

    private Ownership ownership;
    private string saveDataName;

    void Awake()
    {
        saveDataName = gameObject.name;
        ownership = defaultOwnership;
    }
    void Start()
    {
        updatePriceTag();
    }

    public void updatePriceTag()
    {
        switch (ownership)
        {
            case Ownership.ON_SALE:
                priceTag.text = "" + price;
                break;
            case Ownership.PURCHASED:
                priceTag.text = "PURCHASED";
                break;
            case Ownership.SELECTED:
                priceTag.text = "PURCHASED";
                break;
            default:
                priceTag.text = "" + price;
                break;
        }
    }

    public Ownership getOwnership()
    {
        return ownership;
    }

    public void setOwnerShip(Ownership newOwnershipStatus)
    {
        ownership = newOwnershipStatus;
    }

    public string getSaveDataName()
    {
        return saveDataName;
    }
}
