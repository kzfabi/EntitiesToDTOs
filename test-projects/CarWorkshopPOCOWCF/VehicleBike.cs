//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace CarWorkshopPOCOWCF
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(VehicleBikeNormal))]
    [KnownType(typeof(VehicleBikeSidecar))]
    public partial class VehicleBike : Vehicle
    {
        #region Primitive Properties
        [DataMember]
        public virtual int Passengers
        {
            get;
            set;
        }

        #endregion
        #region Complex Properties
        [DataMember]
        public virtual MyComplexT ComplexProperty
        {
            get { return _complexProperty; }
            set { _complexProperty = value; }
        }
        private MyComplexT _complexProperty = new MyComplexT();

        #endregion
    }
}
