namespace SPS.Shared.Domain
{
    public abstract class BaseEntity<TKey> : IEntity<TKey>, IAuditableEntity, ISoftDeletable
    {
        public TKey Id { get; set; } = default!;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (!(obj is BaseEntity<TKey>)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (GetType() != obj.GetType()) return false;
            BaseEntity<TKey> item = (BaseEntity<TKey>)obj;
            if (EqualityComparer<TKey>.Default.Equals(item.Id, default) || EqualityComparer<TKey>.Default.Equals(Id, default))
            {
                return false;
            }
            else
            {
                return EqualityComparer<TKey>.Default.Equals(item.Id, Id);
            }
        }

        public override int GetHashCode()
        {
            if (!EqualityComparer<TKey>.Default.Equals(Id, default))
                return Id.GetHashCode() ^ 31;
            return base.GetHashCode();
        }

        public static bool operator ==(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null) ? true : false;
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
        {
            return !(left == right);
        }

    }
    public interface IEntity<TId>
    {
        TId Id { get; set; }
    }





}
