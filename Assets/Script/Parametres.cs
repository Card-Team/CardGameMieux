using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Parametres : MonoBehaviour
{
    public AudioMixer audiomMixer;
    public TMP_Text volume;
    
    public TMP_Dropdown ResolutionScreen;
    private Resolution[] resolutions;

    public GameObject panelReinitialisation;

    public void Start()
    {
        resolutions = Screen.resolutions;
        ResolutionScreen.ClearOptions();
        //mettre toutes les resolutions disponibles pour cette ecran (avec en premier celui utilisé)
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (!(Camera.main.aspect >= 1.7) && !(Camera.main.aspect >= 1.3))
            {
                continue;       //remonte sur le for
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
    }

    public void SetVolume(float Volume)
    {
        //Debug.Log(Volume);
        audiomMixer.SetFloat("volume", (Volume / 100) * 80 - 80);
        volume.text = (int) Math.Round(Volume) + " %";
    }

    public void SetResolution(int resolutionIndex)
    {
        Debug.Log(resolutionIndex);
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ApuieBoutonReinitialisation()
    {
        panelReinitialisation.SetActive(true);
    }
    public void retourReinitialisation()
    {
        panelReinitialisation.SetActive(false);
    }

    public void Reinitialisation()
    {
        PlayerPrefs.DeleteAll();
        Application.LoadLevel(0);
    }
}