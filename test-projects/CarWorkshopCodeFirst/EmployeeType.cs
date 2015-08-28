using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(EmployeeMechanic))]
    [KnownType(typeof(EmployeePainter))]
    [KnownType(typeof(Employee<object>))]
    public abstract partial class EmployeeType
    {
        public EmployeeType()
        {
            this.Employees = new HashSet<Employee<object>>();
        }
    
        [DataMember]
        public int EmployeeTypeID { get; set; }
    
        [DataMember]
        public virtual ICollection<Employee<object>> Employees { get; set; }
    }
    
}
