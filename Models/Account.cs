//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShoeStore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
