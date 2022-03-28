using UnityEngine;

public class Audio : MonoBehaviour
{
    public const string PPVolume = "PlayerPrefsVolume";
    public AudioSource audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        if (FindObjectsOfType<Audio>().Length >1)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        
        int vol = PlayerPrefs.GetInt(PPVolume, 50);
        audioSource.volume = vol / 100f;
        audioSource.Play();
        DontDestroyOnLoad(this.gameObject);

    }

    // Update is called once per frame
    private void Update()
    {
    }
}