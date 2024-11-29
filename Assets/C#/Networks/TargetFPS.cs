using UnityEngine;

public class TargetFPS
{
    private int target = 60;

    public void Init()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
    }
}