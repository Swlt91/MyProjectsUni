<?php
//retrieve the Torrent file
require_once("Torrent.php");
class Request
{
    //create the used methods
    private static $method_type = array('get', 'post', 'put', 'delete');
    
	

    public static function getRequest()
    {
        //determines which method has been requested
        $method = strtolower($_SERVER['REQUEST_METHOD']);
        if (in_array($method, self::$method_type)) {
            //revoke method
            $data_name = $method . 'Data';
            return self::$data_name($_REQUEST);
        }
        return false;
    }

    //GET 
    private static function getData($request_data)
    {
       //create instance of the torrent class
		$torrent = new Torrent();
        
		// if remove single is in the header		
		if(!empty($request_data['removeSingle'])) 
		{
			// remove the entry from the database by id,ip and file
			$id = $request_data['id'];
			$ip_address = $request_data['ip_address'];
			$hash = $request_data['hash'];
			return $torrent->removeSingle($id, $ip_address, $hash);
		}
		
		if(!empty($request_data['remove'])) 
		{
			// remove the entry from the database by ip and id
			$id = $request_data['id'];
			$ip_address = $request_data['ip_address'];
			return $torrent->remove($id, $ip_address);
		}
		
		if(!empty($request_data['hash'])) 
		{
			// Add new peer and file information
			$hash = $request_data['hash'];
			$id = $request_data['id'];
			$ip_address = $request_data['ip_address'];
			$port = $request_data['port'];
			$left = $request_data['left'];
			return $torrent->getPeers($hash, $id, $ip_address, $port, $left);
		}	
    }

	//POST
    private static function postData($request_data)
    {
		//create instance of the torrent class
		$torrent = new Torrent();
		
		//check to see if fileupload is included in the header
		if(!empty($request_data['fileUpload'])) 
		{
		    $uploads_dir = './uploads'; //Directory to save the file that comes from client application.
            if ($_FILES["file"]["error"] == UPLOAD_ERR_OK) 
            {
				// get the file and save it to the server
                $tmp_name = $_FILES["file"]["tmp_name"];
                $name = $_FILES["file"]["name"];
                move_uploaded_file($tmp_name, "$uploads_dir/$name");
		    }
        }
    }
}