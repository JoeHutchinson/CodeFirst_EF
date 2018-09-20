namespace CodeFirst_EF.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WordMetrics",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        //Word = c.String(nullable: false),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            // manually add encrypyed columns
            Sql(@"AlTER TABLE [dbo].[WordMetrics] ADD [Word] [nvarchar](max) COLLATE Latin1_General_BIN2 ENCRYPTED 
                WITH (COLUMN_ENCRYPTION_KEY = [CEK_Auto1], ENCRYPTION_TYPE = Deterministic, ALGORITHM = 'AEAD_AES_256_CBC_HMAC_SHA_256') NOT NULL");
        }
        
        public override void Down()
        {
            DropTable("dbo.WordMetrics");
        }
    }
}
