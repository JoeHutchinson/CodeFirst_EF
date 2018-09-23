using CodeFirst_EF.Settings;

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
                    Salt = c.String(nullable: true, maxLength: 128)
                    //Word = c.String(nullable: false, maxLength: 128)
                })
            .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.WordMetrics",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Count = c.Int(nullable: false),
                    Salt = c.String(nullable: true, maxLength: 128)
                    //Word = c.String(nullable: false, maxLength: 128)
                })
            .PrimaryKey(t => t.Id);

            if (AppSettings.Get<bool>("EncryptionEnabled"))
            {
                // Manually add Always Encrypted Word field to TempWordMetrics table
                Sql(@"AlTER TABLE [dbo].[TmpWordMetrics] ADD [Word] [nvarchar](max) COLLATE Latin1_General_BIN2 ENCRYPTED 
                WITH (COLUMN_ENCRYPTION_KEY = [CEK_Auto1], ENCRYPTION_TYPE = Deterministic, ALGORITHM = 'AEAD_AES_256_CBC_HMAC_SHA_256') NOT NULL");

                // Manually add Always Encrypted Word field to TempWordMetrics table
                Sql(@"AlTER TABLE [dbo].[WordMetrics] ADD [Word] [nvarchar](max) COLLATE Latin1_General_BIN2 ENCRYPTED 
                WITH (COLUMN_ENCRYPTION_KEY = [CEK_Auto1], ENCRYPTION_TYPE = Deterministic, ALGORITHM = 'AEAD_AES_256_CBC_HMAC_SHA_256') NOT NULL");
            }
            else
            {
                Sql(@"AlTER TABLE [dbo].[TmpWordMetrics] ADD [Word] [nvarchar](max);");
                Sql(@"AlTER TABLE [dbo].[WordMetrics] ADD [Word] [nvarchar](max);");

            }

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
                    INSERT (Id, Word, Count, Salt)
                    VALUES (SOURCE.Id, SOURCE.Word, SOURCE.Count, SOURCE.Salt);
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
