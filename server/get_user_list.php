<?
	$user = "risensh1ne";
	$password = "tkfkdgo1";
	$dbname = "risensh1ne";
	
	mysql_connect("localhost", $user, $password) or die("Cant connect into database");
	mysql_select_db($dbname)or die("Cant connect into database");
	mysql_query("set names utf8");
	
	$sql = "select * from user, user_stat WHERE is_logon=1 AND user.id = user_stat.user_id";
	$ret_arr = array();
	
	$result = mysql_query($sql);
	if ($result) {

		$data_list = array();
		
		while ($row = mysql_fetch_array($result)) {
			
			$data["id"] = $row["id"];
			$data["level"] = $row["level"];

			$data["win"] = $row["win"];
			$data["lose"] = $row["lose"];
			$data["draw"] = $row["draw"];
					
			
			array_push($data_list, $data);
		}
		
		$ret_arr["success"] = true;
		$ret_arr["data"] = $data_list;
		
	} else {
		$ret_arr["success"] = false;
		$ret_arr["data"] = null;
	}
	mysql_close();
	echo json_encode($ret_arr);
?>