using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    public AudioSource audioSource;
    public const string PPVolume = "PlayerPrefsVolume";
    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectsOfType<Audio>().Length >1)
        {
            DestroyImmediate(this.gameObject);
        }
        int vol = PlayerPrefs.GetInt(PPVolume,50);
        audioSource.GetComponent<AudioSource>().volume = vol/100f;
        audioSource.Play();
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
