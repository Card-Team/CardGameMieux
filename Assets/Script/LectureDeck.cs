using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LectureDeck : MonoBehaviour
{
    public List<string> listFile = new List<string>();
    public NomButton buttonTemplate;            //Game object avec forcement un composant Nombutton
    public Canvas nouvelfenetre;
    void OnEnable()
    {
        LireFichier();
    }

    public void LireFichier()
    {
        Directory.CreateDirectory(Application.persistentDataPath);
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = di.GetFiles("*.txt");
        foreach (FileInfo file in files)
        {
            listFile.Add(file.Name);
            //Debug.Log(file.Name);
        }
        //supprimer les buttons
        
         var children = new List<GameObject>();
 foreach (Transform child in transform) children.Add(child.gameObject);
 children.ForEach(child => Destroy(child));

        //ajouter les boutons 
        for(int i=0;i<listFile.Count;i++)
        {
            NomButton button = Instantiate(buttonTemplate, this.transform, false);  //creer et un copie un bouton
            button.text = listFile[i];                                                           //donne aux champs texte le nom du bouton
            button.nouvelfenetre = nouvelfenetre;                                                          
            button.gameObject.SetActive(true);  
            button.GetComponentInChildren<TextMeshProUGUI>().SetText(listFile[i]);
        }
    }

}