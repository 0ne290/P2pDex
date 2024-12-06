using Core.Domain.Enums;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class Dispute : BaseEntity
{
    public Dispute(Guid guid, SellOrder sellOrder) : base(guid)
    {
        Status = DisputeStatus.WaitingForAdministrator;
        SellOrder = sellOrder;
        OrderGuid = sellOrder.Guid;
        Administrator = null;
        AdministratorGuid = null;
    }

    public void AssignAdministrator(Administrator administrator)
    {
        if (Status != DisputeStatus.WaitingForAdministrator)
            throw new InvariantViolationException("CurrentStatus is invalid.");

        Status = DisputeStatus.Active;
        Administrator = administrator;
        AdministratorGuid = administrator.Guid;
    }
    
    public void ResolveInFavorOfBuyer()
    {
        if (Status != DisputeStatus.Active)
            throw new InvariantViolationException("CurrentStatus is invalid.");

        Status = DisputeStatus.ResolvedInFavorOfBuyer;
        Administrator!.IncrementDisputeResolved();
        SellOrder.Buyer!.IncrementWonDisputesAsBuyer();
        SellOrder.Seller!.IncrementLostDisputesAsSeller();
        SellOrder.Complete();
    }
    
    public void ResolveInFavorOfSeller()
    {
        if (Status != DisputeStatus.Active)
            throw new InvariantViolationException("CurrentStatus is invalid.");

        Status = DisputeStatus.ResolvedInFavorOfSeller;
        Administrator!.IncrementDisputeResolved();
        SellOrder.Buyer!.IncrementLostDisputesAsBuyer();
        SellOrder.Seller!.IncrementWonDisputesAsSeller();
        SellOrder.Complete();
    }
    
    public DisputeStatus Status { get; private set; }
    
    public SellOrder SellOrder { get; }
    
    public Guid OrderGuid { get; }
    
    public Administrator? Administrator { get; private set; }
    
    public Guid? AdministratorGuid { get; private set; }
}
