if not exists(select 0 from sys.tables where name = 'UIColumns' and type = 'U') begin
    CREATE TABLE [dbo].[UIColumns](
	    [UIColumnId] [varchar](100) NOT NULL,
	    [AccountId] [uniqueidentifier] NOT NULL,
	    [ObjectId] [int] NOT NULL,
	    [ColumnOrder] [int] NOT NULL,
	    [Size] [int] NOT NULL,
	    [OrderBy] [int] NULL,
	    [Direction] [char](5) NULL,
	    [Visibility] [bit] NULL,
	    [VerCol] [timestamp] NOT NULL,
     CONSTRAINT [PK_UIColumns] PRIMARY KEY CLUSTERED 
    (
	    [UIColumnId] ASC
    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
    ) ON [PRIMARY]
end