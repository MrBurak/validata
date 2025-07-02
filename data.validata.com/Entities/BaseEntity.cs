using util.validata.com;

namespace data.validata.com.Entities
{
    public class BaseEntity
    {
        public const string PeriodStartShadowProperty = "PeriodStart";
        public const string PeriodEndShadowProperty = "PeriodEnd";

        public DateTime CreatedOnTimeStamp { get; set; } = DateTimeUtil.SystemTime;
        public DateTime LastModifiedTimeStamp { get; set; } = DateTimeUtil.SystemTime;
        public DateTime? DeletedOn { get; set; }
        public int? OperationSourceId { get; set; }
        public virtual OperationSource? OperationSource { get; set; }
    }
}