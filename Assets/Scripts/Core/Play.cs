using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{

    #region Public Methods
    
    public void OnPlay()
    {
        SceneManager.LoadScene(1);
    }

    #endregion

}
