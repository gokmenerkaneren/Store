$files = @(
    "c:\User\Projects\Store\FrontEnds\Store.WebUI\Views\Shared\Components\_BestSellersDefaultComponentPartial\Default.cshtml",
    "c:\User\Projects\Store\FrontEnds\Store.WebUI\Views\Shared\Components\_NewProductsDefaultComponentPartial\Default.cshtml",
    "c:\User\Projects\Store\FrontEnds\Store.WebUI\Views\Shared\Components\_TrendsDefaultComponentPartial\Default.cshtml"
)
foreach ($file in $files) {
    if (Test-Path $file) {
        $content = [System.IO.File]::ReadAllText($file)
        
        # Add flex-column if missing
        $content = $content -replace 'd-lg-flex mb-lg-10', 'd-lg-flex flex-column mb-lg-10'
        
        # Inject wishlist button if it doesn't exist
        if (-not $content.Contains('button-wishlist')) {
            $wishlistHtml = @"
<a href="javascript:void(0);" 
   class="button-wishlist d-flex flex-center rounded-circle js-btn-favorite mb-5"
   data-product-id="@product.Id"
   data-js-tooltip
   data-tippy-content="Favorilere Ekle"
   data-tippy-theme="large"
   data-tippy-placement="left"
   data-tippy-distance="5">
    <i class="icon-heart-empty">
        <svg aria-hidden="true" focusable="false" role="presentation" class="icon custom-heart-icon" viewBox="0 0 24 24" width="16" height="16">
            <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path>
        </svg>
    </i>
</a>
"@
            $content = $content -replace '<a href="/Product/QuickView/', ($wishlistHtml + "`r`n`t`t`t`t`t`t`t`t`t`t`t`t`t`t`t`t<a href=`"/Product/QuickView/")
            Write-Host "Injected button-wishlist into $(Split-Path $file -Leaf)"
        }
        
        [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
    }
}