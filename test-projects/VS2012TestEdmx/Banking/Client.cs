//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VS2012TestEdmx.Banking
{
    using System;
    using System.Collections.Generic;
    
    public partial class Client
    {
        public Client()
        {
            this.Accounts = new HashSet<Account>();
        }
    
        public int ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public VS2012TestEdmx.Enums.ClientType TypeOfClient { get; set; }
    
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
