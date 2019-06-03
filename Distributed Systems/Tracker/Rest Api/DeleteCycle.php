<?php
	
// runs every 5 minutes on the server, deleting any entires older than five minutes

include 'database_connect.php';	

  $query = "DELETE FROM torrent WHERE time_entered <  DATE_SUB(NOW(), INTERVAL 5 MINUTE)";
		
    $rows = mysqli_query($con, $query);
    if (mysqli_affected_rows($con) > 0) 
    {
		$result =  array('error' => 'no user found');
		return $result;
	}
			
    else
	{
		$result =  array('success' => 'user deleted');
		return $result;
	}
?>