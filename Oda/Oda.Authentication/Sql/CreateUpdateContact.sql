/* Oda.Authentication : CreateUpdateContact.sql. */
if not exists(select 0 from Contacts with (nolock) where ContactId = @ContactId) begin;
	insert into Contacts 
	(ContactId, AccountId, First, Middle, Last, Address, Address2, City, State, 
	Zip, Email, Company, Title, WebAddress, IMAddress, Fax, Home, Work, Mobile, Notes, Type)
	values
	(@ContactId, @AccountId, @First, @Middle, @Last, @Address, @Address2, @City, @State, 
	@Zip, @Email, @Company, @Title, @WebAddress, @IMAddress, @Fax, @Home, @Work, @Mobile, @Notes, @Type);
end;
else begin;
	update Contacts set
	AccountId = @AccountId,
	First = @First,
	Middle = @Middle,
	Last = @Last,
	Address = @Address,
	Address2 = @Address2,
	City = @City,
	State = @State,
	Zip = @Zip,
	Email = @Email,
	Company = @Company,
	Title = @Title,
	WebAddress = @WebAddress,
	IMAddress = @IMAddress,
	Fax = @Fax,
	Home = @Home,
	Work = @Work,
	Mobile = @Mobile,
	Notes = @Notes,
	Type = @Type
	where
	ContactId = @ContactId;
end;