using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;

public class RemoveDeck : MonoBehaviour
{
    public GameObject InterfaceSuppresionDeck;
    public TMP_Text Text;
    public String CardAppuye;
    
    public void RemoveFichierDeckList()
    {
        InterfaceSuppresionDeck.gameObject.SetActive(true);
        Text.SetText("Etes vous sur de vouloir supprimer '" + CardAppuye + "' du deck ?");
        InterfaceSuppresionDeck.GetComponent<OuiNonInterfaceSuppression>().CarteASupprimer=CardAppuye;
    }
}
