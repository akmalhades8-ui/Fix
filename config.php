<?php
// config.php
date_default_timezone_set("Asia/Kuala_Lumpur");

// User DB (usertbl)
$userServer = "31.58.143.7";
$userSQL    = "sa";
$passSQL    = "Pizam6645@";
$userDB     = "youxiuser";

// Game DB
$gameServer = "31.58.143.7";
$gameSQL    = "sa";
$gamePass   = "Pizam6645@";
$gameDB     = "ROLgame";

// Connect
$Get_User = @odbc_connect("Driver={SQL Server};Server=$userServer;Database=$userDB;", $userSQL, $passSQL);
$Get_Game = @odbc_connect("Driver={SQL Server};Server=$gameServer;Database=$gameDB;", $gameSQL, $gamePass);

// Helper: try to give item to character (adapt to your actual DB / stored-proc)
function GiveItemToChar($charCID, $itemID, $amount = 1) {
    global $Get_Game;
    if (!$Get_Game) return ['success'=>false,'msg'=>'No game DB'];

    // Try lookup UID for CID (optional)
    $uid = 0;
    $stmtCid = @odbc_prepare($Get_Game, "SELECT UID FROM CharInfo WHERE CID = ?");
    if ($stmtCid && @odbc_execute($stmtCid, array($charCID)) && ($r = @odbc_fetch_array($stmtCid))) {
        $uid = $r['UID'] ?? 0;
    }

    // Example insert into TblWebItemInfo (change to your real table/proc)
    $sql = "INSERT INTO TblWebItemInfo (UID, CID, ItemPrototypeID, Amount) VALUES (?, ?, ?, ?)";
    $stmt = @odbc_prepare($Get_Game, $sql);
    if (!$stmt) return ['success'=>false,'msg'=>'Prepare failed'];
    $ok = @odbc_execute($stmt, array($uid, $charCID, $itemID, $amount));
    if ($ok) return ['success'=>true,'msg'=>'Item inserted'];
    return ['success'=>false,'msg'=>'DB insert failed'];
}
?>
