/* Oda.Authentication : LogoffSession.sql. */
declare @EmptyGuid uniqueidentifier = '00000000-0000-0000-0000-000000000000';
update Sessions set AccountId = @EmptyGuid where SessionId = @SessionId;