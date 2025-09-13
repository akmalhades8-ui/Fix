<?php
header('Content-Type: application/json; charset=utf-8');

$admins = json_decode(file_get_contents('admins.json'), true);
$account = $_POST['account'] ?? '';
$password = $_POST['password'] ?? '';

if(!isset($admins[$account]) || $admins[$account] !== $password){
    echo json_encode(["status"=>"error","msg"=>"Unauthorized"]);
    exit;
}

$mailboxFile = 'mailbox.json';
$mailbox = file_exists($mailboxFile) ? json_decode(file_get_contents($mailboxFile), true) : [];

echo json_encode(["status"=>"success", "mails"=>$mailbox]);
?>
