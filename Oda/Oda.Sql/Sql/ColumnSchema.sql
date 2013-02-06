select cast(c.id as varchar(50)) + '_' + cast(colid as varchar(50)) as uid, colid, c.name, '' as Title, '' as [ctype],
[length], colorder as [Order], 0 as width, 0 as hidden, isnullable,
isnull((select 1 from information_schema.key_column_usage where column_name = c.name and table_name = o.name and OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1),0) as PrimaryKey,
'' as [Default], '' as Description, '' as help, c.type, c.id as ParentId, o.name as TableName
from syscolumns c with (nolock)
inner join sysobjects o with (nolock) on c.id = o.id;
