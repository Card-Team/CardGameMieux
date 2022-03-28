using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoutonMenu : MonoBehaviour
{
    private void Start()
    {
        
    }

    public void OuvrirCanvas(Canvas nouvelfenetre)
    {
        var actuel = GameObject.FindObjectOfType<Canvas>(); //recupere la fenetre active
        actuel.gameObject.SetActive(false);
        nouvelfenetre.gameObject.SetActive(true);
    }
    
    public void AllezDeck()
    {
        SceneManager.LoadScene("Deck");
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
