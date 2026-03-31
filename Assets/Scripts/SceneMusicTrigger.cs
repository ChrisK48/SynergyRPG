using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    public AudioClip sceneMusic;

    void Start()
    {
        AudioManager.instance.PlayMusic(sceneMusic);
    }
}