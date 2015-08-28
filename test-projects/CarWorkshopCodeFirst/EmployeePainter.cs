using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    public partial class EmployeePainter : EmployeeType
    {
        [DataMember]
        public bool IsIndependent { get; set; }
    }
    
}
