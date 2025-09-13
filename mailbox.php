<?php
header("Content-Type: application/json; charset=utf-8");
$mailboxFile = __DIR__ . "/mailbox.json";
$mails = file_exists($mailboxFile) ? json_decode(file_get_contents($mailboxFile), true) : [];
if (!is_array($mails)) $mails = [];
echo json_encode(["status"=>"success","mails"=>$mails], JSON_PRETTY_PRINT|JSON_UNESCAPED_UNICODE);
