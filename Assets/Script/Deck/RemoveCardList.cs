using System;
using System.Collections;
using System.Collections.Generic;
using CardGameEngine.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoveCardList : MonoBehaviour
{
    public String NomCard;
    public void DeleteCardDeckList()
    {
        //clique sur une carte dans la liste : le supprimer de la liste
        //Debug.Log("carte a supprimer : "+NomCard);
        FindObjectOfType<LectureCartes>().listeCarteSelectionner.Remove(NomCard);
        int nbCarte = FindObjectOfType<LectureCartes>().listeCarteSelectionner.Count;
        FindObjectOfType<LectureCartes>().nbCartes.SetText(nbCarte+"");
        FindObjectOfType<LectureCartes>().nbCartes.color= new Color(255, 255, 255);
        var cardRenderer = FindObjectOfType<LectureCartes>().ListeCartes[NomCard];
        cardRenderer.SetTransparence(1);
        Destroy(gameObject);
    }
}
