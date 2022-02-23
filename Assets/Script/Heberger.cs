using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public TMP_InputField port;
    public const string PortH="PortHebergement";
    void Start()
    {
        if (PlayerPrefs.HasKey(PortH) && !PortH.Equals("."))
        {
            //mettre le port depuis le PlayerPrefs enregistré
            port.text=(PlayerPrefs.GetString("PortH"));
        }
        else
        {
            Debug.Log("Port jamais rentrer ou Vide");
        }
    }

    public void Herberger()
    {
        PlayerPrefs.SetString(PortH,port.text);                //playerPrefs port
    }
}
