using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour {
    public RectTransform storeUI;
    public Text purrencyLabel;
    public Sprite[] catSprites;
    public Sprite[] towerSprites;
    public Image currentCatButton, currentTowerButton;
    public Text currentSongButton;

    private Color32 purchasedColor = new Color32(14, 192, 113, 255);
    private Dictionary<string, int> items;
    private string[] catNames = { "TabbyCatButton", "HalloweenCatButton", "CowboyCatButton", "FatCat" };
    private string[] towerNames = {"BaseTowerButton", "MidgradeTowerButton", "DeluxeTowerButton", "SuperTowerButton"};
    private string[] songNames = { "Song1Button", "Song2Button" };
    public string[] songLabels = { "Nyan Cat", "Meow Mix" };
    private int towerIndex = 0, songIndex = 0, catIndex = 0;

    void Start()
    {
        items = new Dictionary<string, int>();
        updatePurrency();
        populateInventory();
    }

    public void updatePurrency()
    {
        purrencyLabel.text = "  PURRENCY: " + PlayerPrefs.GetInt("Purrency");
    }

    public void playButtonClicked()
    {
        Application.LoadLevel("demo");
    }

    public void storeButtonClicked()
    {
        storeUI.gameObject.GetComponent<Animator>().SetTrigger("SlideIn");
    }

    public void exitStoreButtonClicked()
    {
        storeUI.gameObject.GetComponent<Animator>().SetTrigger("SlideOut");
    }

    /*=================================================================================================
    ========== INITIALIZING UI ========================================================================
    =================================================================================================*/
    public void populateInventory()
    {
        items.Add("Song1Button", (int)SongType.NYAN);
        items.Add("Song2Button", (int)SongType.MEOW);
        items.Add("TabbyCatButton", (int)CatType.TABBY);
        items.Add("HalloweenCatButton", (int)CatType.HALLOWEEN);
        items.Add("CowboyCatButton", (int)CatType.COWBOY);
        items.Add("FatCat", (int)CatType.FAT);
        items.Add("BaseTowerButton", (int)TowerType.BASE);
        items.Add("MidgradeTowerButton", (int)TowerType.MIDGRADE);
        items.Add("DeluxeTowerButton", (int)TowerType.DELUXE);
        items.Add("SuperTowerButton", (int)TowerType.SUPER);

        Button[] buttons = storeUI.GetComponentsInChildren<Button>();
        foreach(Button button in buttons)
        {
            StoreItem item = button.GetComponent<StoreItem>();
            if(item != null)
            {
                Ownership prefs = (Ownership)PlayerPrefs.GetInt(item.getSaveDataName());
                if (prefs != Ownership.INVALID)
                { 
                    item.setOwnerShip(prefs);
                    item.updatePriceTag();
                }
                else
                {
                    PlayerPrefs.SetInt(item.getSaveDataName(), (int)item.getOwnership());
                }

                if (item.getOwnership() != Ownership.INVALID)
                {
                    if (item.getOwnership() != Ownership.ON_SALE)
                        button.GetComponent<Image>().color = purchasedColor;
                }

                if(item.getOwnership() == Ownership.SELECTED)
                {
                    switch (item.type)
                    {
                        case StoreItemType.SONG:
                            setCurrentSong(item);
                            break;
                        case StoreItemType.CAT:
                            setCurrentCat(item);
                            break;
                        case StoreItemType.TOWER:
                            setCurrentTower(item);
                            break;
                    }
                }
            }
        }
    }

    private void setCurrentCat(StoreItem item)
    {
        for (int i = 0; i < catNames.Length; i++)
        {
            if (catNames[i].Equals(item.getSaveDataName()))
            {
                catIndex = i;
                currentCatButton.sprite = catSprites[catIndex];
            }
        }
    }

    private void setCurrentTower(StoreItem item)
    {
        for (int i = 0; i < towerNames.Length; i++)
        {
            if (towerNames[i].Equals(item.getSaveDataName()))
            {
                towerIndex = i;
                currentTowerButton.sprite = towerSprites[towerIndex];
            }
        }
    }

    private void setCurrentSong(StoreItem item)
    {
       /* for (int i = 0; i < songNames.Length; i++)
        {
            if (songNames[i].Equals(item.getSaveDataName()))
            {
                songIndex = i;
                //currentCatButton.GetComponent<Image>().sprite = songSprites[catIndex];
            }
        }*/
    }

/*=================================================================================================
========== SETTING ITEMS ==========================================================================
=================================================================================================*/
    public void setCat(Image storeItemButton)
    {
        int catsOwned = PlayerPrefs.GetInt("CatsOwned") - 1;
        PlayerPrefs.SetInt(catNames[catIndex], (int)Ownership.PURCHASED);
        while(true)
        {
            if (catIndex < catNames.Length - 1)
            {
                catIndex++;
            }
            else
            {
                catIndex = 0;
            }
            if(PlayerPrefs.GetInt(catNames[catIndex]) == (int)Ownership.PURCHASED)
            {
                break;
            }
        }

        storeItemButton.sprite = catSprites[catIndex];
        PlayerPrefs.SetInt(catNames[catIndex], (int)Ownership.SELECTED);
        PlayerPrefs.SetInt("CatType", items[catNames[catIndex]]);
    }

    public void setTower(Image storeItemButton)
    {
        int towersOwned = PlayerPrefs.GetInt("TowersOwned") - 1;
        PlayerPrefs.SetInt(towerNames[towerIndex], (int)Ownership.PURCHASED);
        while (true)
        {
            if (towerIndex < towerNames.Length - 1)
            {
                towerIndex++;
            }
            else
            {
                towerIndex = 0;
            }
            if (PlayerPrefs.GetInt(towerNames[towerIndex]) == (int)Ownership.PURCHASED)
            {
                break;
            }
        }
        storeItemButton.sprite = towerSprites[towerIndex];
        PlayerPrefs.SetInt(towerNames[towerIndex], (int)Ownership.SELECTED);
        PlayerPrefs.SetInt("TowerType", items[towerNames[towerIndex]]);
    }

    public void setSong(Text storeItemButton)
    {
        int songsOwned = PlayerPrefs.GetInt("SongsOwned") - 1;
        PlayerPrefs.SetInt(songNames[songIndex], (int)Ownership.PURCHASED);
        while (true)
        {
            if (songIndex < songNames.Length - 1)
            {
                songIndex++;
            }
            else
            {
                songIndex = 0;
            }
            if (PlayerPrefs.GetInt(songNames[songIndex]) == (int)Ownership.PURCHASED)
            {
                break;
            }
        }
        storeItemButton.text = songLabels[songIndex];
        PlayerPrefs.SetInt(songNames[songIndex], (int)Ownership.SELECTED);
        PlayerPrefs.SetInt("SongType", items[songNames[songIndex]]);
    }

    /*=================================================================================================
    ========== PURCHASING ITEMS =======================================================================
    =================================================================================================*/

    public void purchaseCat(Button storeItemButton)
    {
        StoreItem item = storeItemButton.GetComponent<StoreItem>();
        if (item.getOwnership() == Ownership.ON_SALE)
        {
            int playerPurrency = PlayerPrefs.GetInt("Purrency");
            if (item.price <= playerPurrency)
            {
                int numCats = PlayerPrefs.GetInt("CatsOwned") + 1;
                PlayerPrefs.SetInt("CatsOwned", numCats);
                purchaseItem(storeItemButton, item, playerPurrency);

            }
        }
    }

    public void purchaseSong(Button storeItemButton)
    {
        StoreItem item = storeItemButton.GetComponent<StoreItem>();
        if (item.getOwnership() == Ownership.ON_SALE)
        {
            int playerPurrency = PlayerPrefs.GetInt("Purrency");
            if (item.price <= playerPurrency)
            {
                int numCats = PlayerPrefs.GetInt("SongsOwned") + 1;
                PlayerPrefs.SetInt("SongsOwned", numCats);
                purchaseItem(storeItemButton, item, playerPurrency);

            }
        }
    }

    public void purchaseTower(Button storeItemButton)
    {
        StoreItem item = storeItemButton.GetComponent<StoreItem>();
        if (item.getOwnership() == Ownership.ON_SALE)
        {
            int playerPurrency = PlayerPrefs.GetInt("Purrency");
            if (item.price <= playerPurrency)
            {
                int numCats = PlayerPrefs.GetInt("TowersOwned") + 1;
                PlayerPrefs.SetInt("TowersOwned", numCats);
                purchaseItem(storeItemButton, item, playerPurrency);

            }
        }
    }

    public void purchaseItem(Button storeItemButton, StoreItem item, int playerPurrency)
    {
        storeItemButton.GetComponent<Image>().color = purchasedColor;
        playerPurrency -= item.price;
        item.setOwnerShip(Ownership.PURCHASED);
        item.updatePriceTag();
        PlayerPrefs.SetInt(item.getSaveDataName(), (int)item.getOwnership());
        PlayerPrefs.SetInt("Purrency", playerPurrency);
        updatePurrency();
    }
}
