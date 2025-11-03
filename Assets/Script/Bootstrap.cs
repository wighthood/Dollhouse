using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private static bool _initialized = false;

    private void Awake()
    {
        if (_initialized)
        {
            Destroy(gameObject);
            return;
        }

        _initialized = true;
        
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
