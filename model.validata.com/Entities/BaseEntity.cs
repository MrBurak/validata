

using model.validata.com.Enumeration;
using util.validata.com;

namespace model.validata.com.Entities
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

        public virtual void MarkAsDeleted()
        {
            if (DeletedOn == null)
            {
                DeletedOn = DateTimeUtil.SystemTime;
                LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                OperationSourceId = (int)BusinessOperationSource.Api;
            }
        }
    }
}