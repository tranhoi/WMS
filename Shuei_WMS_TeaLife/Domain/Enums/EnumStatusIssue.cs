namespace Domain.Enums
{
    public enum EnumStatusIssue
    {
        None = 0,
        OnOrder = 1, // Warehouse Shipment
        Picked = 2,
        Packing = 3,
        Deliveried = 4,
        Cancelled = 5
    }
}
