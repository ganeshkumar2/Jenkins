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
    
    public partial class Limit
    {
        public decimal Id { get; set; }
        public decimal Status { get; set; }
        public System.DateTime OperationDts { get; set; }
        public string LimitId { get; set; }
        public string LimitName { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string AlertValue { get; set; }
        public string Description { get; set; }
    }
}
