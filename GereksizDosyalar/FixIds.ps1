$dirs = @(
    'C:\User\Projects\Store\Store.DtoLayer',
    'C:\User\Projects\Store\Services\Catalog'
)

foreach ($dir in $dirs) {
    $files = Get-ChildItem -Path $dir -Filter '*.cs' -Recurse
    foreach ($file in $files) {
        $text = [System.IO.File]::ReadAllText($file.FullName)
        
        # Replace public string [Prop]Id
        $newText = [regex]::Replace($text, 'public string(\??)\s+([A-Za-z0-9_]*Id)\b', 'public Guid$1 $2')
        
        # Replace initializers like = null!;
        $newText = [regex]::Replace($newText, 'public Guid(\??) ([A-Za-z0-9_]*Id)\s*\{\s*get;\s*set;\s*\}\s*=\s*(null!|string\.Empty|"";|""""|null);', 'public Guid$1 $2 { get; set; }')
        
        if ($text -ne $newText) {
            # If inherits BaseEntity, completely remove 'public Guid Id { get; set; }'
            if ([regex]::IsMatch($newText, ':\s*BaseEntity')) {
                $newText = [regex]::Replace($newText, '(?m)^\s*public Guid\s+Id\s*\{\s*get;\s*set;\s*\}.*\n?', "")
            }
            
            [System.IO.File]::WriteAllText($file.FullName, $newText)
            Write-Host "Updated: $($file.FullName)"
        }
    }
}
