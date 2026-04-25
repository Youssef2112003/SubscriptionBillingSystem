namespace SPS.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    Guid? DeletedBy { get; set; }
    DateTime? DeletedDate { get; set; }
}