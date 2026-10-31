$filePath = 'c:\User\Projects\Store\FrontEnds\Store.WebUI\Store.WebUI.csproj'
$content = Get-Content $filePath -Raw -Encoding UTF8
$endTag = '</Project>'
$idx = $content.IndexOf($endTag)
if ($idx -ge 0) {
    $clean = $content.Substring(0, $idx + $endTag.Length)
    [System.IO.File]::WriteAllText($filePath, $clean, [System.Text.Encoding]::UTF8)
    Write-Host "Dosya basariyla duzeltildi."
    Write-Host "Temizlenen icerik uzunlugu: $($clean.Length) karakter"
} else {
    Write-Host "HATA: </Project> etiketi bulunamadi!"
}
