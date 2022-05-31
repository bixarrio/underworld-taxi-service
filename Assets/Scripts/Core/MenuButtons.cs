using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{

    #region Public Methods
    
    public void OnPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OnQuit()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            Application.Quit();
#if UNITY_EDITOR
        if (Application.isEditor)
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion

}
