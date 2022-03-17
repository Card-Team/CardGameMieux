using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoveDeck : MonoBehaviour
{
    public GameObject InterfaceSuppresionDeck;
    public TMP_Text Text;
    public String TextCard;
    public Button Oui;
    public Button Non;
    
    public void RemoveFichierDeckList()
    {
        InterfaceSuppresionDeck.gameObject.SetActive(true);
        Text.SetText("Etes vous sur de vouloir supprimer le deck : "+Text+" ?");
    }

    public void ButtonOui()
    {
        //var delete = File.Delete("");
    }

    public void ButtonNo()
    { 
        InterfaceSuppresionDeck.gameObject.SetActive(false);
    }
}
