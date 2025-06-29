using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] SoundPlayer soundPlayer;
    [SerializeField] AudioClip music;

    static BGMPlayer instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //this is an anti singleton, if a new one gets made it deletes the old instance instead of keeping the original instance.
        if(instance)
            Destroy(instance.gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);

        soundPlayer.PlaySound(music);
    }
}
