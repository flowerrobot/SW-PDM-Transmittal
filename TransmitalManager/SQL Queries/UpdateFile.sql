--update TransFiles
--set {1}
--where Id = {0}



update TransFiles
set 
--TransId = ,
FileName = '@FileName',
FileId = '@id',
Version = '@Ver',
Revision = '@Rev'
where Id = {0} and TransId={1}