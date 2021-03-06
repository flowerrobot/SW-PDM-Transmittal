/****** Script for SelectTopNRows command from SSMS  ******/
--use GekkoVault;
--SELECT u.[UserID]
--      ,[Username]      
--      ,[Email]      
--      ,[FullName]
--      ,[Initials]           
--	  ,gm.GroupID
--	  ,g.Groupname
--  FROM Users u
--  inner join [GroupMembers] gm on gm.UserId = u.UserID 
--  inner join Groups g on g.GroupID = gm.GroupID
--  where [Enabled] = 1
--  and g.Groupname = 'Drafters'
--  or g.Groupname = 'Librarians'
--  order by u.UserID



  SELECT u.[UserID]
      ,[Username]      
      ,[Email]      
      ,[FullName]
      ,[Initials]           
	  ,gm.GroupID
	  ,g.Groupname
  FROM Users u
  inner join [GroupMembers] gm on gm.UserId = u.UserID 
  inner join Groups g on g.GroupID = gm.GroupID
  where [Enabled] = 1
  and g.Groupname = '{0}'
  or g.Groupname = '{1}'
  order by u.UserID
