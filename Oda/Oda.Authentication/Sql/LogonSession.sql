/* Oda.Authentication : LogonSession.sql. */
declare @EmptyGuid uniqueidentifier = '00000000-0000-0000-0000-000000000000';
if exists(select 0 from Accounts with (nolock) where Logon = @Logon and DigestPassword = @DigestPassword) begin;
	declare @AccountId uniqueidentifier = (select AccountId from Accounts where Logon = @Logon and DigestPassword = @DigestPassword);
	update Sessions set AccountId = @AccountId where SessionId = @SessionId;
	/* update SessionProperties acquired before logging on */ 
	update SessionProperties set 
		AccountId = @AccountId
	where SessionId = @SessionId;
	/* update contacts setting account Id on any contacts that are sessionId based */
	update Contacts set
		AccountId = @AccountId
	where AccountId = @SessionId;
	/* return AccountId and a nifty message */
	select @AccountId,'Logged On';
end;
else begin;
	select @EmptyGuid,'Logon does not exist or password is incorrect.';
end;