using UnityEngine;
using UnityEngine.UI;
public class BuyProduct : MonoBehaviour
{
    public int index;
    private Button _button;

    public void BuyButtonClick()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            InAppManager.Instance.BuyProduct(index);
        }
    }
}