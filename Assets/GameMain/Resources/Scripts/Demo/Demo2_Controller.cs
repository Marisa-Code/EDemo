using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
public class Demo2_Controller : MonoBehaviour
{
    public void EnterGame()
    {
        SceneComponent Scene
            = UnityGameFramework.Runtime.GameEntry.GetComponent<SceneComponent>();


        var loadedSceneAssetNames = Scene.GetLoadedSceneAssetNames();
        for (int i = 0; i < loadedSceneAssetNames.Length; i++)
        {
            Scene.UnloadScene(loadedSceneAssetNames[i]);
        }
        Scene.LoadScene("Assets/Scenes/Demo2_Game.unity", this);

    }
}
