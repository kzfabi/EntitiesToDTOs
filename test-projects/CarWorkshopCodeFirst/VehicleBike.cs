using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(VehicleBikeNormal))]
    [KnownType(typeof(VehicleBikeSidecar))]
    public partial class VehicleBike : Vehicle
    {
        public VehicleBike()
        {
            this.ComplexProperty = new MyComplexT();
        }
    
        [DataMember]
        public int Passengers { get; set; }
    
        [DataMember]
        public MyComplexT ComplexProperty { get; set; }
    }
    
}
