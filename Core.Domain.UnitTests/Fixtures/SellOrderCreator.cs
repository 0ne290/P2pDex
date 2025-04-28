using System.Reflection;
using Core.Domain.Constants;
using Core.Domain.Entities;

namespace Core.Domain.UnitTests.Fixtures;

public class SellOrderCreator
{
    public SellOrderCreator()
    {
        Guid = Guid.Parse("68586aff-0af9-4a2d-aeab-17e276bc9c1b");
        Status = OrderStatus.Created;
        Crypto = Cryptocurrency.Ethereum;
        CryptoAmount = 0.0076m;
        Fiat = FiatCurrency.Ruble;
        CryptoToFiatExchangeRate = 201894;
        FiatAmount = CryptoAmount * CryptoToFiatExchangeRate;
        PaymentMethodInfo = "Test";
        SellerToExchangerFee = 0.00000042m;
        ExchangerToMinersFee = 0.000000000064m;
        SellerId = 12345;
        SellerToExchangerTransferTransactionHash =
            "0xb5c8bd9430b6cc87a0e2fe110ece6bf527fa4f170a4bc8cd032f768fc5219838";
        ExchangerToBuyerTransferTransactionHash = null;
        BuyerId = null;
        BuyerAccountAddress = null;
    }

    public SellOrder CreateExpected()
    {
        var sellOrderType = typeof(SellOrder);
        var baseOrderType = typeof(BaseOrder);
        var baseEntityType = typeof(BaseEntity);
        const BindingFlags invokeAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty |
                                        BindingFlags.Instance;
        var sellOrder = (SellOrder)Activator.CreateInstance(sellOrderType,true)!;

        baseEntityType.InvokeMember("Guid", invokeAttr, null, sellOrder, [Guid]);
        baseOrderType.InvokeMember("Status", invokeAttr, null, sellOrder, [Status]);
        baseOrderType.InvokeMember("Crypto", invokeAttr, null, sellOrder, [Crypto]);
        baseOrderType.InvokeMember("CryptoAmount", invokeAttr, null, sellOrder, [CryptoAmount]);
        baseOrderType.InvokeMember("Fiat", invokeAttr, null, sellOrder, [Fiat]);
        baseOrderType.InvokeMember("CryptoToFiatExchangeRate", invokeAttr, null, sellOrder, [CryptoToFiatExchangeRate]);
        baseOrderType.InvokeMember("FiatAmount", invokeAttr, null, sellOrder, [FiatAmount]);
        baseOrderType.InvokeMember("PaymentMethodInfo", invokeAttr, null, sellOrder, [PaymentMethodInfo]);
        baseOrderType.InvokeMember("SellerToExchangerFee", invokeAttr, null, sellOrder, [SellerToExchangerFee]);
        baseOrderType.InvokeMember("ExchangerToMinersFee", invokeAttr, null, sellOrder, [ExchangerToMinersFee]);
        sellOrderType.InvokeMember("SellerId", invokeAttr, null, sellOrder, [SellerId]);
        sellOrderType.InvokeMember("SellerToExchangerTransferTransactionHash", invokeAttr, null, sellOrder, [SellerToExchangerTransferTransactionHash]);
        baseOrderType.InvokeMember("ExchangerToBuyerTransferTransactionHash", invokeAttr, null, sellOrder, [ExchangerToBuyerTransferTransactionHash]);
        sellOrderType.InvokeMember("BuyerId", invokeAttr, null, sellOrder, [BuyerId]);
        sellOrderType.InvokeMember("BuyerAccountAddress", invokeAttr, null, sellOrder, [BuyerAccountAddress]);
        
        return sellOrder;
    }
    
    // По дефолту все данные валидны и заказ должен создаваться без исключений
    public SellOrder CreateActual()
    {
        return new SellOrder(Guid, Crypto, CryptoAmount, Fiat, CryptoToFiatExchangeRate, PaymentMethodInfo,
            SellerToExchangerFee, ExchangerToMinersFee, SellerId, SellerToExchangerTransferTransactionHash);
    }

    public Guid Guid { get; set; }

    public OrderStatus Status { get; set; }
    
    public Cryptocurrency Crypto { get; set; }
    
    public decimal CryptoAmount { get; set; }
    
    public FiatCurrency Fiat { get; set; }
    
    public decimal CryptoToFiatExchangeRate { get; set; }
    
    public decimal FiatAmount { get; set; }
    
    public string PaymentMethodInfo { get; set; }
    
    public decimal SellerToExchangerFee { get; set; }
    
    public decimal ExchangerToMinersFee { get; set; }
    
    public int SellerId { get; set; }
    
    public string SellerToExchangerTransferTransactionHash { get; set; }
    
    public string? ExchangerToBuyerTransferTransactionHash { get; set; }
    
    public long? BuyerId { get; set; }
    
    public string? BuyerAccountAddress { get; set; }
}