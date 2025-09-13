<?php
header('Content-Type: application/json; charset=utf-8');

$UID = $_GET['UID'] ?? '';
if(!$UID){
    echo json_encode(["status"=>"error","msg"=>"Missing UID"]);
    exit;
}

$mailboxFile = 'mailbox.json';
$mailbox = file_exists($mailboxFile) ? json_decode(file_get_contents($mailboxFile), true) : [];

$playerMails = [];
foreach($mailbox as $m){
    if($m['PlayerID'] == $UID){
        $playerMails[] = $m;
    }
}

echo json_encode(["status"=>"success","mails"=>$playerMails]);
?>
