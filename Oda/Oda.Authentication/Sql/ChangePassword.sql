/* Oda.Authentication : ChangePassword.sql. */
if exists(select 0 from Accounts with (nolock) where Nonce = @OldNonce and DigestPassword = @DigestPassword and AccountId = @AccountId) begin;
	update Accounts 
	set 
	DigestPassword = @NewDigestPassword,
	Nonce = @NewNonce 
	where AccountId = @AccountId and DigestPassword = @DigestPassword;
	/* return */
	select 0, 'Password updated';
end;
else begin;
	/* return */
	select 1, 'Old password is incorrect.';
end;