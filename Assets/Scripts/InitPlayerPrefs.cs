using UnityEngine;
using System.Collections;

public enum CatType
{
    TABBY = 1,
    HALLOWEEN,
    COWBOY,
    FAT,
}

public enum TowerType
{
    BASE = 1,
    MIDGRADE,
    DELUXE,
    SUPER,
}

public enum SongType
{
    NYAN = 1,
    MEOW,
}

public enum Ownership
{
    INVALID,
    ON_SALE,
    PURCHASED,
    SELECTED,
}

public class InitPlayerPrefs : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        PlayerPrefs.DeleteAll();
        if(PlayerPrefs.GetInt("CatsOwned") == 0)
        {
            PlayerPrefs.SetInt("CatsOwned", 1);
        }

        if (PlayerPrefs.GetInt("TowersOwned") == 0)
        {
            PlayerPrefs.SetInt("TowersOwned", 1);
        }

        if (PlayerPrefs.GetInt("SongsOwned") == 0)
        {
            PlayerPrefs.SetInt("SongsOwned", 1);
        }

        if (PlayerPrefs.GetInt("CatType") == 0)
        {
            PlayerPrefs.SetInt("CatType", (int)CatType.TABBY);
        }

        if (PlayerPrefs.GetInt("TowerType") == 0)
        {
            PlayerPrefs.SetInt("TowerType", (int)TowerType.BASE);
        }

        if (PlayerPrefs.GetInt("SongType") == 0)
        {
            PlayerPrefs.SetInt("SongType", (int)SongType.NYAN);
        }

        if (PlayerPrefs.GetInt("Purrency") == 0)
        {
            PlayerPrefs.SetInt("Purrency", 1000);
        }
	}
}
