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
    
    public partial class ErrorCode
    {
        public decimal Id { get; set; }
        public decimal Status { get; set; }
        public System.DateTime OperationDts { get; set; }
        public string Source { get; set; }
        public string ErrorCode1 { get; set; }
        public Nullable<decimal> AlertLevel { get; set; }
        public string ShowInfoMessage { get; set; }
        public string MessageResourceKey { get; set; }
        public string WriteReceiptMessage { get; set; }
        public string ReceiptResourceKey { get; set; }
        public string ErrorDesc { get; set; }
    }
}
