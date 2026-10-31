$file1 = 'C:\User\Projects\Store\Services\Catalog\Store.Catalog\Services\ProductImageServices\ProductImageService.cs'
$text1 = [System.IO.File]::ReadAllText($file1)
$text1 = $text1 -replace 'GetByProductIdProductImageAsync\(string productId\)', 'GetByProductIdProductImageAsync(Guid productId)'
[System.IO.File]::WriteAllText($file1, $text1)

$file2 = 'C:\User\Projects\Store\Services\Catalog\Store.Catalog\Services\ProductImageServices\IProductImageService.cs'
$text2 = [System.IO.File]::ReadAllText($file2)
$text2 = $text2 -replace 'GetByProductIdProductImageAsync\(string productId\)', 'GetByProductIdProductImageAsync(Guid productId)'
[System.IO.File]::WriteAllText($file2, $text2)

$file3 = 'C:\User\Projects\Store\Services\Catalog\Store.Catalog\Controllers\ProductImagesController.cs'
$text3 = [System.IO.File]::ReadAllText($file3)
$text3 = $text3 -replace 'GetByProductIdProductImageAsync\(string id\)', 'GetByProductIdProductImageAsync(Guid id)'
[System.IO.File]::WriteAllText($file3, $text3)

$file4 = 'C:\User\Projects\Store\Services\Catalog\Store.Catalog\Controllers\ProductPricesController.cs'
$text4 = [System.IO.File]::ReadAllText($file4)
$text4 = $text4 -replace 'GetByProductId\(string id', 'GetByProductId(Guid id'
[System.IO.File]::WriteAllText($file4, $text4)
