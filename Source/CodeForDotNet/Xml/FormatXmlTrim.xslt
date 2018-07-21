<?xml version="1.0" encoding="utf-8"?>
<!-- XSLT to trim whitespace from both the XML structure and content -->
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
    <xsl:element name="{name()}" namespace="{namespace-uri()}">
      <xsl:copy-of select="namespace::*"/>
      <xsl:apply-templates select="@* | node()" />
    </xsl:element>
  </xsl:template>

  <!-- Copy comments without stripping white space -->
  <xsl:template match="comment()">
    <xsl:copy-of select="." />
  </xsl:template>

  <!-- Copy content elements (no children just text content) with white space stripped -->
  <xsl:template match="*[count(*) = 0]">
    <xsl:element name="{name()}" namespace="{namespace-uri()}">
      <xsl:apply-templates select="@* | comment()" />
      <xsl:value-of select="normalize-space(text())" />
    </xsl:element>
  </xsl:template>

  <!-- Copy attributes with white space stripped -->
  <xsl:template match="@*">
    <xsl:attribute name="{name()}" namespace="{namespace-uri()}">
      <xsl:value-of select="normalize-space(.)" />
    </xsl:attribute>
  </xsl:template>

</xsl:stylesheet>
