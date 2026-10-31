$files = @(
    "c:\User\Projects\Store\FrontEnds\Store.WebUI\Views\Shared\_ProductCardPartial.cshtml",
    "c:\User\Projects\Store\FrontEnds\Store.WebUI\Views\Shared\Components\_BestSellersDefaultComponentPartial\Default.cshtml",
    "c:\User\Projects\Store\FrontEnds\Store.WebUI\Views\Shared\Components\_NewProductsDefaultComponentPartial\Default.cshtml",
    "c:\User\Projects\Store\FrontEnds\Store.WebUI\Views\Shared\Components\_TrendsDefaultComponentPartial\Default.cshtml"
)
$count = 0
foreach ($file in $files) {
    if (Test-Path $file) {
        $content = [System.IO.File]::ReadAllText($file)
        
        $pattern = 'data-tippy-content="<i>Hızlı Bakış</i>"'
        $replacement = 'data-tippy-content="<i style=''color: #ffffff;''>Hızlı Bakış</i>"'
        
        if ($content.Contains('data-tippy-content="<i>Hızlı Bakış</i>"')) {
            Write-Host "Updating $(Split-Path $file -Leaf)"
            $count++
            
            $content = $content.Replace('data-tippy-content="<i>Hızlı Bakış</i>"', "data-tippy-content=`"<i style='color: #ffffff;'>Hızlı Bakış</i>`"")
            
            [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
        }
    }
}
Write-Host "Total files updated: $count"