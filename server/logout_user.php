<?
	$user = "risensh1ne";
	$password = "tkfkdgo1";
	$dbname = "risensh1ne";
	
	mysql_connect("localhost", $user, $password) or die("Cant connect into database");
	mysql_select_db($dbname)or die("Cant connect into database");
	mysql_query("set names utf8");
	
	$id = $_POST["id"];

	$sql = "update user set is_logon=0 where id='$id'";
	mysql_query($sql);
	mysql_close();
?>
