using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class TextIp : MonoBehaviour
{
    public TextMeshProUGUI champIp;
    private string _localIP;

    public void Start()
    {
        SaveIP();
    }

    private void SaveIP()
    {
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            _localIP = endPoint.Address.ToString();
            champIp.text += _localIP;
        }
    }

    public void CopieIP()
    {
        GUIUtility.systemCopyBuffer = _localIP;
    }
}
