using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomButton : MonoBehaviour
{
    public const string Nomdeck = "NomDeck";

    // Start is called before the first frame update
    public string text;
    public Canvas nouvelfenetre;
    
    public void CliqueButton()
    {
        //Debug.Log("Deck : "+text);
        PlayerPrefs.SetString(Nomdeck,text);                //playerPrefs NomDeck Variables publique
        var actuel = GameObject.FindObjectOfType<Canvas>(); //recupere la fenetre active
        actuel.gameObject.SetActive(false);                 //la desactiver 
        nouvelfenetre.gameObject.SetActive(true);           //et reactiver la nouvelle donné en parametre
    }
}
