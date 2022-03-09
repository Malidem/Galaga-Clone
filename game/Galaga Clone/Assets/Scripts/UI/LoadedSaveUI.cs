using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedSaveUI : BaseUI
{
    public void CallSwitchSceneAndSave(string sceneName)
    {
        StartCoroutine(SwitchSceneAndSave(sceneName));
    }

    private IEnumerator SwitchSceneAndSave(string sceneName)
    {
        yield return StartCoroutine(Constants.SaveDataToDatabase());
        LoadScene(sceneName);
    }
}
