//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TDSAirlines
{
    using System;
    using System.Collections.Generic;
    
    public partial class Flight
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Flight()
        {
            this.Bookings = new HashSet<Booking>();
        }
    
        public string FlightID { get; set; }
        public string DepartureAirportID { get; set; }
        public string ArrivalAirportID { get; set; }
        public System.DateTime DepartureDate { get; set; }
        public decimal AvailiableSeats { get; set; }
    
        public virtual Airport Airport { get; set; }
        public virtual Airport Airport1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}