# STORE.SLN - LOKAL GELİŞTİRME ORTAMI BAŞLATMA SCRİPTİ
# ======================================================

Write-Host "Veritabanı servisleri kontrol ediliyor..." -ForegroundColor Cyan

# Port Kontrolleri
$pg = Test-NetConnection -ComputerName localhost -Port 5432 -InformationLevel Quiet
$mg = Test-NetConnection -ComputerName localhost -Port 27017 -InformationLevel Quiet
$rd = Test-NetConnection -ComputerName localhost -Port 6379 -InformationLevel Quiet

Write-Host "PostgreSQL (5432): $(if ($pg) {'[AYAKTA]'} else {'[KAPALI]'})" -ForegroundColor $(if ($pg) {'Green'} else {'Red'})
Write-Host "MongoDB (27017):   $(if ($mg) {'[AYAKTA]'} else {'[KAPALI]'})" -ForegroundColor $(if ($mg) {'Green'} else {'Red'})
Write-Host "Redis (6379):      $(if ($rd) {'[AYAKTA]'} else {'[KAPALI]'})" -ForegroundColor $(if ($rd) {'Green'} else {'Red'})

if (-not ($pg -and $mg -and $rd)) {
    Write-Warning "Bazı veritabanları kapalı! Lütfen ilgili servisleri başlatın."
}

$projects = @(
    @{ Name = "IdentityServer"; Path = "IdentityServer\Store.IdentityServer\Store.IdentityServer.csproj" },
    @{ Name = "Catalog";        Path = "Services\Catalog\Store.Catalog\Store.Catalog.csproj" },
    @{ Name = "Discount";       Path = "Services\Discount\Store.Discount\Store.Discount.csproj" },
    @{ Name = "Location";       Path = "Services\Location\Presentation\Store.Location.WebApi\Store.Location.WebApi.csproj" },
    @{ Name = "Order";          Path = "Services\Order\Presentation\Store.Order.WebApi\Store.Order.WebApi.csproj" },
    @{ Name = "Basket";         Path = "Services\Basket\Store.Basket\Store.Basket.csproj" },
    @{ Name = "Favorite";       Path = "Services\Favorite\Presentation\Store.Favorite.WebApi\Store.Favorite.WebApi.csproj" },
    @{ Name = "Rental";         Path = "Services\Rental\Presentation\Store.Rental.WebApi\Store.Rental.WebApi.csproj" },
    @{ Name = "WebUI";          Path = "FrontEnds\Store.WebUI\Store.WebUI.csproj" }
)

Write-Host "`nProjeler ayrı terminallerde başlatılıyor..." -ForegroundColor Yellow

$rootDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
if (-not $rootDir) { $rootDir = Get-Location }
if (-not (Test-Path (Join-Path $rootDir "Store.slnx"))) {
    $parent = Split-Path -Parent $rootDir
    if (Test-Path (Join-Path $parent "Store.slnx")) {
        $rootDir = $parent
    } elseif (Test-Path "C:\User\Projects\Store\Store.slnx") {
        $rootDir = "C:\User\Projects\Store"
    }
}

foreach ($proj in $projects) {
    $fullPath = Join-Path $rootDir $proj.Path
    Write-Host "-> Başlatılıyor: $($proj.Name)..." -ForegroundColor White
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "Write-Host '=== $($proj.Name) ===' -ForegroundColor Cyan; cd '$rootDir'; dotnet run --project '$fullPath'"
}

Write-Host "`nTüm projeler başlatıldı!" -ForegroundColor Green
Write-Host "WebUI: http://localhost:5079" -ForegroundColor Cyan
Write-Host "IdentityServer: http://localhost:5001" -ForegroundColor Cyan
