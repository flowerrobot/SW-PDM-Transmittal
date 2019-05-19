

select * 
from TransmitalRecipients TR
--inner join Recipient R on Tr.RecipientId = R.Id
where TR.TransmitalId = {0}