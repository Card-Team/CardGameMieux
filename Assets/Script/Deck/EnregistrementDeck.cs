using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[Serializable]
public class EnregistrementDeck : MonoBehaviour
{
    public TMP_InputField nomDeck;
    public GameObject panelDeckExistant;
    public GameObject pas12Cartes;
    public GameObject panelNomDeckVide;
    public GameObject deckEnregistrer;
    private List<string> liste;

    private void Start()
    {
        liste = FindObjectOfType<LectureCartes>().listeCarteSelectionner;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            deckEnregistrer.SetActive(false);
            panelNomDeckVide.SetActive(false);
            pas12Cartes.SetActive(false);
        }
    }

    private IEnumerator OnCoroutine(GameObject panel)
    {
        yield return new WaitForSeconds(4);
        panel.SetActive(false);
    }

    public void Enregistrement()
    {
        var nomDeckText = nomDeck.text + ".txt";
        //Debug.Log("Nom du deck : " + nomDeck.text + nomDeck.text.Length);
        //champs nom vide
        if (string.IsNullOrWhiteSpace(nomDeck.text))
        {
            //Visible 5 sec 
            //Debug.Log("Nom du deck : ");
            panelNomDeckVide.gameObject.SetActive(true);
            StartCoroutine(OnCoroutine(panelNomDeckVide));
        }
        //si le nombre de carte == a 12
        else if (liste.Count == 12)
        {
            //nom de fichier deja existant
            if (File.Exists(Application.persistentDataPath + @"\" + nomDeckText))
            {
                //interface de validation pour suppression du fichier deja existant
                panelDeckExistant.SetActive(true);
            }
            else
            {
                // Ajouter du texte au fichier  
                File.WriteAllLines(Application.persistentDataPath + "/" + nomDeckText, liste);
                //Debug.Log("fichier"+nomDeckText+ " creer");
                deckEnregistrer.gameObject.SetActive(true);
                StartCoroutine(OnCoroutine(deckEnregistrer));
            }
        }
        else
        {
            pas12Cartes.gameObject.SetActive(true);
            StartCoroutine(OnCoroutine(pas12Cartes));
        }
    }

    public void CliqueOk()
    {
        //Debug.Log(nomDeck.text);
        File.WriteAllLines(Application.persistentDataPath + "/" + nomDeck.text + ".txt", liste);
        deckEnregistrer.gameObject.SetActive(true);
        panelDeckExistant.gameObject.SetActive(false);
        StartCoroutine(OnCoroutine(deckEnregistrer));
        deckEnregistrer.gameObject.SetActive(true);
    }

    public void CliqueNon()
    {
        panelDeckExistant.gameObject.SetActive(false);
    }
}