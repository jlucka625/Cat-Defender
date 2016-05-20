using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RandomDrop : NetworkBehaviour
{
    [Tooltip("The dropping item")]
    public GameObject ItemDrop = null;
    public int spawnTime = 5;
    private bool gameStart = false;

    void Update()
    {
        if (isServer && !gameStart)
        {
            GameObject player1 = GameManager.GetPlayer1();
            GameObject player2 = GameManager.GetPlayer2();
            if (player1 != null && player2 != null)
            {
                gameStart = true;
                StartCoroutine(spawnDrop());
            }
        }
    }

    IEnumerator spawnDrop()
    {
        while (true)
        {
            Vector3 pos = new Vector3(Random.Range(Screen.width/5, 4 * Screen.width/5), Screen.height, 0.0f);
            pos = Camera.main.ScreenToWorldPoint(pos);
            pos.z = 0.0f;
            GameObject newItem = Instantiate(ItemDrop, pos, transform.rotation) as GameObject;
            newItem.GetComponent<ItemDrop>().randomizeItem();
            NetworkServer.Spawn(newItem);
            if(HostManager.GameOver)
            {
                break;
            }
            yield return new WaitForSeconds(spawnTime);

        }
    }
}
