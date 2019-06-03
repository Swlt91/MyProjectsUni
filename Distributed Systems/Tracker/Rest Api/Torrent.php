<?php

Class Torrent 
{
	public function getPeers($hash, $id, $ip_address, $port, $left)
	{
		// adds entry to database and returns list of peers, if there are any active on the same file
		include 'database_connect.php';
	
		// run query to see if this peer already exists in the table
		$query = "SELECT * FROM torrent WHERE hash = '".$hash."' AND id = '".$id."' AND ip_address = '".$ip_address."' AND port = '".$port."'";
		 $rows = mysqli_query($con, $query);
		  
		  // if the peer exists, then update the information rather than add a new entry
		if (mysqli_num_rows($rows) == 1) 
		{
			$query = "UPDATE torrent SET amount_left = '".$left."', time_entered = now() WHERE hash = '".$hash."' AND id = '".$id."' AND ip_address = '".$ip_address."' AND port = '".$port."'";		
			$newUser = mysqli_query($con, $query);
		}
	
		// else the entry is new, add it
    	else
    	{
    		$newUser = mysqli_query($con, "INSERT INTO torrent (hash, id, ip_address, port, amount_left) VALUES ('".$hash."', '".$id."','".$ip_address."','".$port."','".$left."')");
    	}
           
		// Find all entries in the table with the same hash
        $query = "SELECT hash, id, ip_address, port, amount_left FROM torrent WHERE hash = '".$hash."'";
		 $rows = mysqli_query($con, $query);
		  
		 // if the query returns results, return them in array form
		if (mysqli_num_rows($rows) > 0) 
		{
			$outp = array();
			$outp = $rows->fetch_all(MYSQLI_ASSOC);
			return $outp;    
		}		
	}

	public function remove($id, $ipAddress)
	{
		//deletes all entries from the database by id and ip address
		include 'database_connect.php';
		
		// run a query to delete all entries under the incoming id and ip address
		$query = "DELETE FROM torrent WHERE id = '".$id."' AND ip_address = '".$ipAddress."'";
		
		$rows = mysqli_query($con, $query);
		if (mysqli_affected_rows($con) > 0) 
		{
			$result =  array('error' => 'none found');
			return $result;
		}
			
		else
		{
			$result =  array('success' => 'deleted');
			return $result;
		}	
	}
	
	public function removeSingle($id, $ipAddress, $hash)
	{
		//deletes a single entry using hash, id and ip address
		include 'database_connect.php';
		
		// run a query to delete by hash, id and ipaddress
		$query = "DELETE FROM torrent WHERE id = '".$id."' AND ip_address = '".$ipAddress."' AND hash = '".$hash."'";
		
		$rows = mysqli_query($con, $query);
		if (mysqli_affected_rows($con) > 0) 
		{
			$result =  array('error' => 'none found');
			return $result;
		}
			
		else
		{
			$result =  array('success' => 'deleted');
			return $result;
		}	
	}
}
?>