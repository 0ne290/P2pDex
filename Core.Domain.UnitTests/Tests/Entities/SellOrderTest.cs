using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.UnitTests.Fixtures;

namespace Core.Domain.UnitTests.Tests.Entities;

public class SellOrderTest
{
    private static void SellOrdersAreEquals(SellOrder expected, SellOrder actual)
    {
        Assert.Equal(expected.Guid, actual.Guid);
        Assert.Equal(expected.Status, actual.Status);
        Assert.Equal(expected.Crypto, actual.Crypto);
        Assert.Equal(expected.CryptoAmount, actual.CryptoAmount);
        Assert.Equal(expected.Fiat, actual.Fiat);
        Assert.Equal(expected.CryptoToFiatExchangeRate, actual.CryptoToFiatExchangeRate);
        Assert.Equal(expected.FiatAmount, actual.FiatAmount);
        Assert.Equal(expected.PaymentMethodInfo, actual.PaymentMethodInfo);
        Assert.Equal(expected.SellerToExchangerFee, actual.SellerToExchangerFee);
        Assert.Equal(expected.ExchangerToMinersFee, actual.ExchangerToMinersFee);
        Assert.Equal(expected.SellerId, actual.SellerId);
        Assert.Equal(expected.SellerToExchangerTransferTransactionHash, actual.SellerToExchangerTransferTransactionHash);
        Assert.Equal(expected.ExchangerToBuyerTransferTransactionHash, actual.ExchangerToBuyerTransferTransactionHash);
        Assert.Equal(expected.BuyerId, actual.BuyerId);
        Assert.Equal(expected.BuyerAccountAddress, actual.BuyerAccountAddress);
    }
    
    [Fact]
    public void New_Success()
    {
        // Arrange
        var sellOrderCreator = new SellOrderCreator();
        var expected = sellOrderCreator.CreateExpected();
        
        // Act
        var actual = sellOrderCreator.CreateActual();

        // Assert
        SellOrdersAreEquals(expected, actual);
    }

    [Fact]
    public void ConfirmSellerToExchangerTransferTransaction_Success()
    {
        // Arrange
        var sellOrderCreator = new SellOrderCreator();
        var actual = sellOrderCreator.CreateActual();
        sellOrderCreator.Status = OrderStatus.SellerToExchangerTransferTransactionConfirmed;
        var expected = sellOrderCreator.CreateExpected();

        // Act
        actual.ConfirmSellerToExchangerTransferTransaction();

        // Assert
        SellOrdersAreEquals(expected, actual);
    }
    
    [Fact]
    public void RespondByBuyer_Success()
    {
        // Arrange
        var sellOrderCreator = new SellOrderCreator();
        var actual = sellOrderCreator.CreateActual();
        sellOrderCreator.BuyerId = 1337;
        sellOrderCreator.BuyerAccountAddress = "0x1D1479C185d32EB90533a08b36B3CFa5F84A0E6B";
        sellOrderCreator.Status = OrderStatus.RespondedByBuyer;
        var expected = sellOrderCreator.CreateExpected();

        // Act
        actual.ConfirmSellerToExchangerTransferTransaction();
        actual.RespondByBuyer(sellOrderCreator.BuyerId.Value, sellOrderCreator.BuyerAccountAddress);

        // Assert
        SellOrdersAreEquals(expected, actual);
    }
    
    [Fact]
    public void ConfirmTransferFiatToSellerByBuyer_Success()
    {
        // Arrange
        var sellOrderCreator = new SellOrderCreator();
        var actual = sellOrderCreator.CreateActual();
        sellOrderCreator.BuyerId = 1337;
        sellOrderCreator.BuyerAccountAddress = "0x1D1479C185d32EB90533a08b36B3CFa5F84A0E6B";
        sellOrderCreator.Status = OrderStatus.TransferFiatToSellerConfirmedByBuyer;
        var expected = sellOrderCreator.CreateExpected();

        // Act
        actual.ConfirmSellerToExchangerTransferTransaction();
        actual.RespondByBuyer(sellOrderCreator.BuyerId.Value, sellOrderCreator.BuyerAccountAddress);
        actual.ConfirmTransferFiatToSellerByBuyer();

        // Assert
        SellOrdersAreEquals(expected, actual);
    }
    
    [Fact]
    public void ConfirmReceiptFiatFromBuyerBySeller_Success()
    {
        // Arrange
        var sellOrderCreator = new SellOrderCreator();
        var actual = sellOrderCreator.CreateActual();
        sellOrderCreator.BuyerId = 1337;
        sellOrderCreator.BuyerAccountAddress = "0x1D1479C185d32EB90533a08b36B3CFa5F84A0E6B";
        sellOrderCreator.ExchangerToBuyerTransferTransactionHash =
            "0xb0dab6cfa2fc2ae9b2b1d39fbc9a84846dd6ecae17f2d86785f9897dbdf6ca46";
        sellOrderCreator.Status = OrderStatus.ReceiptFiatFromBuyerConfirmedBySeller;
        var expected = sellOrderCreator.CreateExpected();

        // Act
        actual.ConfirmSellerToExchangerTransferTransaction();
        actual.RespondByBuyer(sellOrderCreator.BuyerId.Value, sellOrderCreator.BuyerAccountAddress);
        actual.ConfirmTransferFiatToSellerByBuyer();
        actual.ConfirmReceiptFiatFromBuyerBySeller(sellOrderCreator.ExchangerToBuyerTransferTransactionHash);

        // Assert
        SellOrdersAreEquals(expected, actual);
    }
    
    [Fact]
    public void ConfirmExchangerToBuyerTransferTransaction_Success()
    {
        // Arrange
        var sellOrderCreator = new SellOrderCreator();
        var actual = sellOrderCreator.CreateActual();
        sellOrderCreator.BuyerId = 1337;
        sellOrderCreator.BuyerAccountAddress = "0x1D1479C185d32EB90533a08b36B3CFa5F84A0E6B";
        sellOrderCreator.ExchangerToBuyerTransferTransactionHash =
            "0xb0dab6cfa2fc2ae9b2b1d39fbc9a84846dd6ecae17f2d86785f9897dbdf6ca46";
        sellOrderCreator.Status = OrderStatus.ExchangerToBuyerTransferTransactionConfirmed;
        var expected = sellOrderCreator.CreateExpected();

        // Act
        actual.ConfirmSellerToExchangerTransferTransaction();
        actual.RespondByBuyer(sellOrderCreator.BuyerId.Value, sellOrderCreator.BuyerAccountAddress);
        actual.ConfirmTransferFiatToSellerByBuyer();
        actual.ConfirmReceiptFiatFromBuyerBySeller(sellOrderCreator.ExchangerToBuyerTransferTransactionHash);
        actual.ConfirmExchangerToBuyerTransferTransaction();

        // Assert
        SellOrdersAreEquals(expected, actual);
    }
}