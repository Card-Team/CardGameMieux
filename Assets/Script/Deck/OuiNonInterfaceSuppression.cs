using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OuiNonInterfaceSuppression : MonoBehaviour
{
    public string CarteASupprimer;

    public void ButtonOui()
    {
        var path = Application.persistentDataPath + "/" + CarteASupprimer + ".txt";
        //Debug.Log("\n" + @path);
        File.Delete(path);
        SceneManager.LoadScene("Deck");
    }

    public void ButtonNon()
    {
        gameObject.SetActive(false);
    }
}