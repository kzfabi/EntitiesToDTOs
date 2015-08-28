using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CarWorkshopCodeFirst
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(EmployeeType))]
    public partial class Employee<T> : List<Vehicle>
    {

        public List<T> GenericPropList { get; set; }

        [DataMember]
        public Dictionary<List<long>, bool?> Dict { get; set; }

        [DataMember]
        public int[] ArrayNumbers { get; set; }

        [DataMember]
        public string[] ArrayTexts { get; set; }

        [DataMember]
        public Type[] ArrayTypes { get; set; }

        [DataMember]
        public Vehicle[] ArrayVehicles { get; set; }

        [DataMember]
        public Vehicle Vehicle { get; set; }

        [DataMember]
        public List<string[]> CollectionArray { get; set; }

        [DataMember]
        public List<IEnumerable<Type[]>> NestedCollectionArray { get; set; }

        [DataMember]
        public int EmployeeID { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public double? Salary { get; set; }

        [DataMember]
        public Nullable<System.DateTime> Birthdate { get; set; }

        [DataMember]
        public string Event { get; set; }
    
        [DataMember]
        public virtual EmployeeType EmployeeType { get; set; }

        [DataMember]
        public List<Vehicle> Vehicles { get; set; }

        [DataMember]
        public List<long> SomeNumbers { get; set; }

        [DataMember]
        public IEnumerable<string> SomeTexts { get; set; }

        [DataMember]
        public HashSet<object> SomeObjects { get; set; }

        [DataMember]
        public List<IEnumerable<HashSet<EmployeeType>>> NestedCollections { get; set; }

        public T GenericProp { get; set; }

        internal int Number { get; set; }

        private string Text { get; set; }

        protected bool Flag { get; set; }

    }
}