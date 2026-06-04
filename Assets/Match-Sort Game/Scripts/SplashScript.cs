using UnityEngine;
using UnityEngine.SceneManagement;
public class SplashScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("wait1", 0.1f);
    }
    public void wait1()
    {
        SceneManager.LoadScene("MainMenu");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
