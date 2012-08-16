/* Oda.Authentication : CreateAccount.sql. */
declare @AccountId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
if not exists(select 0 from Accounts with (nolock) where Logon = @Logon) begin;
	set @AccountId = NewId();
	insert into Accounts (AccountId, Logon, DigestPassword, Nonce) values (@AccountId, @Logon, @DigestPassword, @Nonce);
	select @AccountId,'Account created.';
end;
else begin;
	select @AccountId,'Account already exists.';
end;
