$file1 = 'C:\User\Projects\Store\Services\Catalog\Store.Catalog\Controllers\ProductImagesController.cs'
$text1 = [System.IO.File]::ReadAllText($file1)
# The parameter is likely Guid now, but it's passing a string? Let's check line 104 manually.
# Actually let's use powershell to fix 3, 4, 5 first.

$file3 = 'C:\User\Projects\Store\FrontEnds\Store.WebUI\Areas\Admin\Controllers\BrandController.cs'
$text3 = [System.IO.File]::ReadAllText($file3)
$text3 = $text3 -replace 'Guid parentId = null', 'Guid? parentId = null'
$text3 = $text3 -replace 'Guid id = null', 'Guid? id = null'
[System.IO.File]::WriteAllText($file3, $text3)

$file4 = 'C:\User\Projects\Store\FrontEnds\Store.WebUI\Areas\Admin\Controllers\CategoryController.cs'
$text4 = [System.IO.File]::ReadAllText($file4)
$text4 = $text4 -replace 'Guid parentId = null', 'Guid? parentId = null'
$text4 = $text4 -replace 'Guid id = null', 'Guid? id = null'
[System.IO.File]::WriteAllText($file4, $text4)

$file5 = 'C:\User\Projects\Store\FrontEnds\Store.WebUI\Areas\Admin\Controllers\ProductController.cs'
$text5 = [System.IO.File]::ReadAllText($file5)
$text5 = $text5 -replace 'Guid parentId = null', 'Guid? parentId = null'
$text5 = $text5 -replace 'Guid id = null', 'Guid? id = null'
[System.IO.File]::WriteAllText($file5, $text5)

