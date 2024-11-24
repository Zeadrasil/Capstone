using UnityEngine;

public class LocalMusicManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Pass reference to play click sound effect
    public void PlayClick()
    {
        MusicManager.Instance.PlayClick();
    }
}
