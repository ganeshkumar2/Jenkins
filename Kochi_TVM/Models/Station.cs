//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kochi_TVM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Station
    {
        public decimal Id { get; set; }
        public decimal Status { get; set; }
        public System.DateTime OperationDts { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public Nullable<decimal> Distance { get; set; }
        public string RegionCode { get; set; }
        public Nullable<decimal> RegionDistance { get; set; }
        public string Direction { get; set; }
        public string GpsLatitude { get; set; }
        public string GpsLongitude { get; set; }
        public Nullable<decimal> Order { get; set; }
    }
}