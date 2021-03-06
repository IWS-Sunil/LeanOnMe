//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserQuestion
    {
        public long AnswerID { get; set; }
        public long QuestionID { get; set; }
        public Nullable<long> UserID { get; set; }
        public string QuestionText { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Location { get; set; }
        public Nullable<long> LeanID { get; set; }
    
        public virtual AdminQuestion AdminQuestion { get; set; }
        public virtual Registration Registration { get; set; }
        public virtual UserRegistration UserRegistration { get; set; }
    }
}
