using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LectureCartes : MonoBehaviour
{
    GameObject myGO;
    GameObject myText;
    Canvas myCanvas;
    Text text;
    RectTransform rectTransform;
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

        // CREER SANS Canvas !!!
        myGO = new GameObject();
        myGO.name = "Deck";
        myGO.AddComponent<Canvas>();

        myCanvas = myGO.GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        myGO.AddComponent<CanvasScaler>();
        myGO.AddComponent<GraphicRaycaster>();
        
        foreach (var file in files)
        {
            //faire un tri pour le nom des fichier avec le Path en affaiblissement
            String fileSansPath = file.Replace(Application.streamingAssetsPath + "/EffectsScripts/Card","");
            String fileSansPath1 = fileSansPath.Replace(@"\","");
            String fileSansPath2 = fileSansPath1.Replace(".lua","");
            Debug.Log(fileSansPath2);
            // Text
            myText = new GameObject();
            myText.transform.parent = myGO.transform;
            myText.name = "Carte "+fileSansPath2;

            text = myText.AddComponent<Text>();
            text.font = (Font)Resources.Load("MyFont");
            text.text = file;
            text.fontSize = 100;
        }


        // Text position
        rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(400, 200);
    }
}
