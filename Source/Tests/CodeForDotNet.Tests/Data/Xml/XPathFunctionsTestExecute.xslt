<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:fn="http://www.w3.org/2005/xpath-functions">

  <!-- Output XML, indented and whitespace clean.  -->
  <xsl:output method="xml" indent="yes" />
  <xsl:strip-space elements="*" />

  <!-- Test XML document. -->
  <xsl:template match="/">
    <results>
      <functions>
        <matches>
          <xsl:apply-templates select="tests/functions/matches/test" />
        </matches>
      </functions>
    </results>
  </xsl:template>

  <!-- Test element - Executes the XPath matches function on each test pattern. -->
  <xsl:template match="functions/matches/test">
    <xsl:element name="result">
      <xsl:variable name="input" select="input[text()]" />
      <xsl:copy-of select="@id | input" />
      <xsl:variable name="testId" select="@id" />
      <xsl:element name="patterns">
        <xsl:for-each select="patterns/pattern">
          <xsl:element name="pattern">
            <xsl:attribute name="id">
              <xsl:value-of select="concat($testId, 'Pattern', @id)" />
            </xsl:attribute>
            <xsl:copy-of select="@value | @flags | @expected" />
            <xsl:attribute name="output">
              <xsl:choose>
                <xsl:when test="boolean(@flags)">
                  <xsl:value-of select="fn:matches($input, @value, @flags)" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="fn:matches($input, @value)" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>
