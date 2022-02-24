using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangementScene : MonoBehaviour
{
    public string sceneNameLoad;
    void Start()
    {

    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneNameLoad);
    }

}
