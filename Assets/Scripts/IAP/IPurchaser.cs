using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using SimpleJSON;
using GIKCore;
using GIKCore.Net;
using GIKCore.UI;
public struct IPurchaserProduct
{
    public Product product;
    public string storeSpecificId;
    public string localizedDescription;
    public string localizedPriceString;
    public string localizedTitle;
}
public class IPurchaser : MonoBehaviour, IStoreListener
{
    // Values
    public IStoreController storeController { get; private set; } // The Unity Purchasing system.
    private IExtensionProvider storeExtensionProvider; // The store-specific Purchasing subsystems.

    public bool isPurchaseInProgress { get; private set; } = false;

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.   

    public const string SKU_ONE_TIME_OFFER = "com.mytheriaccg.io.firsttime";
    public const string SKU_STARTER = "com.mytheriaccg.io.starter";
    public const string SKU_100_GEM = "com.mytheriaccg.io.100";
    public const string SKU_300_GEM = "com.mytheriaccg.io.300";
    public const string SKU_600_GEM = "com.mytheriaccg.io.600";
    public const string SKU_1600_GEM = "com.mytheriaccg.io.1600";
    public const string SKU_4000_GEM = "com.mytheriaccg.io.4000";
    public const string SKU_8200_GEM = "com.mytheriaccg.io.8200";
    public const string SKU_18000_GEM = "com.mytheriaccg.io.18000";


    private static Dictionary<string, ProductType> dictProduct = new Dictionary<string, ProductType>()
    {
        {SKU_ONE_TIME_OFFER, ProductType.NonConsumable},
        {SKU_STARTER, ProductType.NonConsumable},
        {SKU_100_GEM, ProductType.Consumable},
        {SKU_300_GEM, ProductType.Consumable},
        {SKU_600_GEM, ProductType.Consumable},
        {SKU_1600_GEM, ProductType.Consumable},
        {SKU_4000_GEM, ProductType.Consumable},
        {SKU_8200_GEM, ProductType.Consumable},
        {SKU_18000_GEM, ProductType.Consumable}        
    };

    // Methods
    public IPurchaserProduct GetProduct(string skuId)
    {
        IPurchaserProduct ret = new IPurchaserProduct();

        if (isInitialized)
        {
            Product p = storeController.products.WithStoreSpecificID(skuId);
            if (p != null)
            {
                ret.product = p;
                ret.storeSpecificId = p.definition.storeSpecificId;
                ret.localizedDescription = p.metadata.localizedDescription;
                ret.localizedPriceString = p.metadata.localizedPriceString;
                ret.localizedTitle = p.metadata.localizedTitle;
            }
        }

        return ret;
    }
    public List<IPurchaserProduct> GetAllProduct()
    {
        List<IPurchaserProduct> ret = new List<IPurchaserProduct>();
        if (isInitialized)
        {
            foreach (Product p in storeController.products.all)
            {
                ret.Add(new IPurchaserProduct()
                {
                    product = p,
                    storeSpecificId = p.definition.storeSpecificId,
                    localizedDescription = p.metadata.localizedDescription,
                    localizedPriceString = p.metadata.localizedPriceString
                });
            }
        }
        return ret;
    }
    private void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (isInitialized)
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        foreach (KeyValuePair<string, ProductType> pair in dictProduct)
        {
            builder.AddProduct(pair.Key, pair.Value);
        }

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    // Only say we are initialized if both the Purchasing references are set.
    public bool isInitialized { get { return storeController != null && storeExtensionProvider != null; } }

    public void BuyProduct(Product product)
    {
        // If Purchasing has been initialized ...
        if (isInitialized)
        {
            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("IAP Purchasing product asychronously: '{0}'", product.definition.id));
                Game.main.netBlock.ShowNow();
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                isPurchaseInProgress = true;
                storeController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("IAP BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase.");
                Toast.Show("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase.", 4f);
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("IAP BuyProductID FAIL. Not initialized.");
        }
    }

    public void BuyProduct(string productId)
    {
        Product product = null;

        if (isInitialized)
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            product = storeController.products.WithID(productId);
        }

        BuyProduct(product);
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!isInitialized)
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("IAP RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("IAP RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            IAppleExtensions apple = storeExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("IAP RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                if (result)
                {
                    // This does not mean anything was restored,
                    // merely that the restoration process succeeded.
                }
                else
                {
                    // Restoration failed.
                }
            });

        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("IAP RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    // IStoreListener
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("IAP OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        storeController = controller;        
        // Store specific subsystem, for accessing device-specific store features.
        storeExtensionProvider = extensions;

        HandleNetData.QueueNetData(NetData.IAP_INIT_PASS);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("IAP OnInitializeFailed InitializationFailureReason: " + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("IAP OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        isPurchaseInProgress = false;
        Game.main.netBlock.Hide();
        HandleNetData.QueueNetData(NetData.IAP_PURCHASE_FAIL, product.definition.id);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        isPurchaseInProgress = false;
        string productId = e.purchasedProduct.definition.id;
        bool success = false;
        Game.main.netBlock.Hide();

        foreach (KeyValuePair<string, ProductType> pair in dictProduct)
        {
            if (String.Equals(productId, pair.Key, StringComparison.Ordinal))
            { // A product has been purchased by this user.
                //System.IO.File.WriteAllText(Application.persistentDataPath + "/FotunaIAPLog.txt", e.purchasedProduct.receipt);
                Debug.Log(string.Format("IAP Process Purchase: PASS. Product: '{0}'", productId));
                string token = "";
#if UNITY_ANDROID
                JSONNode root = JSON.Parse(e.purchasedProduct.receipt);
                JSONNode payload = JSON.Parse(root["Payload"].Value);
                JSONNode json = JSON.Parse(payload["json"].Value);
                token = json["purchaseToken"].Value;
#elif UNITY_IOS
                JSONNode N = JSON.Parse(e.purchasedProduct.receipt);
                token = N["Payload"].Value;
#endif
                // TODO: The consumable item has been successfully purchased.
                success = true;
                HandleNetData.QueueNetData(NetData.IAP_PURCHASE_SUCCESS, new string[] { productId, token });
                break;
            }
        }

        if (!success)
        {//An unknown product has been purchased by this user. Fill in additional products here....
            Debug.Log(string.Format("IAP Process Purchase: FAIL. Unrecognized product: '{0}'", productId));
            HandleNetData.QueueNetData(NetData.IAP_PURCHASE_FAIL, productId);
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    // Use this for initialization
    void Awake()
    {
        Game.main.IAP = this;
#if (UNITY_ANDROID || UNITY_IOS)
        // If we haven't set up the Unity Purchasing reference
        if (storeController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
#endif
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}
