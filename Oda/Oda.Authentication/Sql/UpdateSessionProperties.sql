/* Oda.Authentication : UpdateSessionProperties.sql. */
if not exists(select 0 from SessionProperties with (nolock) where SessionPropertyId = @SessionPropertyId) begin;
	insert into SessionProperties 
		(SessionPropertyId, AccountId, SessionId, Name, Value, DataType)
	values
		(@SessionPropertyId, @AccountId, @SessionId, @Name, @Value, @DataType)
end;
else begin;
	update SessionProperties
	set
		AccountId = @AccountId,
		SessionId = @SessionId,
		Name = @Name,
		Value = @Value,
		DataType = @DataType
	where SessionPropertyId = @SessionPropertyId;
end;