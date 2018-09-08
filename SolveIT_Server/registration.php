<?php
if ($_POST['enter'] and $_POST['name']) {
    $_POST['name'] = FormChars($_POST['name']);
    mysqli_query($CONNECT, "INSERT INTO SolveIT (`id`, `name`, `team`, `money`, `stage`) VALUES (NULL, '$_POST[name]', '$_POST[name]', 0, 0)");
}
?>
<!DOCTYPE html>
<html lang="ru">
<?php
echo '
	<body>
		<form method="POST" action="/page/solveITreg">
			<input type="text" class="NameAdd" name="name" required><br>
			<input type="submit" name="enter" value="Добавить">
			<input type="button" value="Назад" onclick="location.href=\'/subject/'.$name_angl.'\';"/>
		</form>';
?>
</body>
</html>