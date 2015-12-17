<?
	$user = "risensh1ne";
	$password = "tkfkdgo1";
	$dbname = "risensh1ne";
	
	mysql_connect("localhost", $user, $password) or die("Cant connect into database");
	mysql_select_db($dbname)or die("Cant connect into database");
	mysql_query("set names utf8");

	$ret_arr = array();
	
	$id = $_POST["id"];
	$pass = $_POST["pass"];
	$name = $_POST["name"];
	$email = $_POST["email"];
	
	$sql = "insert into user (id, pass, name, last_login, email) values ('$id', '$pass', '$name', now(), '$email');";
	
	$register_success = false;
	
	$result = mysql_query($sql);
	if ($result) {
		$sql = "insert into user_stat (user_id) values ('$id')";
		$result = mysql_query($sql);
		
		if ($result) {
			
			$register_success = true;
		}
	}
	
	if ($register_success) {
		$ret_arr["result"] = "success";
	} else {
		$ret_arr["result"] = "fail";
		$ret_arr["errno"] = mysql_errno();
	}
	
	mysql_close();

	echo json_encode($ret_arr);
?>