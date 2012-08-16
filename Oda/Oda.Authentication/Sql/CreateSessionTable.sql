/* Oda.Authentication : CreateSessionTable.sql. */
if not exists(select 0 from sys.tables where name = 'Sessions' and type = 'U') begin
	CREATE TABLE Sessions
	(
		SessionId uniqueidentifier NOT NULL,
		Referer varchar(1000) NOT NULL,
		UserAgent varchar(1000) NOT NULL,
		IpAddress varchar(15) NOT NULL,
		AccountId uniqueidentifier NOT NULL,
		VerCol timestamp NOT NULL,
		CONSTRAINT PK_Sessions PRIMARY KEY CLUSTERED ( SessionId ASC )
		WITH 
		(
			PAD_INDEX = OFF, 
			STATISTICS_NORECOMPUTE = OFF, 
			IGNORE_DUP_KEY= OFF, 
			ALLOW_ROW_LOCKS = ON, 
			ALLOW_PAGE_LOCKS = ON, 
			FILLFACTOR = 85
		) ON [PRIMARY]
	)
	ON [PRIMARY]
end;
if not exists(select 0 from sys.tables where name = 'SessionProperties' and type = 'U') begin
	CREATE TABLE SessionProperties
	(
		SessionPropertyId uniqueidentifier NOT NULL,
		AccountId uniqueidentifier NOT NULL,
		SessionId uniqueidentifier NOT NULL,
		Name varchar(50) NOT NULL,
		Value varchar(max) NOT NULL,
		DataType varchar(25) NOT NULL,
		VerCol timestamp NOT NULL,
		CONSTRAINT PK_SessionProperties PRIMARY KEY CLUSTERED ( SessionPropertyId ASC )
		WITH 
		(
			PAD_INDEX = OFF,
			STATISTICS_NORECOMPUTE = OFF,
			IGNORE_DUP_KEY = OFF,
			ALLOW_ROW_LOCKS = ON,
			ALLOW_PAGE_LOCKS = ON,
			FILLFACTOR = 80
		) ON [PRIMARY]
	) 
	ON [PRIMARY]
end;
if not exists(select 0 from sys.tables where name = 'Accounts' and type = 'U') begin
	CREATE TABLE Accounts (
		AccountId uniqueidentifier NOT NULL,
		Logon varchar(50) NOT NULL,
		DigestPassword varchar(100) NOT NULL,
		Nonce varchar(100) NOT NULL,
		VerCol timestamp NOT NULL,
		CONSTRAINT PK_Accounts PRIMARY KEY CLUSTERED ( AccountId ASC )
		WITH 
		(
			PAD_INDEX = OFF,
			STATISTICS_NORECOMPUTE = OFF,
			IGNORE_DUP_KEY = OFF,
			ALLOW_ROW_LOCKS = ON,
			ALLOW_PAGE_LOCKS = ON,
			FILLFACTOR = 99
		) ON [PRIMARY]
	) 
	ON [PRIMARY]
end;
if not exists(select 0 from sys.tables where name = 'Contacts' and type = 'U') begin
	CREATE TABLE Contacts(
		ContactId uniqueidentifier NOT NULL,
		AccountId uniqueidentifier NOT NULL,
		First varchar(50) NOT NULL,
		Middle varchar(50) NOT NULL,
		Last varchar(50) NOT NULL,
		Address varchar(50) NOT NULL,
		Address2 varchar(50) NOT NULL,
		City varchar(50) NOT NULL,
		State varchar(50) NOT NULL,
		Zip varchar(15) NOT NULL,
		Email varchar(50) NOT NULL,
		Company varchar(50) NOT NULL,
		Title varchar(50) NOT NULL,
		WebAddress varchar(50) NOT NULL,
		IMAddress varchar(50) NOT NULL,
		Fax varchar(25) NOT NULL,
		Home varchar(25) NOT NULL,
		Work varchar(25) NOT NULL,
		Mobile varchar(25) NOT NULL,
		Notes varchar(max) NOT NULL,
		Type int NOT NULL,
		VerCol timestamp NOT NULL,
		CONSTRAINT PK_Contacts PRIMARY KEY CLUSTERED (ContactId ASC)
		WITH (
			PAD_INDEX = OFF,
			STATISTICS_NORECOMPUTE = OFF,
			IGNORE_DUP_KEY = OFF,
			ALLOW_ROW_LOCKS = ON,
			ALLOW_PAGE_LOCKS = ON
		) ON [PRIMARY]
	) ON [PRIMARY]
end;