create function ParameterTypeToDeclaration(
	@name varchar(100),
	@type varchar(100),
	@length varchar(10),
	@value sql_variant
) returns varchar(max)
as 
begin
	return('declare ' + @name + ' ' + @type + case when len(@length) > 0 then '(' + @length + ')' else '' end + '; set ' + @name + ' = ''' + convert(varchar(max),@value) + ''';');
end