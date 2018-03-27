using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ContosoUniversity.DAL
{
    public class SchoolContext : DbContext 
    {
        /*The name of the connection string is passed in to the constructor.
         In this case the connection string is called SchoolContext. 
         Alternatively, you can pass in the complete connection string.
         
         If you don't specify a connection string or the name of one explicitly, 
         Entity Framework assumes that the connection string name is the same
         as the class name
        public SchoolContext() : base("SchoolContext")
        {
        }*/


        /*You need to create a DBSet<EntityType> for each Entity in your 
         models folder*/
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<Scholarship> Scholarships { get; set; }
        public DbSet<Organization> Organizations { get; set; }


        /*If you don't remove the PluralizingTableNameConvention in the 
         OnModelCreating method, when the Model is created, the generated 
         tables in the DB would be named Students, Enrollments, and Courses. 
         Tables should not have plural names, so this is how that is configured*/
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Course>()
             .HasMany(c => c.Instructors).WithMany(i => i.Courses)
             .Map(t => t.MapLeftKey("CourseID")
                 .MapRightKey("InstructorID")
                 .ToTable("CourseInstructor"));
            modelBuilder.Entity<Department>().MapToStoredProcedures();
        }
    }
}