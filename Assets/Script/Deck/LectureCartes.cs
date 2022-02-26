using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LectureCartes : MonoBehaviour
{
    public void Start()
    {
        NomCartes();
    }

    public void NomCartes()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath);
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/EffectsScripts/Card");
        //FileInfo[] files = di.GetFiles("*.lua;_ *");
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/EffectsScripts/Card", "*", SearchOption.AllDirectories)
            .Where(s => !(s.Contains("_")) && s.EndsWith(".lua")).ToList();

        foreach (var file in files)
        {
            Debug.Log(file);
        }
    }
}
