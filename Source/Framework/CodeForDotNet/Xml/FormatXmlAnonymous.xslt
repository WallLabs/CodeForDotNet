<?xml version="1.0" encoding="utf-8"?>
<!-- XSLT to strip XML namespaces from elements, making them anonymous -->
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  exclude-result-prefixes="msxsl"
>
  <!-- Format outer XML stripping extra space and XML declaration -->
  <xsl:strip-space elements="*" />
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

  <!-- Copy structural elements (with children) preserving any XSL indent spacing (do not strip whitespace) -->
  <xsl:template match="*[count(*) > 0]">
    <xsl:element name="{name()}">
      <xsl:apply-templates select="@* | node()" />
    </xsl:element>
  </xsl:template>

  <!-- Copy comments without stripping white space -->
  <xsl:template match="comment()">
    <xsl:copy-of select="." />
  </xsl:template>

  <!-- Copy content elements (no children just text content) with white space stripped -->
  <xsl:template match="*[count(*) = 0]">
    <xsl:element name="{name()}">
      <xsl:apply-templates select="@* | comment()" />
      <xsl:value-of select="text()" />
    </xsl:element>
  </xsl:template>

  <!-- Copy attributes with white space stripped -->
  <xsl:template match="@*">
    <xsl:attribute name="{name()}">
      <xsl:value-of select="." />
    </xsl:attribute>
  </xsl:template>

</xsl:stylesheet>
