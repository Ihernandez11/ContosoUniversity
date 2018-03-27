using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{

    public enum Grade
    {
        A, B, C, D, F
    }


    public class Enrollment
    {


        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        //The question mark after the type indicates that it's nullable
        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }


        /*Course is a navigation property for the corresponding foreign key
         CourseID*/
        public virtual Course Course { get; set; }
        /*The StudentID property is the foreign key, and the corresponding
         navigation property is Student. 
         
         So if a entity relationship is 1 to Many, like in this case we have
         one Student per Enrollment, so the navigation property can only hold
         a single Student entity. In the case of the Enrollments navigation 
         property in Student - there can be multiple Enrollments per student
         so the navigation property has to be a collection
         
         Entity Framework interprets a property as a foreign key property
         if it's named "Navigation property name" + "Primary key property name"*/
        public virtual Student Student { get; set; }


    }
}