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
    
    public partial class GetProducts_Result
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Nullable<double> ProductPrice { get; set; }
        public Nullable<int> ProductViewer { get; set; }
        public Nullable<int> ProductCategory { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> ColorSum { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageExtension { get; set; }
    }
}
