<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>Список студентів гуртожитку</title>
				<style>
					table { font-family: Arial, sans-serif; border-collapse: collapse; width: 100%; }
					th, td { border: 1px solid #dddddd; text-align: left; padding: 8px; }
					th { background-color: #f2f2f2; }
				</style>
			</head>
			<body>
				<h2>
					Студенти (Гуртожиток №<xsl:value-of select="Hostel/@number"/>)
				</h2>
				<table>
					<tr>
						<th>П.І.П.</th>
						<th>Факультет</th>
						<th>Курс</th>
						<th>№ кімнати</th>
						<th>Плата (грн/міс)</th>
					</tr>
					<xsl:for-each select="Hostel/Student">
						<tr>
							<td>
								<xsl:value-of select="FullName"/>
							</td>
							<td>
								<xsl:value-of select="Faculty"/>
							</td>
							<td>
								<xsl:value-of select="Course"/>
							</td>
							<td>
								<xsl:value-of select="RoomNumber"/>
							</td>
							<td>
								<xsl:value-of select="MonthlyFee"/>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
