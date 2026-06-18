using UnityEngine;

public class SetLoading : MonoBehaviour
{
    public GameObject tab_img, Normal_img;
    void Start()
    {
        if (ResCheck.ResolutionType==ResType.tab)
        {
            Normal_img.SetActive(false);
            tab_img.SetActive(true);
        }
        else
        {
            tab_img.SetActive(false);
            Normal_img.SetActive(true);
        }
    }
    private void OnEnable()
    {
        IntitializeAdmob.Instance.ShowBigBanner();
    }
    private void OnDisable()
    {
        IntitializeAdmob.Instance.ShowBanner();
        IntitializeAdmob.Instance.HideBigBanner();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
