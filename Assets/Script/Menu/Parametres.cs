using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Parametres : MonoBehaviour
{
    public const string PPVolume = "PlayerPrefsVolume";
    public const string PPBordures = "PlayerPrefsBordures";
    public AudioSource Audio;
    public TMP_Text volume;
    public TMP_Dropdown ResolutionScreen;
    public GameObject panelReinitialisation;
    public GameObject slider;
    public GameObject mute;
    private Resolution[] resolutions;

    public void Start()
    {
        resolutions = Screen.resolutions;
        ResolutionScreen.ClearOptions();
        //mettre toutes les resolutions disponibles pour cette ecran (avec en premier celui utilisé)
        var options = new List<string>();
        var currentResolutionIndex = 0;
        for (var i = 0; i < resolutions.Length; i++)
        {
            if (!((float) resolutions[i].width / resolutions[i].height >= 1.7)) continue; //remonte sur le for

            var option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        ResolutionScreen.AddOptions(options);
        ResolutionScreen.value = currentResolutionIndex;
        ResolutionScreen.RefreshShownValue();

        var volI = PlayerPrefs.GetInt(PPVolume, 50);
        Audio.GetComponent<AudioSource>().volume = volI / 100f;
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
        var vol = (int) Math.Round(Volume);
        Audio.GetComponentInChildren<AudioSource>().volume = vol / 100f;
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

    public void BordureChange(bool change)
    {
        if (change)
        {
            //full screen 
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerPrefs.SetInt(PPBordures, 1);
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            PlayerPrefs.SetInt(PPBordures, 0);
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        //Debug.Log(resolutionIndex);
        var resolution = resolutions[resolutionIndex];
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

        Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
        Application.Quit();
    }
}