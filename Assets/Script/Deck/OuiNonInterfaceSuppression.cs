using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class OuiNonInterfaceSuppression : MonoBehaviour
{
    public String CarteASupprimer;
    
    public void ButtonOui()
    {
        String path = Application.persistentDataPath + "/" + CarteASupprimer + ".txt";
        Debug.Log("\n" + @path);
        File.Delete(@path);
        SceneManager.LoadScene("Deck");
    }

    public void ButtonNon()
    {
        gameObject.SetActive(false);
    }
}