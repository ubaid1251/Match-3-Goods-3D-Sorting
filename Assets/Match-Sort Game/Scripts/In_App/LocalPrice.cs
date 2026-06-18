using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalPrice : MonoBehaviour
{
    private TMP_Text _priceText;
    public InAppProduct.InAppProductType itemType;

    private void Awake()
    {
        _priceText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        foreach (var t in InAppManager.Instance.purchaseIDController)
        {
            if (itemType != t.itemType) continue;
            _priceText.text = t.localPrice;
            break;
        }
    }
}