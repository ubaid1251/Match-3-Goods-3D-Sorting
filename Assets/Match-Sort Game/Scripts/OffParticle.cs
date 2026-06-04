using UnityEngine;

public class OffParticle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    Invoke("WaitOFf", 1.2f);
    //}
    private void OnEnable()
    {
        Invoke("WaitOFf", 1.2f);

    }
    // Update is called once per frame
    void WaitOFf()
    {
        gameObject.SetActive(false);
    }
}
