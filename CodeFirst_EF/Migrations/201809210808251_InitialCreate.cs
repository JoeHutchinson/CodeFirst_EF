namespace CodeFirst_EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TmpWordMetrics",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Count = c.Int(nullable: false),
                        //Word = c.String(),
                    })
            .PrimaryKey(t => t.Id);

            // Manually add Always Encrypted Word field to TempWordMetrics table
            Sql(@"AlTER TABLE [dbo].[TmpWordMetrics] ADD [Word] [nvarchar](max) COLLATE Latin1_General_BIN2 ENCRYPTED 
                WITH (COLUMN_ENCRYPTION_KEY = [CEK_Auto1], ENCRYPTION_TYPE = Deterministic, ALGORITHM = 'AEAD_AES_256_CBC_HMAC_SHA_256') NOT NULL");

            CreateTable(
                "dbo.WordMetrics",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Count = c.Int(nullable: false),
                        //Word = c.String(),
                    })
            .PrimaryKey(t => t.Id);

            // Manually add Always Encrypted Word field to TempWordMetrics table
            Sql(@"AlTER TABLE [dbo].[WordMetrics] ADD [Word] [nvarchar](max) COLLATE Latin1_General_BIN2 ENCRYPTED 
                WITH (COLUMN_ENCRYPTION_KEY = [CEK_Auto1], ENCRYPTION_TYPE = Deterministic, ALGORITHM = 'AEAD_AES_256_CBC_HMAC_SHA_256') NOT NULL");

            // Add stored procedure to do a blind upsert into main table from temporary table
            CreateStoredProcedure(
                @"p_MergeIntoWordMetrics", 
                @"SET NOCOUNT ON
	            MERGE WordMetrics AS TARGET
                    USING TmpWordMetrics AS SOURCE 
                    ON (TARGET.Word = SOURCE.Word)
                    WHEN MATCHED THEN
                    UPDATE SET TARGET.Count = TARGET.Count + SOURCE.Count
                    WHEN NOT MATCHED BY TARGET THEN 
                    INSERT (Id, Word, Count)
                    VALUES (SOURCE.Id, SOURCE.Word, SOURCE.Count);
                TRUNCATE TABLE TmpWordMetrics;
                ");
        }
        
        public override void Down()
        {
            DropTable("dbo.WordMetrics");
            DropTable("dbo.TmpWordMetrics");
            DropStoredProcedure("dbo.p_MergeIntoWordMetrics");
        }
    }
}
