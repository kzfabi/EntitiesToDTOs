using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    public partial class VehicleTruck : Vehicle
    {
        [DataMember]
        public bool HasCargo { get; set; }
    }
    
}
