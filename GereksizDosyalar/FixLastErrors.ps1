$file1 = 'C:\User\Projects\Store\Services\Catalog\Store.Catalog\Controllers\ProductImagesController.cs'
$text1 = [System.IO.File]::ReadAllText($file1)
$text1 = $text1 -replace 'public async Task<IActionResult> SetMainImage\(\[FromQuery\] string imageId, \[FromQuery\] Guid productId\)', 'public async Task<IActionResult> SetMainImage([FromQuery] Guid imageId, [FromQuery] Guid productId)'
[System.IO.File]::WriteAllText($file1, $text1)

$file2 = 'C:\User\Projects\Store\Services\Catalog\Store.Catalog\Services\CategoryServices\CategoryService.cs'
$text2 = [System.IO.File]::ReadAllText($file2)
$text2 = $text2 -replace 'var parentIdStr = updateCategoryDto\.ParentId \?\? existing\.ParentId\?\.ToString\(\);(?:\r?\n.*?){3}\s*\{', 'var parentGuid = updateCategoryDto.ParentId ?? existing.ParentId;
                if (parentGuid.HasValue && parentGuid.Value != Guid.Empty)
                {'
[System.IO.File]::WriteAllText($file2, $text2)

