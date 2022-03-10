using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

[Serializable]
public class EnregistrementDeck : MonoBehaviour
{
    public Button enregistrer;
    public TMP_InputField nomDeck; 
    List<string> liste = FindObjectOfType<LectureCartes>().listeCarteSelectionner;
    public GameObject panelDeckExistant;
    public void Enregistrement()
    {
        if (liste.Count == 12)
        {
            if (File.Exists(nomDeck + ".txt"))
            {
                panelDeckExistant.SetActive(true);
                //interface de validation pour suppression du fichier deja existant
                File.Delete(nomDeck + ".txt");
                Debug.Log("fichier deja existant");
            }

            //creation de fichier
            using (FileStream fileStr = File.Create(nomDeck + ".txt"))
            {
                // Ajouter du texte au fichier  
                Byte[] textfile = new UTF8Encoding(true).GetBytes("TESTTTT");
                fileStr.Write(textfile, 0, textfile.Length);
                Debug.Log("creer le fichier");
            }
        }
    }
}