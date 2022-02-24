using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomButton : MonoBehaviour
{
    [SerializeField]
    public const string Nomdeck = "NomDeck";

    // Start is called before the first frame update
    public string text;
    public Canvas nouvelfenetre;
    
    public void CliqueButton()
    {
        //Debug.Log("Deck : " + PlayerPrefs.GetString("NomDeck"));  //afficher la valeur du PlayerPrefs 'NomDeck'
        PlayerPrefs.SetString(Nomdeck,text);                //playerPrefs NomDeck Variables publique
        var actuel = GameObject.FindObjectOfType<Canvas>(); //recupere la fenetre active
        actuel.gameObject.SetActive(false);                 //la desactiver 
        nouvelfenetre.gameObject.SetActive(true);           //et reactiver la nouvelle donné en parametre
    }
}
