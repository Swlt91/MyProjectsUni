<?php

//retrieve the config file containing the database connection information
require_once __DIR__ . '/database_config.php';

//connect using the details
$con = mysqli_connect(DB_SERVER, DB_USER, DB_PASSWORD, DB_DATABASE);

if(!$con)
{
    echo "cannot connet to the server";
}

?>