<?php
header('Content-Type: application/json; charset=utf-8');

$admins = json_decode(file_get_contents('admins.json'), true);
$account = $_POST['account'] ?? '';
$password = $_POST['password'] ?? '';

if(isset($admins[$account]) && $admins[$account] === $password){
    echo json_encode(["success"=>true, "message"=>"Login successful"]);
}else{
    echo json_encode(["success"=>false, "message"=>"Unauthorized"]);
}
?>
