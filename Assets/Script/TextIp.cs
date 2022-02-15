using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class TextIp : MonoBehaviour
{
    public TextMeshProUGUI champIp;
    private string _localIP;

    public TMP_InputField ip1;
    public TMP_InputField ip2;
    public TMP_InputField ip3;
    public TMP_InputField ip4;
    public TMP_InputField port;

    public void Start()
    {
        SaveIP();
    }

    private void SaveIP()
    {
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530); //test avec quelle carte reseaux virtuel elle se connecte a google
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            _localIP = endPoint.Address.ToString();
            champIp.text += _localIP;
        }
    }

    public void CopieIP()
    {
        GUIUtility.systemCopyBuffer = _localIP;
    }

    public void AfficheIP()
    {
        Debug.Log("Ip : " + ip1.text + "." + ip2.text + "." + ip3.text + "." + ip4.text+"\nPort : "+port.text);
    }
}