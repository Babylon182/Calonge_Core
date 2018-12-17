using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour 
{
    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }
    
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
