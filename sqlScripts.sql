use [9katthe]
SELECT userid,RoleId,u.UserName,r.Name,ur.TenantId  FROM [AspNetUserRoles] ur left join AspNetUsers u on ur.UserId=u.Id left join AspNetRoles r on ur.RoleId=r.Id
SELECT *  FROM [Tenants]
--SELECT *   FROM [AspNetRoles]
--SELECT *  FROM [AspNetUserRoles]
--select * from [AspNetUsers]

--update [AspNetUsers]
--set TenantId='b7384612-5768-445a-bf62-aeddd039cf9a' where id='b76c06e9-a767-4824-835f-8b4e86b52c90'

--select * FROM [AspNetRoleClaims]
--select * from [AspNetUserClaims]