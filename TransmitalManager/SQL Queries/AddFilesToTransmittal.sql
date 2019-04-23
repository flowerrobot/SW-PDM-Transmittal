--insert into TransFiles (TransId, FileName, FileId, Version, Revision)
--values ('@Tid', '@FileName', '@fileId','@Ver','@Rev')
--SELECT SCOPE_IDENTITY();  


insert into TransFiles (TransId, FileName, FileId, Version, Revision)
values (@Tid, @FileName, @fileId,@Ver,@Rev)
SELECT SCOPE_IDENTITY();  

