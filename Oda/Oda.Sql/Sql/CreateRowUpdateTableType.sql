if not exists(select 0 from systypes where name = 'RowUpdate') begin
	CREATE TYPE RowUpdate AS TABLE(
		KeyName varchar(100) NOT NULL,
		KeyValue sql_variant NOT NULL,
		Primary_key bit NOT NULL,
		DataType varchar(50) NOT NULL,
		DataLength int NOT NULL,
		VarCharMaxValue varchar(max) NULL
	)
end