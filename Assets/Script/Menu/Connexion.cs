using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Connexion : MonoBehaviour
{
    [SerializeField]
    public const string Ip = "Ip";
    public const string PortC="PortConnexion";
    public TMP_InputField ip1;
    public TMP_InputField ip2;
    public TMP_InputField ip3;
    public TMP_InputField ip4;
    public TMP_InputField port;
    public GameObject panelConnexion;
    public TMP_Text textErreur;
    public Loading loading;
    public void Start()
    {
        if(PlayerPrefs.HasKey(Ip) && PlayerPrefs.HasKey(PortC) && (!Ip.Equals("....")) && (!PortC.Equals(".")))
        {
            //Debug.Log("PlayerPrefs IP : "+PlayerPrefs.GetString("Ip")+"\nPlayerPrefs Port : "+PlayerPrefs.GetString("Port"));
            //mettre l'IP depuis le PlayerPrefs enregistré
            Char[] mychar = {'.'};
            String[] name = PlayerPrefs.GetString("Ip").Split(mychar);
            TMP_InputField[] tabIp = {ip1,ip2,ip3,ip4};
            for (int i = 0; i < name.Length; i++)
            {
                tabIp[i].text = name[i];
            }
            //mettre le port depuis le PlayerPrefs enregistré
            port.text=(PlayerPrefs.GetString("PortConnexion"));
        }
        else
        {
            Debug.Log("Ip jamais rentrer ou Vide");
        }
    }

    IEnumerator OnCoroutine()
    {
        yield return new WaitForSeconds(3f);
        textErreur.SetText("Connexion Impossible");
        loading.Avancer = false;
    }

    public void AfficheIP()
    {
        Debug.Log("Ip : " + ip1.text + "." + ip2.text + "." + ip3.text + "." + ip4.text+"\nPort : "+port.text);
    }

    public void Rejoindre()
    {
        PlayerPrefs.SetString(Ip,ip1.text + "." + ip2.text + "." + ip3.text + "." + ip4.text);                //playerPrefs IP
        PlayerPrefs.SetString(PortC,port.text);                //playerPrefs port
    }
    public void AppuieConnexion()
    {
        //ouvrir une autre scene
        /*
        loading.Avancer = true;
        panelConnexion.SetActive(true);
        StartCoroutine(OnCoroutine());*/
    }
    
    public void RetourConnexion()
    {
        panelConnexion.SetActive(false);
    }
}
