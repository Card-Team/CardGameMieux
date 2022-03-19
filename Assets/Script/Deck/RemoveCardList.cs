using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoveCardList : MonoBehaviour
{
    public String NomCard;
    public void AppuieCardDeckList()
    {
        //clique sur une carte dans la liste : le supprimer de la liste
        //Debug.Log("carte a supprimer : "+NomCard);
        FindObjectOfType<LectureCartes>().listeCarteSelectionner.Remove(NomCard);
        int nbCarte = FindObjectOfType<LectureCartes>().listeCarteSelectionner.Count;
        FindObjectOfType<LectureCartes>().nbCartes.SetText(nbCarte+"");
        Destroy(gameObject);
    }
}
