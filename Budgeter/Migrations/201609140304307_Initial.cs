namespace Budgeter.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Migrations;
    using System.Data.SqlTypes;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE dbo.Accounts DROP COLUMN ReconciledBalance");
            Sql("ALTER TABLE dbo.Accounts ADD ReconciledBalance Decimal SELECT SUM(Amount) FROM [Transactions] WHERE IsReconciled = 1 or IsActive = 1 or IsVoid=0");
            //AddColumn("dbo.Accounts", "ReconciledBalance", c=> c.Decimal(nullable: false, precision: 18, scale: 2),)
            //CreateTable(
            //    "dbo.Accounts",
            //c => new
            //{
            //    Id = c.Int(nullable: false, identity: true),
            //    IsActive = c.Boolean(),
            //    Name = c.String(),
            //    Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
            //    //ReconciledBalance = c.Decimal(),
            //    HouseholdId = c.Int(),
            //})
            //.PrimaryKey(t => t.Id)
            //.ForeignKey("dbo.Households", t => t.HouseholdId);
            //Sql("ALTER TABLE dbo.Accounts ADD ReconciledBalance Decimal AS BEGIN SELECT SUM(Amount) FROM dbo.Transactions WHERE IsReconciled = true or IsActive = true or IsVoid=false");
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.Accounts", "ReconciledBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("dbo.Accounts");
        }
    }
}
