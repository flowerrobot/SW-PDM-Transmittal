 SELECT *

FROM TransFiles tf 
inner join Transmittal t on t.Id = tf.[TransId] 
where tf.FileName like {0}
{1}
order by tf.TransId