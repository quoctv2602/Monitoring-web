namespace Monitoring_HistoryArchive.App_Code
{
	public class SQLStatements
	{
		public static string MoveDataTableHistory = @"
           DECLARE @TbQueryCreateColummExists AS TABLE ([Index] int IDENTITY(1,1) PRIMARY KEY,TABLE_NAME VARCHAR(128),COLUMN_NAME VARCHAR(128),DATA_TYPE VARCHAR(128),  QUERY NVARCHAR(MAX))

			DECLARE @tableCheck AS TABLE ( [Index] int IDENTITY(1,1) PRIMARY KEY, TABLE_NAME VARCHAR(125))
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Config Table backup - Start'
			INSERT INTO @tableCheck
			(
				TABLE_NAME
			)
			VALUES 
			('Trans_Data_Health'),
			('Trans_System_Health'),
			('Trans_Message_Log'),
			('Trans_System_Health_Instance'),
			('Trans_System_Health_Storage'),
			('TransactionBase'),
			('ListErrorBase')

			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Config Table backup - End'
 
			DECLARE @tableName VARCHAR(128)
			DECLARE @cnt INT = 0;
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Render query create column - Start'
			WHILE @cnt < (SELECT COUNT(1) FROM @tableCheck)
			BEGIN
	
				SELECT @tableName= t.TABLE_NAME FROM @tableCheck AS t WHERE t.[Index] = @cnt + 1
	
				INSERT INTO @TbQueryCreateColummExists (TABLE_NAME, COLUMN_NAME, DATA_TYPE, [QUERY])
				SELECT  'history.' +c.TABLE_NAME +'_Archive',c.COLUMN_NAME,
				c.DATA_TYPE + IIF(c.CHARACTER_MAXIMUM_LENGTH IS NULL,'',IIF(c.CHARACTER_MAXIMUM_LENGTH = -1,'(max)','('+CONVERT(VARCHAR(12),c.CHARACTER_MAXIMUM_LENGTH )+')') ),
				 '
				 ALTER TABLE history.' +@tableName+'_Archive ADD ['+COLUMN_NAME+'] '+c.DATA_TYPE + IIF(c.CHARACTER_MAXIMUM_LENGTH IS NULL,'',IIF(c.CHARACTER_MAXIMUM_LENGTH = -1,'(max)','('+CONVERT(VARCHAR(12),c.CHARACTER_MAXIMUM_LENGTH )+')') ) 'Query'
				FROM INFORMATION_SCHEMA.COLUMNS AS c
				OUTER APPLY (
					SELECT b.TABLE_NAME ,b.COLUMN_NAME COLUMN_NAME_NotExists
					FROM INFORMATION_SCHEMA.COLUMNS AS b 
					WHERE TABLE_NAME =  @tableName +'_Archive' AND c.COLUMN_NAME = b.COLUMN_NAME 
				) b
				WHERE c.TABLE_NAME = @tableName
				AND b.COLUMN_NAME_NotExists IS NULL
			   SET @cnt = @cnt + 1;
			END;
			SET @cnt = 0
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Render query create column - End'
 


			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Exec query create column - Start'
			WHILE @cnt < (SELECT COUNT(1) FROM @TbQueryCreateColummExists)
			BEGIN
				DECLARE @query NVARCHAR(max)
				SELECT @query= t.[QUERY] FROM @TbQueryCreateColummExists AS t WHERE t.[Index] = @cnt + 1
	
				PRINT @query
				EXEC (@query)
	
				SET @cnt = @cnt +1
			end
			SET @cnt = 0
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Exec query create column - End'


			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Exec query move data to table bk - Start'
			WHILE @cnt < (SELECT COUNT(1) FROM @tableCheck)
			BEGIN
	 
				SELECT @tableName= t.TABLE_NAME FROM @tableCheck AS t WHERE t.[Index] = @cnt + 1
	
 
				DECLARE @query_column NVARCHAR(MAX)
				DECLARE @query_insert_data NVARCHAR(MAX)

				SELECT TOP 1 @query_column =  STUFF((
					SELECT ',[' + c.COLUMN_NAME+']'
					FROM INFORMATION_SCHEMA.COLUMNS AS c WHERE c.TABLE_NAME = @tableName ORDER BY c.COLUMN_NAME
					FOR XML PATH('')
					), 1, 1, '')  
				FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName

				SET @query_insert_data = '
				INSERT INTO history.'+@tableName+'_Archive ('+@query_column+')
				SELECT '+@query_column+' 
				FROM '+@tableName+' as r
				WHERE  '+IIF(@tableName = 'Trans_System_Health_Storage','CreateDate','CreatedDate')	+' < DATEADD(DAY,-{0},CURRENT_TIMESTAMP)
	
				DELETE FROM '+@tableName+' 
				WHERE  '+IIF(@tableName = 'Trans_System_Health_Storage','CreateDate','CreatedDate')	+' < DATEADD(DAY,-{0},CURRENT_TIMESTAMP)
				'
			 
				PRINT @query_insert_data
				EXEC (@query_insert_data)
				SET @cnt = @cnt +1
			end
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Exec query move data to table bk - End'
        ";
	}
}
