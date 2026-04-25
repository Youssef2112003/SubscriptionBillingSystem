namespace SPS.Shared.Domain
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        Guid? DeletedBy { get; set; }
        DateTime? DeletedDate { get; set; }
    }
}
