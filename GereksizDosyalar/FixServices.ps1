$dirs = @(
    'C:\User\Projects\Store\Services\Catalog',
    'C:\User\Projects\Store\FrontEnds\Store.WebUI'
)

foreach ($dir in $dirs) {
    $files = Get-ChildItem -Path $dir -Filter '*.cs' -Recurse
    foreach ($file in $files) {
        $text = [System.IO.File]::ReadAllText($file.FullName)
        
        # Replace Guid.Parse(guid) or Guid.Parse(dto.Id)
        $newText = [regex]::Replace($text, 'Guid\.Parse\(([A-Za-z0-9_\.]+Id)\)', '$1')
        $newText = [regex]::Replace($newText, 'Guid\.Parse\((id)\)', '$1')
        $newText = [regex]::Replace($newText, 'Guid\.Parse\((ParentId)\)', '$1')

        # Replace (string id) with (Guid id) in Controllers and Services
        $newText = [regex]::Replace($newText, 'public\s+(async\s+)?(Task(?:<IActionResult>|<[^>]+>)?|IActionResult|void)\s+([A-Za-z0-9_]+)\s*\((.*?)\bstring\s+(id|productId|categoryId|brandId)\b(.*?)\)', 'public $1$2 $3($4Guid $5$6)')
        
        # Replace in IService interfaces
        $newText = [regex]::Replace($newText, '(Task(?:<[^>]+>)?)\s+([A-Za-z0-9_]+)\s*\((.*?)\bstring\s+(id|productId|categoryId|brandId)\b(.*?)\)', '$1 $2($3Guid $4$5)')

        if ($text -ne $newText) {
            [System.IO.File]::WriteAllText($file.FullName, $newText)
            Write-Host "Updated: $($file.FullName)"
        }
    }
}
