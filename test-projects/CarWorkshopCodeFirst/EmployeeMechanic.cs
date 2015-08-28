using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    public partial class EmployeeMechanic : EmployeeType
    {
        [DataMember]
        public bool NeedsSupervison { get; set; }
    }
    
}
