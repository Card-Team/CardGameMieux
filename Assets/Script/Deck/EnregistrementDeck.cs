using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

[Serializable]
public class EnregistrementDeck : MonoBehaviour
{
    public TMP_InputField nomDeck;
    private List<string> liste;
    public GameObject panelDeckExistant;
    public GameObject pas12Cartes;
    public GameObject panelNomDeckVide;
    public GameObject deckEnregistrer;

    void Start()
    {
        liste = FindObjectOfType<LectureCartes>().listeCarteSelectionner;
    }

    IEnumerator OnCoroutine(GameObject panel)
    {
        yield return new WaitForSeconds(5);
        panel.SetActive(false);
        if (panel == deckEnregistrer)
        {
            Debug.Log("SORTIR");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            deckEnregistrer.SetActive(false);
            panelNomDeckVide.SetActive(false);
            pas12Cartes.SetActive(false);
        }
    }

    public void Enregistrement()
    {
        //Debug.Log("Nom du deck : " + nomDeck.text + nomDeck.text.Length);
        if (String.IsNullOrWhiteSpace(nomDeck.text))
        {
            //Visible 5 sec
            //Debug.Log("Nom du deck : ");
            panelNomDeckVide.gameObject.SetActive(true);
            StartCoroutine(OnCoroutine(panelNomDeckVide));
        }
        else if (liste.Count == 12)
        {
            var nomDeckText = nomDeck.text + ".txt";
            if (File.Exists(nomDeckText))
            {
                //interface de validation pour suppression du fichier deja existant
                panelDeckExistant.SetActive(true);
                return;
            }
            // Ajouter du texte au fichier  
            File.WriteAllLines(Application.persistentDataPath+"/"+nomDeckText, liste);
            //Debug.Log("fichier"+nomDeckText+ " creer");
            deckEnregistrer.gameObject.SetActive(true);
            StartCoroutine(OnCoroutine(deckEnregistrer));
        }
        else
        {
            pas12Cartes.gameObject.SetActive(true);
            StartCoroutine(OnCoroutine(pas12Cartes));
        }
    }

    public void CliqueOk()
    {
        Debug.Log(nomDeck.text);
        File.WriteAllLines(Application.persistentDataPath+"/"+nomDeck.text+".txt", liste);
        deckEnregistrer.gameObject.SetActive(true);
        panelDeckExistant.gameObject.SetActive(false);
        StartCoroutine(OnCoroutine(deckEnregistrer));
    }

    public void CliqueNon()
    {
        panelDeckExistant.gameObject.SetActive(false);
    }
}