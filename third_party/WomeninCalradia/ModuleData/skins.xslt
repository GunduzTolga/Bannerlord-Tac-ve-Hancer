<xsl:stylesheet version = "1.0" xmlns:xsl = "http://www.w3.org/1999/XSL/Transform">
    <xsl:output omit-xml-declaration = "yes"/>
    <xsl:template match = "@*|node()">
        <xsl:copy>
            <xsl:apply-templates select = "@*|node()"/>
        </xsl:copy>
    </xsl:template>
	
	<xsl:template match='race[@id = "human"]/skin[@name = "man"]/face_textures'>
		<xsl:copy>
            <xsl:copy-of select = "@*" />
            <xsl:copy-of select = "*" />
            <face_texture
				name="malfoy_male_face_a"
				lod_material="head_male_a.lod"
				color="0xFFFFFFFF"
				tags="face_texture1,face_texture2"></face_texture>
			<face_texture
				name="malfoy_male_face_b"
				lod_material="head_male_a.lod"
				color="0xFFFFFFFF"
				tags="face_texture3,face_texture4"></face_texture>
			<face_texture
				name="malfoy_male_face_c"
				lod_material="head_male_a.lod"
				color="0xFFFFFFFF"
				tags="face_texture5,face_texture6,face_texture7"></face_texture>
			<face_texture
				name="malfoy_male_face_d"
				lod_material="head_male_a.lod"
				color="0xFFFFFFFF"
				tags="face_texture8,face_texture9,face_texture10"></face_texture>
        </xsl:copy>
	</xsl:template>
	<xsl:template match='race[@id = "human"]/skin[@name = "kid_2_male"]/face_textures'>
		<xsl:copy>
            <xsl:copy-of select = "@*" />
            <xsl:copy-of select = "*" />
            <face_texture
				name="malfoy_male_face_a"
				lod_material="head_male_a.lod"
				color="0xFFFFFFFF"
				tags="face_texture1,face_texture2"></face_texture>
			<face_texture
				name="malfoy_male_face_b"
				lod_material="head_male_a.lod"
				color="0xFFFFFFFF"
				tags="face_texture3,face_texture4"></face_texture>
			<face_texture
				name="malfoy_male_face_c"
				lod_material="head_male_a.lod"
				color="0xFFFFFFFF"
				tags="face_texture5,face_texture6,face_texture7"></face_texture>
			<face_texture
				name="malfoy_male_face_d"
				lod_material="head_male_a.lod"
				color="0xFFFFFFFF"
				tags="face_texture8,face_texture9,face_texture10"></face_texture>
        </xsl:copy>
	</xsl:template>

    <xsl:template match='race[@id = "human"]/skin[@name = "woman"]/face_textures'>
		<face_textures>
			<face_texture
				name="head_female_a"
				lod_material="head_female_a.lod"
				color="0xFFCAD3E0"
				tags="face_texture2"></face_texture>
			<face_texture
				name = "malfoy_female_face_b"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture2"></face_texture>
			<face_texture
				name = "malfoy_female_face_c"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture3"></face_texture>
			<face_texture
				name = "malfoy_female_face_d"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture4"></face_texture>
			<face_texture
				name = "malfoy_female_face_a"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture2"></face_texture>
		</face_textures>		
	</xsl:template>
	
	<xsl:template match='race[@id = "human"]/skin[@name = "kid_1_female"]/face_textures'>
		<face_textures>
			<face_texture
				name="head_female_a"
				lod_material="head_female_a.lod"
				color="0xFFCAD3E0"
				tags="face_texture2"></face_texture>
			<face_texture
				name = "malfoy_female_face_b"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture2"></face_texture>
			<face_texture
				name = "malfoy_female_face_c"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture3"></face_texture>
			<face_texture
				name = "malfoy_female_face_d"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture4"></face_texture>
			<face_texture
				name = "malfoy_female_face_a"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture2"></face_texture>
		</face_textures>		
	</xsl:template>
	
	<xsl:template match='race[@id = "human"]/skin[@name = "kid_2_female"]/face_textures'>
		<face_textures>
			<face_texture
				name="head_female_a"
				lod_material="head_female_a.lod"
				color="0xFFCAD3E0"
				tags="face_texture2"></face_texture>
			<face_texture
				name = "malfoy_female_face_b"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture2"></face_texture>
			<face_texture
				name = "malfoy_female_face_c"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture3"></face_texture>
			<face_texture
				name = "malfoy_female_face_d"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture4"></face_texture>
			<face_texture
				name = "malfoy_female_face_a"
				lod_material = "head_female_a.lod"
				color = "0xFFCAD3E0"
				tags = "face_texture2"></face_texture>
		</face_textures>		
	</xsl:template>

</xsl:stylesheet>