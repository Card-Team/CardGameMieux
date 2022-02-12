using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomButton : MonoBehaviour
{
    // Start is called before the first frame update
    public string text;
    
    public void CliqueButton()
    {
        Debug.Log(text);
    }
}
