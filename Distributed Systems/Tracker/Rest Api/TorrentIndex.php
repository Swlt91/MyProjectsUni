<?php
//data control
require('TorrentRequest.php');
//output
require('Response.php');
//get data
$data = Request::getRequest();
//output result
Response::sendResponse($data);

