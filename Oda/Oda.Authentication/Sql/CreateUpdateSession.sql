/* Oda.Authentication : CreateUpdateSession.sql. */
declare @EmptyGuid uniqueidentifier = '00000000-0000-0000-0000-000000000000';
declare @AccountId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
if not exists(select 0 from Sessions with (nolock) where SessionId = @SessionId) begin;
    insert into Sessions
        (SessionId, Referer, UserAgent, IpAddress, AccountId) 
    values 
        (@SessionId, @Referer, @UserAgent, @IpAddress, @AccountId);
end;
/* Oda.Session : Select session. */
select 
	@AccountId = AccountId 
from Sessions with (nolock)
where SessionId = @SessionId
if (@AccountId is null) begin;
	set @AccountId = @EmptyGuid;
end;
select @AccountId;
/* Oda.Session : Update properties when logged on */
if not @AccountId = @EmptyGuid begin;
	/* update existing properties with account Id */
	update SessionProperties set 
		AccountId = @AccountId
	where SessionId = @SessionId;
	/* update existing contacts with account Id */
	update Contacts set
		AccountId = @AccountId
	where AccountId = @SessionId;
end;
/* Oda.Session : Select SessionProperties.* */
select 
	SessionPropertyId, 
	Name, 
	Value, 
	DataType
from SessionProperties with (nolock)
where 
	SessionId = @SessionId 
	or AccountId = @AccountId
/* Oda.Session : Select Contacts. */
select
	ContactId,
	AccountId,
	First,
	Middle,
	Last,
	Address,
	Address2,
	City,
	State,
	Zip,
	Email,
	Company,
	Title,
	WebAddress,
	IMAddress,
	Fax,
	Home,
	Work,
	Mobile,
	Notes,
	Type
from Contacts with (nolock)
where
	ContactId = @AccountId
	or AccountId = @AccountId
	or AccountId = @SessionId;