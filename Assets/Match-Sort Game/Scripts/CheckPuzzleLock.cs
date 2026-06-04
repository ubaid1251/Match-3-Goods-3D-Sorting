using UnityEngine;
using UnityEngine.UI;
public class CheckPuzzleLock : MonoBehaviour
{
    public GameObject butn;
    void Start()
    {
        if (PlayerPrefs.GetInt("Purchased") == 1)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Lock")
        {
          //  Destroy(collision.transform.Gameobject);
          butn.GetComponent<Button>().interactable = false;
        }
       else if (collision.name == "5")
        {
            //  Destroy(collision.transform.Gameobject);
            butn.GetComponent<Button>().interactable = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
