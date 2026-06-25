using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

[System.Serializable]
public class InAppProduct
{
    public string ProductName = "";
    public string purchaseID = "";
    public InAppProductType itemType;
    public ProductType purchaseableType = ProductType.NonConsumable;
    public string price;
    public string localPrice;

    public enum InAppProductType
    {
        remove_ads,

        coin_500,
        coin_1200,
        coin_2500,
        coin_5000,
        coin_10000,
        coin_15000,
        coin_25000,
        get_coins_50000
    }
}

public class InAppManager : MonoBehaviour
{
    [Serializable]
    public class PurchaseItem
    {
        public string purchaseID;
        public InAppProduct.InAppProductType itemType;
        public ProductType purchaseableType;
        public string localPrice;
        public string price;
    }

    [SerializeField] public PurchaseItem[] purchaseIDController;

    private StoreController _storeController;
    private bool _initStarted;
    private bool _initialized;

    public bool IsInitialized => _initialized && _storeController != null;
    public static InAppManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private async void Start()
    {
        try
        {
            if (_initStarted) return;
            _initStarted = true;

            await InitializeAsync();
        }
        catch (Exception e)
        {
            print("exception"); // TODO handle exception
        }
    }

    private async Task InitializeAsync()
    {
        Debug.Log("IAP v5 InitializeAsync() called");

        if (purchaseIDController == null || purchaseIDController.Length == 0)
        {
            Debug.Log("purchaseIDController is empty");
            return;
        }

        try
        {
            _storeController = UnityIAPServices.StoreController();

            // ✅ Bind BEFORE connect
            _storeController.OnStoreConnected -= OnStoreConnected;
            _storeController.OnStoreConnected += OnStoreConnected;

            _storeController.OnStoreDisconnected -= OnStoreDisconnected;
            _storeController.OnStoreDisconnected += OnStoreDisconnected;

            await _storeController.Connect();

            // ✅ Bind AGAIN after connect (Unity bug workaround)
            _storeController.OnStoreConnected -= OnStoreConnected;
            _storeController.OnStoreConnected += OnStoreConnected;

            _storeController.OnStoreDisconnected -= OnStoreDisconnected;
            _storeController.OnStoreDisconnected += OnStoreDisconnected;

            // ✅ Register purchase + product callbacks
            RegisterCallbacks(_storeController);

            Debug.Log("Store connected");

            var productDefinitions = purchaseIDController
                .Select(p => new ProductDefinition(p.purchaseID, p.purchaseableType))
                .ToList();

            _storeController.FetchProducts(productDefinitions);
            _storeController.FetchPurchases();
        }
        catch (Exception ex)
        {
            Debug.Log($"IAP init failed: {ex}");
        }
    }

    private void RegisterCallbacks(StoreController storeController)
    {
        // Remove first (prevents stacking)
        storeController.OnPurchasePending -= OnPurchasePending;
        storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
        storeController.OnPurchaseFailed -= OnPurchaseFailed;
        storeController.OnPurchaseDeferred -= OnPurchaseDeferred;

        storeController.OnPurchasePending += OnPurchasePending;
        storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
        storeController.OnPurchaseFailed += OnPurchaseFailed;
        storeController.OnPurchaseDeferred += OnPurchaseDeferred;

        storeController.OnProductsFetched += OnProductsFetched;
        storeController.OnProductsFetchFailed += OnProductsFetchFailed;

        storeController.OnPurchasesFetched += OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
    }

    private void OnDestroy()
    {
        if (_storeController == null) return;

        _storeController.OnProductsFetched -= OnProductsFetched;
        _storeController.OnProductsFetchFailed -= OnProductsFetchFailed;

        _storeController.OnPurchasePending -= OnPurchasePending;
        _storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
        _storeController.OnPurchaseFailed -= OnPurchaseFailed;
        _storeController.OnPurchaseDeferred -= OnPurchaseDeferred;

        _storeController.OnPurchasesFetched -= OnPurchasesFetched;
        _storeController.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;
    }

    private void OnProductsFetched(List<Product> products)
    {
        Debug.Log($"Products fetched: {products.Count}");

        foreach (var item in purchaseIDController)
        {
            var product = products.FirstOrDefault(p => p.definition.id == item.purchaseID);
            if (product == null) continue;

            item.localPrice = product.metadata.localizedPriceString;
            item.price = item.localPrice;

            Debug.Log($"Product ready: {item.purchaseID} price={item.localPrice}");
        }

        _initialized = true; // ✅ MOVE HERE
        Debug.Log("IAP FULLY INITIALIZED ✅");
    }

    private void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        Debug.Log($"Products fetch failed: {failure}");
    }

    private void OnPurchasesFetched(Orders orders)
    {
        Debug.Log("Previous purchases fetched");
        // Restore non-consumables/subscriptions here if needed.
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
    {
        Debug.Log($"Purchases fetch failed: {failure}");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void BuyProduct(int productIndex)
    {
        if (!IsInitialized || _storeController == null)
        {
            Debug.LogWarning("IAP not ready yet 🚫");
            return;
        }

        var productId = purchaseIDController[productIndex].purchaseID;
        var product = _storeController.GetProductById(productId);

        if (product == null)
        {
            Debug.Log($"Product not found: {productId}");
            return;
        }

        // 🔥 Rebind (fix Unity bug)
        RegisterCallbacks(_storeController);

        Debug.Log("Purchasing product...");

        _storeController.PurchaseProduct(product);
    }

    private void OnStoreConnected()
    {
        Debug.Log("Store connected ✅");
    }

// 🔥 THIS was your error — FIXED SIGNATURE
    private void OnStoreDisconnected(StoreConnectionFailureDescription failure)
    {
        Debug.LogWarning("Store disconnected ❌: " + failure);
    }

    private void OnPurchasePending(PendingOrder order)
    {
        Debug.Log($"Purchase pending: {order.Info.TransactionID}");

        // Grant content here ONLY when your own validation / save succeeds.
        // For consumables, confirm only after fulfillment is safely recorded.
        // For simple local testing, you can confirm immediately:

        _storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseConfirmed(Order order)
    {
        Debug.Log($"Purchase confirmed: {order.Info.TransactionID}");
        var item = order.CartOrdered.Items().First();
        var productId = item.Product.definition.id;
        ProductBought(productId);
    }

    private void OnPurchaseFailed(FailedOrder failedOrder)
    {
        IntitializeAdmob.Instance.ShowBanner();
        Debug.Log($"Purchase failed: {failedOrder}");
    }

    private void OnPurchaseDeferred(DeferredOrder deferredOrder)
    {
        Debug.LogWarning($"Purchase deferred: {deferredOrder.Info.TransactionID}");
    }

    public string GetPrice(string productId)
    {
        if (!IsInitialized) return string.Empty;

        var product = _storeController.GetProductById(productId);
        return product != null ? product.metadata.localizedPriceString : string.Empty;
    }

    #region Rewards

    private void ProductBought(string product)
    {
        switch (product)
        {
            case nameof(InAppProduct.InAppProductType.remove_ads):
                {
                    print("bought");
                    PlayerPrefs.SetInt("RemoveAds", 1);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                break;

            case "coins_500":
                AddCoins(500);
                break;

            case "coins_1200":
                AddCoins(1200);
                break;

            case "coins_2500":
                AddCoins(2500);
                break;

            case "coins_5000":
                AddCoins(5000);
                break;

            case "coins_10000":
                AddCoins(10000);
                break;

            case "coins_15000":
                AddCoins(15000);
                break;

            case "coins_25000":
                AddCoins(25000);
                break;

            case "coins_50000":
                AddCoins(50000);
                break;
            default:
                Debug.LogWarning("Unknown product: " + product);
                break;
        }
        Debug.Log("Reward Granted for: " + product);
    }
    private void AddCoins(int amount)
    {
        //int coins = PlayerPrefs.GetInt("Coins", 0);
        CoinUI.instance.AddCoinWithEffect(amount);
        //coins += amount;

        //PlayerPrefs.SetInt("Coins", coins);
        //PlayerPrefs.Save();

        Debug.Log("Coins Added: " + amount);
    }
    #endregion
}
