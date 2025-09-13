<?php
header('Content-Type: application/json; charset=utf-8');

$UID = $_POST['UID'] ?? '';
$CID = $_POST['CID'] ?? '';
$ItemID = $_POST['ItemID'] ?? '';
$MailID = $_POST['MailIdentifier'] ?? '';

if(!$UID || !$CID || !$ItemID || !$MailID){
    echo json_encode(["status"=>"error","msg"=>"Missing parameters"]);
    exit;
}

$mailboxFile = 'mailbox.json';
$mailbox = file_exists($mailboxFile) ? json_decode(file_get_contents($mailboxFile), true) : [];

$found = false;
foreach($mailbox as &$m){
    if($m['ID'] == $MailID && $m['PlayerID'] == $UID){
        if($m['Claimed']){
            echo json_encode(["status"=>"error","msg"=>"Already claimed"]);
            exit;
        }

        $m['Claimed'] = true;
        $found = true;

        // INSERT TO SQL (ROLgame.dbo.TblWebItemInfo)
        $gameServer = "31.58.143.7";
        $gameSQL    = "sa";
        $gamePass   = "Pizam6645@";
        $gameDB     = "ROLgame";
        $Get_Game = odbc_connect("Driver={SQL Server};Server=$gameServer;Database=$gameDB;", $gameSQL, $gamePass);
        if($Get_Game){
            $stmt = odbc_prepare($Get_Game, "INSERT INTO TblWebItemInfo (UID,CID,ItemPrototypeID,Amount) VALUES (?, ?, ?, ?)");
            odbc_execute($stmt, [$UID, $CID, $ItemID, 1]);
        }
        break;
    }
}
if(!$found){
    echo json_encode(["status"=>"error","msg"=>"Mail not found"]);
    exit;
}

file_put_contents($mailboxFile, json_encode($mailbox, JSON_PRETTY_PRINT));
echo json_encode(["status"=>"success","msg"=>"Claim successful"]);
?>
