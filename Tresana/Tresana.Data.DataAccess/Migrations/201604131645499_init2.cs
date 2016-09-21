namespace Tresana.Data.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.Tasks", "Priority_Id", "dbo.Priorities");
            DropForeignKey("dbo.Tasks", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.Users", "Team_Id", "dbo.Teams");
            DropIndex("dbo.Users", new[] { "Project_Id" });
            DropIndex("dbo.Users", new[] { "Team_Id" });
            DropIndex("dbo.Tasks", new[] { "Priority_Id" });
            DropIndex("dbo.Tasks", new[] { "Status_Id" });
            AddColumn("dbo.Tasks", "Priority", c => c.Int(nullable: false));
            AddColumn("dbo.Tasks", "Status", c => c.String());
            DropColumn("dbo.Users", "Project_Id");
            DropColumn("dbo.Users", "Team_Id");
            DropColumn("dbo.Tasks", "Priority_Id");
            DropColumn("dbo.Tasks", "Status_Id");
            DropTable("dbo.Priorities");
            DropTable("dbo.Projects");
            DropTable("dbo.Status");
            DropTable("dbo.Teams");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Ordinal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        Duedate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Priorities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Tasks", "Status_Id", c => c.Int());
            AddColumn("dbo.Tasks", "Priority_Id", c => c.Int());
            AddColumn("dbo.Users", "Team_Id", c => c.Int());
            AddColumn("dbo.Users", "Project_Id", c => c.Int());
            DropColumn("dbo.Tasks", "Status");
            DropColumn("dbo.Tasks", "Priority");
            CreateIndex("dbo.Tasks", "Status_Id");
            CreateIndex("dbo.Tasks", "Priority_Id");
            CreateIndex("dbo.Users", "Team_Id");
            CreateIndex("dbo.Users", "Project_Id");
            AddForeignKey("dbo.Users", "Team_Id", "dbo.Teams", "Id");
            AddForeignKey("dbo.Tasks", "Status_Id", "dbo.Status", "Id");
            AddForeignKey("dbo.Tasks", "Priority_Id", "dbo.Priorities", "Id");
            AddForeignKey("dbo.Users", "Project_Id", "dbo.Projects", "Id");
        }
    }
}
