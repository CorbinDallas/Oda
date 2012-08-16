/* Oda.Authentication : ResetPassword.sql. */
if exists(select 0 from Accounts with (nolock) where Logon = @Logon) begin;
	update Accounts set DigestPassword = @NewDigestPassword where Logon = @Logon;
	select 0, 'Password reset';
end;
else begin;
	select 1, 'Cannot find logon.';
end;