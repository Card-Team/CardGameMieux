using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Parametres : MonoBehaviour
{
    public AudioSource Audio;
    public TMP_Text volume;
    public TMP_Dropdown ResolutionScreen;
    private Resolution[] resolutions;
    public GameObject panelReinitialisation;
    public GameObject slider;
    public const string PPVolume = "PlayerPrefsVolume";
    public GameObject mute;

    public void Start()
    {
        resolutions = Screen.resolutions;
        ResolutionScreen.ClearOptions();
        //mettre toutes les resolutions disponibles pour cette ecran (avec en premier celui utilisé)
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (!((float) resolutions[i].width / resolutions[i].height >= 1.7))
            {
                continue; //remonte sur le for
            }

            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        ResolutionScreen.AddOptions(options);
        ResolutionScreen.value = currentResolutionIndex;
        ResolutionScreen.RefreshShownValue();

        int volI = PlayerPrefs.GetInt(PPVolume, 50);
        Audio.GetComponent<AudioSource>().volume = volI/100f;
        volume.text = volI + " %";
        slider.GetComponent<Slider>().value = volI;
        if (volI == 0)
        {
            volume.text = " ";
            mute.SetActive(true);
        }
    }

    public void SetVolume(float Volume)
    {
        //Debug.Log(Volume);
        int vol = (int) Math.Round(Volume);
        Audio.GetComponentInChildren<AudioSource>().volume = vol/100f;
        volume.text = vol + " %";
        if (vol == 0)
        {
            volume.text = " ";
            mute.SetActive(true);
        }
        else
        {
            mute.SetActive(false);
        }
        PlayerPrefs.SetInt(PPVolume, vol);
    }

    public void SetResolution(int resolutionIndex)
    {
        //Debug.Log(resolutionIndex);
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ApuieBoutonReinitialisation()
    {
        panelReinitialisation.SetActive(true);
    }

    public void RetourReinitialisation()
    {
        panelReinitialisation.SetActive(false);
    }

    public void Reinitialisation()
    {
        PlayerPrefs.DeleteAll();
        
        string[] endings = new string[]{
            "exe", "x86", "x86_64", "app"
        };
        string executablePath = Application.dataPath + "/..";
        foreach (string file in System.IO.Directory.GetFiles(executablePath)) {
            foreach (string ending in endings) {
                if (file.ToLower ().EndsWith ("." + ending)) {
                    System.Diagnostics.Process.Start (executablePath + file);
                    Application.Quit ();
                    return;
                }
            }
             
        }
    }
}