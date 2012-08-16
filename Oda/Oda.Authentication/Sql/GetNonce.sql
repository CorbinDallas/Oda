/* Oda.Authentication : GetNonce.sql. */
select isnull(Nonce,'') from Accounts with (nolock) where Logon = @Logon;