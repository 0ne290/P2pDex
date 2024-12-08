using Core.Domain.Enums;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class Dispute : BaseEntity
{
    public Dispute(Guid guid, SellOrder sellOrder, Trader trader) : base(guid)
    {
        Status = DisputeStatus.WaitingForAdministrator;
        SellOrder = sellOrder;
        Administrator = null;
        
        sellOrder.Deny(trader);
    }

    public void AssignAdministrator(Administrator administrator)
    {
        if (Status != DisputeStatus.WaitingForAdministrator)
            throw new InvariantViolationException("CurrentStatus is invalid.");

        Status = DisputeStatus.Active;
        Administrator = administrator;
    }
    
    public void ResolveInFavorOfBuyer(string exchangerToBuyerTransferTransactionHash)
    {
        if (Status != DisputeStatus.Active)
            throw new InvariantViolationException("CurrentStatus is invalid.");

        Status = DisputeStatus.ResolvedInFavorOfBuyer;
        Administrator!.IncrementDisputeResolved();
        SellOrder.Buyer!.IncrementWonDisputesAsBuyer();
        SellOrder.Seller!.IncrementLostDisputesAsSeller();
        SellOrder.Complete(exchangerToBuyerTransferTransactionHash);
    }
    
    public void ResolveInFavorOfSeller(string exchangerToBuyerTransferTransactionHash)
    {
        if (Status != DisputeStatus.Active)
            throw new InvariantViolationException("CurrentStatus is invalid.");

        Status = DisputeStatus.ResolvedInFavorOfSeller;
        Administrator!.IncrementDisputeResolved();
        SellOrder.Buyer!.IncrementLostDisputesAsBuyer();
        SellOrder.Seller!.IncrementWonDisputesAsSeller();
        SellOrder.Complete(exchangerToBuyerTransferTransactionHash);
    }
    
    public DisputeStatus Status { get; private set; }
    
    public SellOrder SellOrder { get; }
    
    public Administrator? Administrator { get; private set; }
}
