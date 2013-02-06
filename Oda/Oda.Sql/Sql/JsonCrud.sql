-- this is the original 'clean' version of this SP.  This file should never be called, but is only here for reference.
create procedure [dbo].[JsonCrud] 
	@objName varchar(50), 
	@record_from int, 
	@record_to int, 
	@suffix varchar(max), 
	@accountId uniqueidentifier, 
	@searchSuffix varchar(max), 
	@aggregateColumns varchar(max),
	@selectedRowsCSV varchar(max),
	@includeSchema bit,
	@checksum bigint,
	@delete bit,
	@orderBy_override varchar(50),
	@orderDirection_override varchar(4)
as
	set XACT_ABORT ON
	set nocount on
	if(LEN(@objName)=0)begin return end
	declare @name varchar(50)
	declare @type varchar(50)
	declare @newChecksum bigint
	declare @max_length int
	declare @objectId int
	declare @colorder int
	declare @colSize int
	declare @orderBy int
	declare @direction bit
	declare @visibility bit
	declare @column_count int = 0
	declare @order_by_name varchar(1000) = '1'
	declare @order_by_direction_name varchar(4) = 'asc'
	declare @header varchar(max) = '';
	declare @schema varchar(max) = '';
	declare @current_pk_length int;
	declare @current_pk_dataType varchar(50);
	declare @sql varchar(max) = '';
	declare @searchCols varchar(max) = '';
	declare @defaultValue varchar(max) = '';
	declare @pramDef int;
	declare @description varchar(max) = ''
	declare @is_nullable bit = 0;
	declare @primary_key bit = 0;
	declare @current_pk_column varchar(max);
	declare @displayName varchar(50);
	declare @hidden bit = 0;
	declare @tableDisplayName sql_variant;
	/* check column sizes */
	declare column_cursor cursor forward_only static for
		select syscolumns.name,
		s.DATA_TYPE,
		length,
		case when objectId is null then cast(syscolumns.id as varchar(50)) else objectId end as objectId,
		case when UIColumns.ColumnOrder is null then syscolumns.colOrder else UIColumns.ColumnOrder end as colOrder,
		case when size is null then 200 else size end as size,
		case when orderby is null then 0 else orderby end as orderby,
		case when direction is null then 0 else direction end as direction,
		case when s.is_nullable is null then 0 else case when s.is_nullable = 'YES' then 1 else 0 end end as is_nullable,
		case when c.CONSTRAINT_NAME is null then 0 else 1 end as primary_key,
		case when m.text is null then '' else m.text end as defaultValue
		from syscolumns with (NOLOCK)
		left join UIColumns on UIColumnId = cast(cast(syscolumns.id as varchar(50))+'_'+cast(colid as varchar(50)) as varchar(100)) and AccountId = @accountId and objectId = syscolumns.id
		left join information_schema.columns s on s.TABLE_NAME = @objName and s.column_name = syscolumns.name
		left join information_schema.constraint_column_usage u on u.TABLE_NAME = @objName and u.column_name = syscolumns.name and @includeSchema = 1 and substring(u.constraint_name,1,3) = 'PK_'
		left join information_schema.table_constraints c on CONSTRAINT_TYPE = 'PRIMARY KEY' and c.TABLE_NAME = u.TABLE_NAME and @includeSchema = 1
		left join syscomments m on m.id = syscolumns.cdefault and @includeSchema = 1
		where syscolumns.id = object_id(@objName) order by syscolumns.colid
	OPEN column_cursor;
	FETCH NEXT FROM column_cursor
	INTO @name, @type, @max_length, @objectId, @colorder, @colSize, @orderBy, @direction, @is_nullable, @primary_key, @defaultValue
	WHILE @@FETCH_STATUS = 0
	BEGIN
		if (@primary_key=1) begin
			set @current_pk_column = @name;
			set @current_pk_dataType = @type;
			set @current_pk_length = @max_length;
		end
		if @type in ('text','varchar','nchar','char','ntext','nvarchar','sql_variant')  begin
			set @sql+='''"''+cast(case when ['+@name+'] is null then '''' else replace(replace(['+@name+'],''\'',''\\''),''"'',''\"'') end as varchar(max))+''",''+';
		end else if @type = 'timestamp' begin 
			set @sql+='''"''+cast(cast(['+@name+'] as int) as varchar(max))+''",''+';
		end else if @type = 'uniqueidentifier'  begin
			set @sql+='''"''+cast(case when ['+@name+'] is null then ''00000000-0000-0000-0000-000000000000'' else replace(replace(['+@name+'],''\'',''\\''),''"'',''\"'') end as varchar(36))+''",''+';
		end else if @type = 'datetime'  begin
			set @sql+='''"''+case when ['+@name+'] is null then '''' else convert(varchar,['+@name+'],126) end+''",''+';
		end else begin
			set @sql+='''"''+cast(case when ['+@name+'] is null then '''' else replace(replace(['+@name+'],''\'',''\\''),''"'',''\"'') end as varchar(max))+''",''+';
		end
		set @searchCols+='['+@name+'],'
		set @header+='{"name":"'+replace(@name,'"','\"')+'","dataType":"'+@type+'",
		"dataLength":'+cast(@max_length as varchar(50))+',
		"columnOrder":'+cast(@column_count as varchar(50))+',
		"columnSize":'+cast(@colSize as varchar(50))+',
		"isNullable":'+cast(@is_nullable as varchar(50))+',
		"primaryKey":'+cast(@primary_key as varchar(50))+',
		"defaultValue":"'+replace(cast(@defaultValue as varchar(50)),'"','\"')+'"
		},';
		if @orderBy = @column_count begin
			set @order_by_name = @name
		end
		if @direction=1 begin
			set @order_by_direction_name = 'desc';
		end
		if len(@orderBy_override) > 0 begin
			set @order_by_name = @orderBy_override
		end
		if len(@orderDirection_override) > 0 begin
			set @order_by_direction_name = @orderDirection_override
		end
		set @column_count=@column_count+1;
		FETCH NEXT FROM column_cursor
		INTO @name, @type, @max_length, @objectId, @colorder, @colSize, @orderBy, @direction, @is_nullable, @primary_key, @defaultValue
	END
	CLOSE column_cursor;
	DEALLOCATE column_cursor;
	declare @count table(row_count int);
	declare @tableChecksum table(tableChecksum bigint);
	if(@includeSchema=1) begin
		insert into @count exec ('select cast(count(1) as varchar(50)) from ['+@objName+'] '+@suffix);
	end else begin
		insert into @count select -1
	end
	insert into @tableChecksum exec ('select sum(cast(BINARY_CHECKSUM(VerCol) as bigint)) from ['+@objName+']'); /* checksum looks w/o suffix */
	set @newChecksum = (select tableChecksum from @tableChecksum)
	set @sql = SUBSTRING(@sql,0,len(@sql)-2)
	declare @newTableChecksum bigint = (select CAST(tableChecksum as varchar(100)) from @tableChecksum)
	set @schema = '{
	"error":0,
	"description":"",
	"objectId":'+cast(@objectId as varchar(50))+',
	"columns":'+CAST(@column_count as varchar(50))+',
	"records":'+(select cast(row_count as varchar(50)) from @count)+',
	"orderBy":'+cast(@orderBy as varchar(50))+',
	"orderByDirection":'+cast(@direction as varchar(50))+',
	"checksum":'+cast(case when @newTableChecksum is null then -1 else @newTableChecksum end as varchar(100))+',
	"name":"'+@objName+'"}';
	set @header = SUBSTRING(@header,1,len(@header)-1)
	set @searchCols = SUBSTRING(@searchCols,1,len(@searchCols)-1)
	if (@searchSuffix = '' and @aggregateColumns = '') begin
		set @sql = '
		select '''+@schema+''' as JSON,-2 as ROW_NUMBER,NULL as PK,NULL as PK_DATATYPE,0 as PK_DATALENGTH
		union all
		select ''['+case when @includeSchema = 1 then @header else '' end+']'' as JSON,-1 as ROW_NUMBER,NULL as PK,NULL as PK_DATATYPE,0 as PK_DATALENGTH
		union all 
		select JSON,ROW_NUMBER,PK,NULL as PK_DATATYPE,0 as PK_DATALENGTH
		from (
			select ''[''+' + @sql + ']'' as JSON,
			row_number() over(order by ['+@order_by_name+'] '+@order_by_direction_name+') as ROW_NUMBER,'+
			case when len(@current_pk_column) > 0 then
				' ['+@current_pk_column+'] as PK,
				'''+@current_pk_dataType+''' as PK_DATATYPE, '''+cast(@current_pk_length as varchar(50))+''' as PK_DATALENGTH'
			else
				' NULL as PK,NULL as PK_DATATYPE,0 as PK_DATALENGTH '
			end+
		' from ['+@objName+'] with (NOLOCK) '+@suffix+') J where ' +
		case when @delete = 1 and len(@selectedRowsCSV) > 0 then
			'ROW_NUMBER in ('+@selectedRowsCSV+')'
		else
			'ROW_NUMBER between @Rfrom  and @Rto '
		end +' group by JSON,ROW_NUMBER,PK,PK_DATATYPE,PK_DATALENGTH order by ROW_NUMBER'
		
		if (@newChecksum = @checksum and @delete = 1 and len(@selectedRowsCSV) > 0) begin 
			print 'DELETE!'
			declare @jsonTable table(json varchar(max),row_number int, pk sql_variant,pk_datType varchar(50), pk_dataLength int)
			declare @prep nchar(1000) = 'EXEC sp_executesql @Rsql'
			insert into @jsonTable exec sp_executesql @prep,N'@Rsql ntext',@Rsql = @sql
			declare @csvPKList varchar(max) = '';
			select * from @jsonTable
			SELECT @csvPKList=COALESCE(@csvPKList,'')+''''+CONVERT(varchar(max),pk)+''',' from @jsonTable where ROW_NUMBER > 0
			set @csvPKList = SUBSTRING(@csvPKList,1,len(@csvPKList)-1)
			print @csvPKList;
			declare @deleteStatement varchar(max) = 'delete from ['+@objName+'] where ['+@current_pk_column+'] in ('+@csvPKList+')';
			set @schema = replace(@schema,'"description":""','"description":"One or more records deleted - JSON = deleted recordset"')
		end else if @delete = 1 begin 
			print 'DELETE FAIL => CHECKSUM MISMATCH!'
			set @schema = replace(@schema,'"error":0','"error":-1')
			set @schema = replace(@schema,'"description":""','"description":"Table version checksum error / table has changed since last update - delete aborted."')
		end
	end else if (@aggregateColumns = '') begin/* search for a row number, don't output any data */
		set @sql = '
		select ROW_NUMBER
		from (
			select '+@searchCols+',
			row_number() over(order by ['+@order_by_name+'] '+@order_by_direction_name+') as ROW_NUMBER
			from '+@objName+' with (NOLOCK) '+@suffix+'
		) J '+@searchSuffix+' group by '+@searchCols+',ROW_NUMBER order by ROW_NUMBER
		'
	end else begin/* produce a list of aggregates (within @selectedRowsCSV if @selectedRowsCSV has data)*/
		declare @aggResults table(columnName varchar(50), result varchar(50));
		declare @splitDest table(val nvarchar(max));
		declare @aggDest table(col nvarchar(100),agg varchar(100));
		declare @colAgg table(col nvarchar(100),agg nvarchar(50));
		declare @select varchar(max);
		declare @group varchar(max);
		declare @aggWhere varchar(max) = '';
		set @SQL = '';
		insert into @splitDest select * from dbo.SPLIT(@aggregateColumns,',',0,0);
		insert into @colAgg select substring(val,0,CHARINDEX('|',val,0)) as col, substring(val,CHARINDEX('|',val,0)+1,999) as agg from @splitDest
		if(len(@selectedRowsCSV)>0) begin
			set @aggWhere = 'where ROW_NUMBER in ('+@selectedRowsCSV+')';
		end
		SELECT @SQL=COALESCE(@SQL,'')+CAST(' select ''{"name":"'+col+'","aggregateFunction":"'+agg+'","aggregateResult":''+cast('+agg+'(['+col+']) as varchar(100))+''}'' as JSON from (
			select ['+col+'] from 
			(select ['+col+'] from
				(select
					['+col+'],
					row_number() over(order by ['+@order_by_name+'] '+@order_by_direction_name+') as ROW_NUMBER
					from '+@objName+' with (NOLOCK) '+@suffix+'
				) J '+@aggWhere+' group by ['+col+'],ROW_NUMBER
			) F ) g union all ' AS VARCHAR(MAX))
		from @colAgg;
		if(len(@sql)>0) begin
			set @SQL = substring(@sql,0,len(@sql)-9)/*remove the last union all<space> command*/
		end
	end
	BEGIN TRY
		/* sp_executesql wrapper used to pass (and cache) very large query to sp_executesql */
		declare @prep2 nchar(1000) = 'EXEC sp_executesql @Rsql,N''@Rfrom int,@Rto int'',@Rfrom='+cast(@record_from as varchar(50))+',@Rto='+cast(@record_to as varchar(50))+''
		exec sp_executesql @prep2,N'@Rsql ntext',@Rsql = @sql
	END TRY
	BEGIN CATCH
		if(ERROR_NUMBER()=1204 or ERROR_NUMBER()=1205)begin
			/* if there was a deadlock then try again after the timeout */
			execute dbo.toJson
			@objName, 
			@record_from, 
			@record_to, 
			@suffix, 
			@accountId, 
			@searchSuffix, 
			@aggregateColumns,
			@selectedRowsCSV,
			@includeSchema,
			@checksum,
			@delete,
			@orderBy_override,
			@orderDirection_override
		end else begin
			print @sql;
			declare @ERROR_NUMBER int = ERROR_NUMBER();
			declare @ERROR_MESSAGE varchar(255) = ERROR_MESSAGE();
			RAISERROR (@ERROR_NUMBER,
			16, -- Severity,
			1, -- State,
			@ERROR_MESSAGE);
		end
	END CATCH;
	if (@delete = 1 and len(@deleteStatement) > 0) begin
		/* execute delete statement after the select statment */
		print @deleteStatement;
		exec sp_executesql @prep,N'@Rsql ntext',@Rsql = @deleteStatement;
	end
	set nocount off