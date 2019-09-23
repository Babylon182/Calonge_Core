using CalongeCore.Events;
using CalongeCore.PauseManager;
using MEC;
using UnityEngine.SceneManagement;

public class CustomSceneManager : Singleton<CustomSceneManager>
{
    public void ChangeScene(int sceneIndex)
    {
        Timing.KillCoroutines();
        SceneManager.LoadSceneAsync(sceneIndex);
        PauseManager.Instance.Unpause();
        EventsManager.ClearDictionary();

        //SceneManager.LoadScene(sceneIndex);
    }

    public void ChangeScene(string sceneName)
    {
        Timing.KillCoroutines();
        SceneManager.LoadSceneAsync(sceneName);
        PauseManager.Instance.Unpause();
        EventsManager.ClearDictionary();

        //SceneManager.LoadScene(sceneName);
    }
}
