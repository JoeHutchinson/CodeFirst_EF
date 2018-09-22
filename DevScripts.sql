USE CountVonCountDB

SELECT * FROM dbo.TmpWordMetrics twm 
select * FROM dbo.WordMetrics wm

SELECT * FROM dbo.TmpWordMetrics twm WHERE twm.Word = '04c772ee-e'
select * FROM dbo.WordMetrics wm WHERE wm.Word = '04c772ee-e'

TRUNCATE TABLE dbo.TmpWordMetrics
TRUNCATE TABLE dbo.WordMetrics

TRUNCATE TABLE dbo.__MigrationHistory
DROP TABLE dbo.WordMetrics
DROP TABLE  dbo.TmpWordMetrics

IF EXISTS (
  SELECT * 
    FROM sysobjects 
   WHERE name = N'p_MergeIntoWordMetrics' 
	 AND type = 'P'
)
  DROP PROCEDURE [dbo].[p_MergeIntoWordMetrics]
GO

SELECT * FROM dbo.__MigrationHistory mh

