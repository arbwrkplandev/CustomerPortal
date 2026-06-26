$inputPath = "Database-Table-Usage.md"
$outputPath = "Database-Table-Usage.pdf"

$rawLines = Get-Content -Path $inputPath
$maxLinesPerPage = 48
$pages = @()
for ($i = 0; $i -lt $rawLines.Count; $i += $maxLinesPerPage) {
    $end = [Math]::Min($i + $maxLinesPerPage - 1, $rawLines.Count - 1)
    $pages += ,($rawLines[$i..$end])
}

function Escape-PdfText([string]$text) {
    return $text.Replace('\\', '\\\\').Replace('(', '\\(').Replace(')', '\\)')
}

$sb = New-Object System.Text.StringBuilder
$null = $sb.AppendLine('%PDF-1.4')
$offsets = New-Object System.Collections.Generic.List[int]
$offsets.Add(0) | Out-Null

function Add-Obj {
    param(
        [System.Text.StringBuilder]$Builder,
        [System.Collections.Generic.List[int]]$Offsets,
        [int]$Number,
        [string]$Body
    )

    $Offsets.Add([System.Text.Encoding]::ASCII.GetByteCount($Builder.ToString())) | Out-Null
    $null = $Builder.AppendLine("$Number 0 obj")
    $null = $Builder.AppendLine($Body)
    $null = $Builder.AppendLine('endobj')
}

$pageCount = $pages.Count
$fontObjNum = 3 + ($pageCount * 2)
$kids = @()

for ($p = 0; $p -lt $pageCount; $p++) {
    $streamObjNum = 3 + ($p * 2)
    $pageObjNum = 4 + ($p * 2)
    $kids += "$pageObjNum 0 R"

    $content = New-Object System.Text.StringBuilder
    $null = $content.AppendLine('BT')
    $null = $content.AppendLine('/F1 9 Tf')
    $null = $content.AppendLine('50 790 Td')
    foreach ($line in $pages[$p]) {
        $safeLine = if ($null -eq $line) { '' } else { [string]$line }
        $escaped = Escape-PdfText $safeLine
        $null = $content.AppendLine("($escaped) Tj")
        $null = $content.AppendLine('0 -14 Td')
    }
    $null = $content.AppendLine('ET')

    $contentText = $content.ToString()
    $len = [System.Text.Encoding]::ASCII.GetByteCount($contentText)
    $streamBody = "<< /Length $len >>`nstream`n$contentText`nendstream"
    Add-Obj -Builder $sb -Offsets $offsets -Number $streamObjNum -Body $streamBody

    $pageBody = "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 $fontObjNum 0 R >> >> /Contents $streamObjNum 0 R >>"
    Add-Obj -Builder $sb -Offsets $offsets -Number $pageObjNum -Body $pageBody
}

Add-Obj -Builder $sb -Offsets $offsets -Number 1 -Body "<< /Type /Catalog /Pages 2 0 R >>"
Add-Obj -Builder $sb -Offsets $offsets -Number 2 -Body "<< /Type /Pages /Count $pageCount /Kids [ $($kids -join ' ') ] >>"
Add-Obj -Builder $sb -Offsets $offsets -Number $fontObjNum -Body "<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>"

$xrefStart = [System.Text.Encoding]::ASCII.GetByteCount($sb.ToString())
$size = $fontObjNum + 1
$null = $sb.AppendLine('xref')
$null = $sb.AppendLine("0 $size")
$null = $sb.AppendLine('0000000000 65535 f ')
for ($i = 1; $i -le $fontObjNum; $i++) {
    $off = $offsets[$i]
    $null = $sb.AppendLine(($off.ToString('0000000000') + ' 00000 n '))
}
$null = $sb.AppendLine('trailer')
$null = $sb.AppendLine("<< /Size $size /Root 1 0 R >>")
$null = $sb.AppendLine('startxref')
$null = $sb.AppendLine($xrefStart.ToString())
$null = $sb.Append('%%EOF')

[System.IO.File]::WriteAllText((Join-Path (Get-Location) $outputPath), $sb.ToString(), [System.Text.Encoding]::ASCII)
Write-Host "Generated $outputPath"
Get-Item $outputPath | Select-Object Name, Length, LastWriteTime
