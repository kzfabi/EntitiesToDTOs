using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

namespace CarWorkshopCodeFirst
{
    public partial class CarWorkshopContainer : DbContext, ITestInterface, ITestInterfaceOther
    {
        public CarWorkshopContainer()
            : this(false) { }

        public CarWorkshopContainer(bool proxyCreationEnabled)
            : base("name=CarWorkshopContainer")
        {
            this.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
        }

        public CarWorkshopContainer(string connectionString)
            : this(connectionString, false) { }

        public CarWorkshopContainer(string connectionString, bool proxyCreationEnabled)
            : base(connectionString)
        {
            this.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
        }

        public ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public DbSet<Employee<object>> Employees { get; set; }
        public DbSet<EmployeeType> EmployeeTypes { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }



        // Test interfaces

        public void DoSomething()
        {
            // Nothing here
        }

        public void DoOtherThing(int num)
        {
            // Nothing here
        }

    }
}
