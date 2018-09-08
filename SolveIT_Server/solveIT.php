<?php
/**
 * Created by PhpStorm.
 * User: taller
 * Date: 5/9/2017
 * Time: 4:30 PM
 */
global $CONNECT;

//print_r($_GET);

if(isset($_GET['insert']))
{
	$name = $_GET['insert'];
	mysqli_connect($CONNECT, "INSERT INTO `SolveIT`(`name`, `team`, `money`, `stage`) VALUES ('$name','$name',0,1)");
}

if(isset($_GET['auth']))
{
	$param = $_GET['auth'];
	$sth = mysqli_query($CONNECT, "SELECT * FROM SolveIT ORDER BY money");
	$rows = array();
	while($r = mysqli_fetch_assoc($sth)) {
	    $rows[] = $r;
	}
	print json_encode($rows);
}

if(isset($_GET['login']) && isset($_GET['stage']) && isset($_GET['money']))
{
	$login = $_GET['login'];
	$stage = $_GET['stage'];
	$money = $_GET['money'];

	mysqli_query($CONNECT, "UPDATE SolveIT SET stage = '$stage', money = '$money' WHERE name = '$login'");
}

if (isset($_GET['rating'])) {
	$sth = mysqli_query($CONNECT, "SELECT * FROM SolveIT ORDER BY money LIMIT 8");
	$rows = array();
	while($r = mysqli_fetch_assoc($sth)) {
	    $rows[] = $r;
	}
	print json_encode($rows);
}

?>