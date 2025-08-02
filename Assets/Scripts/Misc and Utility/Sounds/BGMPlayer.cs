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
        //its made this way to change songs by going into a scene that should have a new song
        //however if its meant to keep playing the same song it will delete the new one instead, like a normal singleton
        if (instance)
        {
            if(music == instance.music)
            {
                Destroy(gameObject);
                return;
            }

            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        soundPlayer.PlaySound(music);
    }
}
