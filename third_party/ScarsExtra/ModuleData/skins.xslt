<?xml version="1.0" encoding="utf-8"?>
<!-- Scars Extra
     1) scar_nobl_* - шрамы через глаз БЕЗ бельма (с culture-тегами: их получают и NPC)
     2) scar_dyn/dbr/dbl_* - пул комбо-шрамов (без culture-тегов), scar_p*_01 - предпросмотр
     3.1: слоты добавляются во ВСЕ расы (man/woman), не только human.
     ПОРЯДОК НЕ МЕНЯТЬ: номера слотов сохраняются у героев. Новое - только в конец. -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output omit-xml-declaration="yes"/>
    <xsl:template match="@*|node()">
        <xsl:copy><xsl:apply-templates select="@*|node()"/></xsl:copy>
    </xsl:template>
    <xsl:template match='race/skin[@name="man" or @name="woman"]/tattoo_materials'>
        <xsl:copy>
            <xsl:copy-of select="@*"/>
            <xsl:copy-of select="node()"/>
            <tattoo_material name="scar_nobl_c" tags="scar">
                <style_tags><style_tag name="ScarNoBlind_C"/><style_tag name="empire"/><style_tag name="battania"/><style_tag name="aserai"/><style_tag name="sturgia"/><style_tag name="vlandia"/><style_tag name="khuzait"/><style_tag name="nord"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_nobl_g" tags="scar">
                <style_tags><style_tag name="ScarNoBlind_G"/><style_tag name="empire"/><style_tag name="battania"/><style_tag name="aserai"/><style_tag name="sturgia"/><style_tag name="vlandia"/><style_tag name="khuzait"/><style_tag name="nord"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_nobl_i" tags="scar">
                <style_tags><style_tag name="ScarNoBlind_I"/><style_tag name="empire"/><style_tag name="battania"/><style_tag name="aserai"/><style_tag name="sturgia"/><style_tag name="vlandia"/><style_tag name="khuzait"/><style_tag name="nord"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_nobl_j" tags="scar">
                <style_tags><style_tag name="ScarNoBlind_J"/><style_tag name="empire"/><style_tag name="battania"/><style_tag name="aserai"/><style_tag name="sturgia"/><style_tag name="vlandia"/><style_tag name="khuzait"/><style_tag name="nord"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_nobl_m" tags="scar">
                <style_tags><style_tag name="ScarNoBlind_M"/><style_tag name="empire"/><style_tag name="battania"/><style_tag name="aserai"/><style_tag name="sturgia"/><style_tag name="vlandia"/><style_tag name="khuzait"/><style_tag name="nord"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_01" tags="scar">
                <style_tags><style_tag name="ScarDyn01"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_02" tags="scar">
                <style_tags><style_tag name="ScarDyn02"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_03" tags="scar">
                <style_tags><style_tag name="ScarDyn03"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_04" tags="scar">
                <style_tags><style_tag name="ScarDyn04"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbr_01" tags="scar,blindrighteye">
                <style_tags><style_tag name="ScarDynBR01"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbr_02" tags="scar,blindrighteye">
                <style_tags><style_tag name="ScarDynBR02"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbl_01" tags="scar,blindlefteye">
                <style_tags><style_tag name="ScarDynBL01"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbl_02" tags="scar,blindlefteye">
                <style_tags><style_tag name="ScarDynBL02"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_05" tags="scar">
                <style_tags><style_tag name="ScarDyn05"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_06" tags="scar">
                <style_tags><style_tag name="ScarDyn06"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_07" tags="scar">
                <style_tags><style_tag name="ScarDyn07"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_08" tags="scar">
                <style_tags><style_tag name="ScarDyn08"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_09" tags="scar">
                <style_tags><style_tag name="ScarDyn09"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_10" tags="scar">
                <style_tags><style_tag name="ScarDyn10"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_11" tags="scar">
                <style_tags><style_tag name="ScarDyn11"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dyn_12" tags="scar">
                <style_tags><style_tag name="ScarDyn12"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbr_03" tags="scar,blindrighteye">
                <style_tags><style_tag name="ScarDynBR03"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbr_04" tags="scar,blindrighteye">
                <style_tags><style_tag name="ScarDynBR04"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbr_05" tags="scar,blindrighteye">
                <style_tags><style_tag name="ScarDynBR05"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbr_06" tags="scar,blindrighteye">
                <style_tags><style_tag name="ScarDynBR06"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbl_03" tags="scar,blindlefteye">
                <style_tags><style_tag name="ScarDynBL03"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbl_04" tags="scar,blindlefteye">
                <style_tags><style_tag name="ScarDynBL04"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbl_05" tags="scar,blindlefteye">
                <style_tags><style_tag name="ScarDynBL05"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_dbl_06" tags="scar,blindlefteye">
                <style_tags><style_tag name="ScarDynBL06"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_pdy_01" tags="scar">
                <style_tags><style_tag name="ScarPreview"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_pbr_01" tags="scar,blindrighteye">
                <style_tags><style_tag name="ScarPreviewBR"/></style_tags>
            </tattoo_material>
            <tattoo_material name="scar_pbl_01" tags="scar,blindlefteye">
                <style_tags><style_tag name="ScarPreviewBL"/></style_tags>
            </tattoo_material>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
