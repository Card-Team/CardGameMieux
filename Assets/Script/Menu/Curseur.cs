using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Curseur : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    //[SerializeField]
    //public const string PlayerPrefCursor = "PlayerPrefCursor";
    
    [NonSerialized] public String path;
    Texture2D texture2D;
    DirectoryInfo dir;
    FileInfo[] files;
    
    void Start()
    {
        // path = Application.dataPath +"/Images/Menu/curseur";
        // dir = new DirectoryInfo(path);
        // files = dir.GetFiles("*.png");
        // Debug.Log(path);
        //
        //
        // if (cursorTexture == null) 
        //     Debug.Log("Load Cursor Fail"); 
        //
        // foreach (var file in files)
        // {
        //     //Debug.Log(file);
        //     // if (PlayerPrefs.HasKey("PlayerPrefCursor"))
        //     // {
        //     //     Debug.Log("PP Cursor: " + PlayerPrefs.GetString("PlayerPrefCursor"));
        //     // }
        // }
    }
    

    public void Suivant()
    {
        Debug.Log("entrer");
        Texture2D cursorTexture = (Texture2D) Resources.Load(Application.dataPath +"/Images/Menu/curseur/curseur6.png");
        Debug.Log(Application.dataPath);
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        //Debug.Log(cursorTextur.name);
        //PlayerPrefs.SetString(PlayerPrefCursor,cursorTexture.name);   
    }

    public void Precedant()
    {
        Debug.Log("entrer");
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        //Debug.Log(cursorTexture.name);
        //PlayerPrefs.SetString(PlayerPrefCursor,cursorTexture.name); 
    }
}