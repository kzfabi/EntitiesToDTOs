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
    public partial class EmployeePainter : EmployeeType
    {
        #region Primitive Properties
        [DataMember]
        public virtual bool IsIndependent
        {
            get;
            set;
        }

        #endregion
    }
}
