using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LectureCartes : MonoBehaviour
{
    public TextMeshPro textTemplate;
    TextMeshPro text;
    public void Start()
    {
        NomCartes();
    }

    public void NomCartes()
    {
        
        //recuperer tout les noms de cartes du jeu qui finnissent en .lua sauf ceux qui commencent par un '_'
        Directory.CreateDirectory(Application.streamingAssetsPath);
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/EffectsScripts/Card");
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/EffectsScripts/Card", "*.lua", SearchOption.AllDirectories)
            .Where(s => !(s.Contains("_")) && s.EndsWith(".lua")).ToList();

        int tour = 0;
        int posY = 3;
        int posX = -5;
        foreach (var file in files)
        {
            //faire un tri pour le nom des fichier avec le Path en affaiblissement
            String fileSansPath = file.Replace(Application.streamingAssetsPath + "/EffectsScripts/Card","");
            String fileSansPath1 = fileSansPath.Replace(@"\","");
            String fileSansPath2 = fileSansPath1.Replace(".lua","");
            Debug.Log(fileSansPath2);
            // Text
            text = Instantiate(textTemplate, null);
            text.text = fileSansPath2;      // NE MARCHE PAS POUR RENOMé le composant text
            text.SetText(fileSansPath2);
            text.fontSize = 5;
            text.transform.position = new Vector3(posX, posY, 0);
            tour+=1;
            if (tour%5 == 0)
            {
                posY = 3;
                posX += 5;
            }
            else
            {
                posY -= 1;
            }
        }
        
    }
}
