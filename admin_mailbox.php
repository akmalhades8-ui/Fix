<?php
header('Content-Type: application/json; charset=utf-8');

$admins = json_decode(file_get_contents('admins.json'), true);
$account = $_POST['account'] ?? '';
$password = $_POST['password'] ?? '';

if(!isset($admins[$account]) || $admins[$account] !== $password){
    echo json_encode(["status"=>"error","msg"=>"Unauthorized"]);
    exit;
}

$PlayerID = $_POST['PlayerID'] ?? '';
$ItemID   = $_POST['ItemID'] ?? '';
$Message  = $_POST['Message'] ?? '';

if(!$PlayerID || !$Message){
    echo json_encode(["status"=>"error","msg"=>"Missing parameters"]);
    exit;
}

// Load mailbox.json
$mailboxFile = 'mailbox.json';
$mailbox = file_exists($mailboxFile) ? json_decode(file_get_contents($mailboxFile), true) : [];

$entry = [
    "ID" => uniqid(),
    "PlayerID" => $PlayerID,
    "ItemID" => $ItemID,
    "Message" => $Message,
    "Claimed" => false,
    "Date" => date("Y-m-d H:i:s")
];

$mailbox[] = $entry;
file_put_contents($mailboxFile, json_encode($mailbox, JSON_PRETTY_PRINT));

echo json_encode(["status"=>"success","msg"=>"Mail sent to PlayerID $PlayerID"]);
?>
