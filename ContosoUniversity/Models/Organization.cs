using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public class Organization
    {

        public int OrganizationID { get; set; }
        public string Name { get; set; }
        public int StudentID { get; set; }


        //create virtual property for foreign key
        public virtual Student Student { get; set; }

    }
}