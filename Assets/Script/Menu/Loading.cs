using UnityEngine;

public class Loading : MonoBehaviour
{
    public RectTransform mainIcon;
    public float timeStep;
    public float oneStepAngle;
    public bool Avancer = true;

    private float statTime;

    //SCRIPT chargement
    //sources : https://www.youtube.com/watch?v=ltu27NLeIWc
    private void Start()
    {
        statTime = Time.time;
    }

    private void Update()
    {
        if (Avancer)
            if (Time.time - statTime >= timeStep)
            {
                var iconAngle = mainIcon.localEulerAngles;
                iconAngle.z += oneStepAngle;

                mainIcon.localEulerAngles = iconAngle;
                statTime = Time.time;
            }
    }

    public void Stop()
    {
        Avancer = false;
    }
}