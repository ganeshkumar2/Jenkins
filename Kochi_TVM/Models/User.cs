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
    
    public partial class User
    {
        public int recId { get; set; }
        public string code { get; set; }
        public string alias { get; set; }
        public string fName { get; set; }
        public string sName { get; set; }
        public string lName { get; set; }
        public string password { get; set; }
        public Nullable<System.DateTime> passwordExpireDT { get; set; }
        public Nullable<byte> isActive { get; set; }
        public Nullable<byte> changeOnInitialLogin { get; set; }
        public Nullable<byte> wrongAttempCount { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public Nullable<int> insertUserId { get; set; }
        public System.DateTime insertDT { get; set; }
        public Nullable<int> editUserId { get; set; }
        public System.DateTime editDT { get; set; }
        public bool isDeleted { get; set; }
        public Nullable<int> stationId { get; set; }
        public Nullable<int> roleId { get; set; }
        public string password2 { get; set; }
        public Nullable<int> institutionId { get; set; }
        public Nullable<int> staffId { get; set; }
        public Nullable<bool> isValidForAll { get; set; }
    }
}
