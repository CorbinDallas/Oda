if not exists(select 0 from systypes where name = 'ParameterList') begin
	CREATE TYPE ParameterList AS TABLE(
		Name varchar(100) NOT NULL,
		Type varchar(100) NOT NULL,
		Length varchar(5) NOT NULL,
		Value sql_variant NOT NULL
	)
end