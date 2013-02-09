-- this is the original 'clean' version of this SP.  This file should never be called, but is only here for reference.
create function RowUpdateTableToUpdateString(
	@value sql_variant,
	@dataType varchar(50),
	@dataLength int,
	@varCharMaxValue varchar(max)
) returns varchar(max)
as 
begin
	declare @return varchar(max);
	if @dataType in ('int','money','float','real','bigint','datetime','bit','uniqueidentifier','timestamp') begin
		set @return = 'convert('+@dataType+','''+convert(varchar(max),@value)+'''),';
	end else if @dataType in ('varchar') begin
		set @return = 'convert('+@dataType+case when @dataLength > 0 then '(' + cast(@dataLength as varchar(50)) + ')' else '(max)' end + ','''+replace(convert(varchar(max),@varCharMaxValue),'''','''''')+'''),';
	end else if @dataType in ('nvarchar','ntext','char','nchar') begin
		set @return = 'convert('+@dataType+'(' + cast(@dataLength as varchar(50)) + ')'+ ','''+replace(convert(varchar(max),@varCharMaxValue),'''','''''')+'''),';
	end;
	return(@return);
end