<?
	$user = "risensh1ne";
	$password = "tkfkdgo1";
	$dbname = "risensh1ne";
	
	mysql_connect("localhost", $user, $password) or die("Cant connect into database");
	mysql_select_db($dbname)or die("Cant connect into database");
	mysql_query("set names utf8");
	
	$id = $_POST["id"];
	$pass = $_POST["pass"];
	$name = $_POST["name"];
	$email = $_POST["email"];

	$sql = "insert into user (id, pass, name, last_login, email) values ('$id', '$pass', '$name', now(), '$email')";
	
	$result = mysql_query($sql);
	
	mysql_close();
	
	echo $result;
?>