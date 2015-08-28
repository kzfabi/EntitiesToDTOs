using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    public partial class VehicleCar : Vehicle
    {
        [DataMember]
        public bool Is4WD { get; set; }
    }
    
}
