using UnityEngine;

public class showBanner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IntitializeAdmob.Instance.ShowBanner();
    }

}
