using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Heberger : MonoBehaviour
{
    public TMP_InputField port;
    public const string PortH="PortHebergement"; 
    public GameObject panelHeberger;
    public TMP_Text compteur;
    void Start()
    {
        if (PlayerPrefs.HasKey(PortH) && !PortH.Equals("."))
        {
            //mettre le port depuis le PlayerPrefs enregistré
            port.text=(PlayerPrefs.GetString("PortHebergement"));
        }
        else
        {
            Debug.Log("Port jamais rentrer ou Vide");
        }
    }
    IEnumerator OnCoroutine(int i)
    {
        while (i>0)
        {
            //compteur de 1 seconde
            yield return new WaitForSeconds(1f);
            compteur.SetText(""+i--);
            if (i < 10)
            {
                compteur.color = Color.red;
            }
            if (i==0)
            {
                panelHeberger.SetActive(false);
            }
        }
    }
    public void Herberger()
    {
        PlayerPrefs.SetString(PortH,port.text);                //playerPrefs port
    }

    public void AppuieHerberger()
    {
        panelHeberger.SetActive(true);
        StartCoroutine(OnCoroutine(60));
    }
    
    public void RetourHerberger()
    {
        panelHeberger.SetActive(false);
    }
}

