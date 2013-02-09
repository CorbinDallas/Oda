 if not exists(select 0 from sys.objects where name = 'split' and type = 'TF') begin
	 declare @statement nvarchar(max) = 'create function split
(
  @s nvarchar(max),
  @c nvarchar(10) = '','',
  @trimPieces bit = 0,
  @returnEmptyStrings bit = 1
)
returns @t table (val nvarchar(max))
as
begin

declare @i int, @j int
select @i = 0, @j = (len(@s) - len(replace(@s,@c,'''')))

;with cte 
as
(
  select
    i = @i + 1,
    s = @s, 
    n = substring(@s, 0, charindex(@c, @s)),
    m = substring(@s, charindex(@c, @s)+1, len(@s) - charindex(@c, @s))

  union all

  select 
    i = cte.i + 1,
    s = cte.m, 
    n = substring(cte.m, 0, charindex(@c, cte.m)),
    m = substring(
      cte.m,
      charindex(@c, cte.m) + 1,
      len(cte.m)-charindex(@c, cte.m)
    )
  from cte
  where i <= @j
)
insert into @t (val)
select pieces
from 
(
  select 
  case 
    when @trimPieces = 1
    then ltrim(rtrim(case when i <= @j then n else m end))
    else case when i <= @j then n else m end
  end as pieces
  from cte
) t
where
  (@returnEmptyStrings = 0 and len(pieces) > 0)
  or (@returnEmptyStrings = 1)
option (maxrecursion 0)

return

end'
	exec sp_executesql @statement
end