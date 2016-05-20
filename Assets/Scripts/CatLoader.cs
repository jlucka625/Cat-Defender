using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public enum SoundNames
{
    INTRO,
    TUNA_CATCH,
    CATNIP_CATCH,
    BOMB,
    DEFEAT,
    VICTORY,
}
public class CatLoader : NetworkBehaviour {
    public Sprite[] catSprites;
    public RuntimeAnimatorController[] catAnimations;

    public AudioClip[] baseCatNoises;
    public AudioClip[] halloweenCatNoises;
    public AudioClip[] cowboyCatNoises;
    public AudioClip[] fatCatNoises;

    private AudioClip[] currentCatNoises;

    private AudioSource audioPlayer;

    private Sprite currentSprite;
    private RuntimeAnimatorController currentAnimator;

    [SyncVar]
    private int index;

    // Use this for initialization
    void Start () {
    }

    public override void OnStartClient()
    {
        audioPlayer = GetComponent<AudioSource>();
        syncCatSprites();

    }

    [ClientRpc]
    public void RpcSetCatAnimation(string name)
    {
        GetComponent<Animator>().SetTrigger(name);
    }

    [ClientRpc]
    public void RpcSetCatSprites(int index)
    {
        this.index = index;
        syncCatSprites();
    }

    private void syncCatSprites()
    {
        currentSprite = catSprites[index];
        currentAnimator = catAnimations[index];
        GetComponent<Animator>().runtimeAnimatorController = currentAnimator;
        GetComponent<SpriteRenderer>().sprite = currentSprite;
        switch(index)
        {
            case 0: currentCatNoises = baseCatNoises; break;
            case 1: currentCatNoises = halloweenCatNoises; break;
            case 2: currentCatNoises = cowboyCatNoises; break;
            case 3: currentCatNoises = fatCatNoises; break;
        }
        playCatSounds(SoundNames.INTRO);
    }

    public void playCatSounds(SoundNames soundName)
    {
        Debug.Log("NEED TO PLAY SOUND");
        audioPlayer.clip = currentCatNoises[(int)soundName];
        audioPlayer.Play();
    }
}
