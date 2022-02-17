using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Connexion : MonoBehaviour
{
    public TMP_InputField ip1;
    public TMP_InputField ip2;
    public TMP_InputField ip3;
    public TMP_InputField ip4;
    public TMP_InputField port;
    // Start is called before the first frame update
    public void AfficheIP()
    {
        Debug.Log("Ip : " + ip1.text + "." + ip2.text + "." + ip3.text + "." + ip4.text+"\nPort : "+port.text);
    }
}
