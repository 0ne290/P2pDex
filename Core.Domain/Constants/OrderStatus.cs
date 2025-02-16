using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Domain.Constants;

[JsonConverter(typeof(StringEnumConverter))]
public enum OrderStatus
{
    Created,
    SellerToExchangerTransferTransactionConfirmed,
    RespondedByBuyer,
    RespondedBySeller,
    TransferFiatToSellerConfirmedByBuyer,
    ReceiptFiatFromBuyerConfirmedBySeller,
    //FrozenForDurationOfDispute,
    Cancelled,
    ExchangerToBuyerTransferTransactionConfirmed
}