using UnityEngine;
using UnityEngine.SceneManagement;
using KidsItemsSort;
public class ForAni : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void LoadScene()
    {
        SceneManager.LoadScene("GoodSort");
    }
    public void objetOff()
    {
        IntitializeAdmob.Instance.ShowBanner();
        IntitializeAdmob.Instance.HideBigBanner();
        GameManager.instance.Ui_Off();
        if (GameManager.instance.IsHomePress == false)
        {
            GameManager.instance.LoadingPanel.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
