using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(VehicleCar))]
    [KnownType(typeof(VehicleTruck))]
    [KnownType(typeof(VehicleBike))]
    [KnownType(typeof(VehicleBikeNormal))]
    [KnownType(typeof(VehicleBikeSidecar))]
    public partial class Vehicle
    {
        [DataMember]
        public int VehicleID { get; set; }
    }
    
}
