<?
	$user = "risensh1ne";
	$password = "tkfkdgo1";
	$dbname = "risensh1ne";
	
	mysql_connect("localhost", $user, $password) or die("Cant connect into database");
	mysql_select_db($dbname)or die("Cant connect into database");
	mysql_query("set names utf8");
	
	$id = $_POST["id"];
	$pass = $_POST["pass"];

	$sql = "select * from user where id='$id' and pass='$pass'";
	$ret_arr = array();
	
	$result = mysql_query($sql);
	if ($result) {
		$row = mysql_fetch_array($result);
		if ($row) {
			
			$data["id"] = $row["id"];
			$data["name"] = $row["name"];
			$data["level"] = $row["level"];
			$data["cash"] = $row["cash"];
			
			$ret_arr["success"] = true;
			$ret_arr["data"] = $data;
		} else {
			$ret_arr["success"] = true;
		}
	} else {
		$ret_arr["success"] = false;
		$ret_arr["data"] = null;
	}
	mysql_close();
	echo json_encode($ret_arr);
?>
