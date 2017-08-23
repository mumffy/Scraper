namespace University.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TphInheritanceForInstructorAndStudentFromPerson : DbMigration
    {
        public override void Up()
        {
            //Student table cannot be dropped while these FKs refer to it
            DropForeignKey("dbo.Enrollment", "StudentID", "dbo.Student");

            RenameTable(name: "dbo.Instructor", newName: "Person");
            AddColumn("dbo.Person", "EnrollmentDate", c => c.DateTime());
            AddColumn("dbo.Person", "Discriminator", c => c.String(nullable: false, maxLength: 128, defaultValue: "Instructor"));
            AlterColumn("dbo.Person", "HireDate", c => c.DateTime());
            AddColumn("dbo.Person", "OldId", c => c.Int(nullable: true)); //just for migrating Enrollment table's FK

            // Copy existing Student data into new Person table.
            Sql("INSERT INTO dbo.Person (LastName, FirstName, HireDate, EnrollmentDate, Discriminator, OldId) SELECT LastName, FirstName, null AS HireDate, EnrollmentDate, 'Student' AS Discriminator, ID AS OldId FROM dbo.Student");

            // Fix up existing relationships to match new PKs.
            Sql("UPDATE dbo.Enrollment SET StudentId = (SELECT ID FROM dbo.Person WHERE OldId = Enrollment.StudentId AND Discriminator = 'Student')");

            // Remove temporary key
            DropColumn("dbo.Person", "OldId");

            DropTable("dbo.Student");

            // Re-create foreign keys and indexes pointing to new table.
            AddForeignKey("dbo.Enrollment", "StudentID", "dbo.Person", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Student",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        EnrollmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AlterColumn("dbo.Person", "HireDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Person", "Discriminator");
            DropColumn("dbo.Person", "EnrollmentDate");
            RenameTable(name: "dbo.Person", newName: "Instructor");
        }
    }
}
