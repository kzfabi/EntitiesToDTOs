using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    public partial class VehicleBikeSidecar : VehicleBike
    {
        [DataMember]
        public string AnotherProp { get; set; }
    }
    
}
