using Core.Domain.Constants;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class Dispute : BaseEntity
{
    public Dispute(Guid guid, Guid orderGuid) : base(guid)
    {
        Status = DisputeStatus.WaitingForAdministrator;
        OrderGuid = orderGuid;
        AdministratorGuid = null;
        
        //sellOrder.DenyBySellerReceiptFiatFromBuyer(trader);
    }

    public void AssignAdministrator(Guid administratorGuid)
    {
        if (Status != DisputeStatus.WaitingForAdministrator)
            throw new InvariantViolationException("Status is invalid.");

        Status = DisputeStatus.Active;
        AdministratorGuid = administratorGuid;
    }
    
    public void ResolveInFavorOfBuyer(string exchangerToBuyerTransferTransactionHash)
    {
        if (Status != DisputeStatus.Active)
            throw new InvariantViolationException("CurrentStatus is invalid.");

        Status = DisputeStatus.ResolvedInFavorOfBuyer;
        //Administrator!.IncrementDisputeResolved();
        //SellOrder.Buyer!.IncrementWonDisputesAsBuyer();
        //SellOrder.Seller!.IncrementLostDisputesAsSeller();
        //SellOrder.Complete(exchangerToBuyerTransferTransactionHash);
    }
    
    public void ResolveInFavorOfSeller(string exchangerToBuyerTransferTransactionHash)
    {
        if (Status != DisputeStatus.Active)
            throw new InvariantViolationException("CurrentStatus is invalid.");

        Status = DisputeStatus.ResolvedInFavorOfSeller;
        //Administrator!.IncrementDisputeResolved();
        //SellOrder.Buyer!.IncrementLostDisputesAsBuyer();
        //SellOrder.Seller!.IncrementWonDisputesAsSeller();
        //SellOrder.Complete(exchangerToBuyerTransferTransactionHash);
    }
    
    public DisputeStatus Status { get; private set; }
    
    public Guid OrderGuid { get; }
    
    public Guid? AdministratorGuid { get; private set; }
}
