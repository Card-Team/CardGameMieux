using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Close : MonoBehaviour
{
    public GameObject panel;
    
    public void close()
    {
        panel.SetActive(false);
    }
}
