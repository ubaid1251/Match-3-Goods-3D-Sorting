using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MenuMain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void PlayBtn()
    {
        SoundManager.instance.PlayEffect_Instance(7);

        SceneManager.LoadScene("GoodSort");
    }
    public void PlaySound()
    {
        SoundManager.instance.PlayEffect_Instance(7);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
