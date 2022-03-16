using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoveCardList : MonoBehaviour
{
    public String NomCard;
    public GameObject Croix;

    private List<String> listeCarte;
    public void AppuieCardDeckList()
    {
        //clique sur une carte dans la liste : le supprimer de la liste
        //Debug.Log("carte a supprimer : "+NomCard);
        int nbCarte = FindObjectOfType<LectureCartes>().listeCarteSelectionner.Count;
        FindObjectOfType<LectureCartes>().nbCartes.SetText(nbCarte+"");
        listeCarte=FindObjectOfType<LectureCartes>().listeCarteSelectionner;
        listeCarte.Remove(NomCard);
        Destroy(gameObject);
        Destroy(Croix);
        //TODO REORGANISER LA LISTE DES CARTES car sinon il y'a des troues donc des epsaces
    }
    
    public void RemoveFichierDeckList()
    {
        //supprimer le fichier deck
        
    }
}
