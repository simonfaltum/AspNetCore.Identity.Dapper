IF NOT EXISTS ( SELECT  * FROM sys.schemas  WHERE [name] = '$schemaname$' )
EXEC('CREATE SCHEMA [$schemaname$] AUTHORIZATION [dbo]');