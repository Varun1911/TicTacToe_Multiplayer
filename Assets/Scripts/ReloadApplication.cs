using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadApplication : MonoBehaviour
{
  
    void Update()
    {
        if((Input.GetKey(KeyCode.LeftControl) ||  Input.GetKey(KeyCode.RightControl)) && (Input.GetKeyDown(KeyCode.R)))
        {
            SceneManager.LoadScene(0);
        }
    }
}
