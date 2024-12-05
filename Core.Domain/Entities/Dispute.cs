using Core.Domain.Enums;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class Dispute : EntityBase
{
    public Dispute(Guid guid, Order order) : base(guid)
    {
        Status = DisputeStatus.WaitingForAdministrator;
        Order = order;
        OrderGuid = order.Guid;
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
        Order.Buyer!.IncrementWonDisputesAsBuyer();
        Order.Seller!.IncrementLostDisputesAsSeller();
        Order.Complete();
    }
    
    public void ResolveInFavorOfSeller()
    {
        if (Status != DisputeStatus.Active)
            throw new InvariantViolationException("CurrentStatus is invalid.");

        Status = DisputeStatus.ResolvedInFavorOfSeller;
        Administrator!.IncrementDisputeResolved();
        Order.Buyer!.IncrementLostDisputesAsBuyer();
        Order.Seller!.IncrementWonDisputesAsSeller();
        Order.Complete();
    }
    
    public DisputeStatus Status { get; private set; }
    
    public Order Order { get; }
    
    public Guid OrderGuid { get; }
    
    public Administrator? Administrator { get; private set; }
    
    public Guid? AdministratorGuid { get; private set; }
}
