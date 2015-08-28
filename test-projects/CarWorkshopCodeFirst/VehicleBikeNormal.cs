using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    public partial class VehicleBikeNormal : VehicleBike
    {
        [DataMember]
        public string TestProp { get; set; }
    }
    
}
