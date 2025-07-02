using System.ComponentModel.DataAnnotations;

namespace model.validata.com.Enumeration
{
    public enum BusinessOperationSource
    {
        [Display(Name = "Pre Defined")]
        PreDefined = 1,
        [Display(Name = "Api")]
        Api = 2,
        [Display(Name = "Import")]
        Import = 3
    }
}
