using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    public partial class MyComplexT
    {
        [DataMember]
        public bool PropBool { get; set; }
        [DataMember]
        public int PropInt { get; set; }
    }
    
}
