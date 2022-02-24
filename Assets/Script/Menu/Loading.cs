using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    public RectTransform mainIcon;
    public float timeStep;
    public float oneStepAngle;
    float statTime;
    //SCRIPT chargement
    //sources : https://www.youtube.com/watch?v=ltu27NLeIWc
    void Start()
    {
        statTime = Time.time;
    }
    void Update()
    {
        if (Time.time - statTime >= timeStep)
        {
            Vector3 iconAngle = mainIcon.localEulerAngles;
            iconAngle.z += oneStepAngle;

            mainIcon.localEulerAngles = iconAngle;
            statTime = Time.time;
        }
    }
}
