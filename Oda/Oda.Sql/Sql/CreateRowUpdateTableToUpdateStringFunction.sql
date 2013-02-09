 if not exists(select 0 from sys.objects where name = 'RowUpdateTableToUpdateString' and type = 'FN') begin
	 declare @statement nvarchar(max) = 'create function RowUpdateTableToUpdateString(
		@value sql_variant,
		@dataType varchar(50),
		@dataLength int,
		@varCharMaxValue varchar(max)
	) returns varchar(max)
	as 
	begin
		declare @return varchar(max);
		if @dataType in (''int'',''money'',''float'',''real'',''bigint'',''datetime'',''bit'',''uniqueidentifier'',''timestamp'') begin
			set @return = ''convert(''+@dataType+'',''''''+convert(varchar(max),@value)+''''''),'';
		end else if @dataType in (''varchar'') begin
			set @return = ''convert(''+@dataType+case when @dataLength > 0 then ''('' + cast(@dataLength as varchar(50)) + '')'' else ''(max)'' end + '',''''''+replace(convert(varchar(max),@varCharMaxValue),'''''''','''''''''''')+''''''),'';
		end else if @dataType in (''nvarchar'',''ntext'',''char'',''nchar'') begin
			set @return = ''convert(''+@dataType+''('' + cast(@dataLength as varchar(50)) + '')''+ '',''''''+replace(convert(varchar(max),@varCharMaxValue),'''''''','''''''''''')+''''''),'';
		end;
		return(@return);
	end'
	exec sp_executesql @statement
end