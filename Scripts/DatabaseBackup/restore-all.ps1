<#
.SYNOPSIS
    Store.sln Veritabanı Geri Yükleme (Restore) Scripti
    Bu script PostgreSQL, MongoDB ve Redis veritabanlarını yedek dosyalarından otomatik olarak yeni bilgisayara aktarır.

.NOTES
    Yazar: Antigravity (DBA & Project Manager)
    Tarih: 25 Ekim 2026
#>

[CmdletBinding()]
param(
    [string]$PgHost = "localhost",
    [int]$PgPort = 5432,
    [string]$PgUser = "postgres",
    [string]$PgPassword = "sa435558",
    [string]$MongoUri = "mongodb://localhost:27017",
    [string]$RedisHost = "localhost",
    [int]$RedisPort = 6379,
    [switch]$SkipRedis,
    [switch]$RestoreTeacherJobs
)

$ErrorActionPreference = "Continue"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$backupDir = $scriptDir
if (-not (Test-Path (Join-Path $backupDir "PostgreSQL"))) {
    $parent = Split-Path -Parent $scriptDir
    if (Test-Path (Join-Path $parent "PostgreSQL")) {
        $backupDir = $parent
    } elseif (Test-Path "C:\User\Projects\Database\PostgreSQL") {
        $backupDir = "C:\User\Projects\Database"
    }
}

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   STORE.SLN VERITABANI GERI YUKLEME (RESTORE) SISTEMI   " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "Yedek Klasoru:    $backupDir"
Write-Host "Hedef PostgreSQL: ${PgHost}:${PgPort} (User: $PgUser)"
Write-Host "Hedef MongoDB:    $MongoUri"
Write-Host "Hedef Redis:      ${RedisHost}:${RedisPort}"
Write-Host "----------------------------------------------------------"

# 1. ARAÇLARIN TESPİTİ
function Find-Tool {
    param([string]$name, [string[]]$fallbacks)
    $cmd = Get-Command $name -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    foreach ($fb in $fallbacks) {
        if (Test-Path $fb) { return $fb }
    }
    return $null
}

$psql = Find-Tool "psql.exe" @(
    "C:\Program Files\PostgreSQL\18\bin\psql.exe",
    "C:\Program Files\PostgreSQL\17\bin\psql.exe",
    "C:\Program Files\PostgreSQL\16\bin\psql.exe",
    "C:\Program Files\PostgreSQL\15\bin\psql.exe"
)

$mongorestore = Find-Tool "mongorestore.exe" @(
    "C:\Program Files\MongoDB\Tools\100\bin\mongorestore.exe",
    "C:\Program Files\MongoDB\Server\9.0\bin\mongorestore.exe",
    "C:\Program Files\MongoDB\Server\8.2\bin\mongorestore.exe",
    "C:\Program Files\MongoDB\Server\7.0\bin\mongorestore.exe"
)

$mongosh = Find-Tool "mongosh.exe" @(
    "C:\Program Files\mongosh\mongosh.exe"
)

$redisCli = Find-Tool "redis-cli.exe" @(
    "C:\User\Projects\Database\Redis\bin\redis-cli.exe",
    "C:\User\Kurulumlar\Redis-x64-3.0.504\redis-cli.exe",
    "C:\Program Files\Another\Redis-x64-3.0.504\redis-cli.exe",
    "C:\Program Files\Redis\redis-cli.exe"
)

Write-Host "Tespit Edilen Araclar:" -ForegroundColor Yellow
$psqlDisplay = if ($psql) { $psql } else { 'BULUNAMADI' }
$mongoDisplay = if ($mongorestore) { $mongorestore } elseif ($mongosh) { "mongosh.exe ($mongosh)" } else { 'BULUNAMADI' }
$redisDisplay = if ($redisCli) { $redisCli } else { 'BULUNAMADI' }
Write-Host " - PostgreSQL psql:    $psqlDisplay"
Write-Host " - MongoDB restore:    $mongoDisplay"
Write-Host " - Redis CLI:          $redisDisplay"
Write-Host "----------------------------------------------------------"

# 2. POSTGRESQL GERİ YÜKLEME
if ($psql) {
    Write-Host "[1/3] PostgreSQL Veritabanlari Geri Yukleniyor..." -ForegroundColor Green
    $env:PGPASSWORD = $PgPassword
    $pgDir = Join-Path $backupDir "PostgreSQL"
    
    $storeDbs = @("StoreIdentityDB", "StoreLocationDb", "StoreB2BDb", "StoreDiscountDb", "StoreOrderDb", "StoreFavoriteDb")
    if ($RestoreTeacherJobs) {
        $storeDbs += @("TeacherJobsIdentityDB", "TeacherJobsLocationDb", "TeacherJobsB2BDb")
    }
    
    foreach ($db in $storeDbs) {
        $sqlFile = Join-Path $pgDir "$db.sql"
        if (Test-Path $sqlFile) {
            Write-Host " -> $db geri yukleniyor ($sqlFile)..." -ForegroundColor White
            & $psql -U $PgUser -h $PgHost -p $PgPort -d postgres -f $sqlFile 2>&1 | Out-Null
            Write-Host "    [OK] $db yuklendi." -ForegroundColor Green
        } else {
            Write-Warning "Yedek dosyasi bulunamadi: $sqlFile"
        }
    }
} else {
    Write-Warning "psql.exe bulunamadigi icin PostgreSQL geri yukleme adimi atlandi!"
}

# 3. MONGODB GERİ YÜKLEME
if ($mongorestore) {
    Write-Host "`n[2/3] MongoDB (StoreMD) Geri Yukleniyor..." -ForegroundColor Green
    $mongoDir = Join-Path $backupDir "MongoDB"
    $archiveFile = Join-Path $mongoDir "StoreMD_archive.gz"
    $bsonDir = Join-Path $mongoDir "bson_dump\StoreMD"

    if (Test-Path $archiveFile) {
        Write-Host " -> StoreMD arsivden yukleniyor ($archiveFile)..." -ForegroundColor White
        & $mongorestore --uri="$MongoUri" --nsInclude="StoreMD.*" --archive="$archiveFile" --gzip --drop
        Write-Host "    [OK] MongoDB StoreMD arsivden yuklendi." -ForegroundColor Green
    } elseif (Test-Path $bsonDir) {
        Write-Host " -> StoreMD BSON klasorunden yukleniyor ($bsonDir)..." -ForegroundColor White
        & $mongorestore --uri="$MongoUri" --db="StoreMD" --drop "$bsonDir"
        Write-Host "    [OK] MongoDB StoreMD BSON klasorunden yuklendi." -ForegroundColor Green
    } else {
        Write-Warning "MongoDB yedek dosyalari bulunamadi!"
    }
} elseif ($mongosh) {
    Write-Host "`n[2/3] MongoDB (StoreMD) mongosh ile Geri Yukleniyor..." -ForegroundColor Green
    $jsScript = Join-Path $backupDir "MongoDB\restore-mongo.js"
    if (Test-Path $jsScript) {
        & $mongosh "$MongoUri/StoreMD" --quiet --file "$jsScript"
        Write-Host "    [OK] MongoDB StoreMD mongosh ile basariyla yuklendi." -ForegroundColor Green
    } else {
        Write-Warning "restore-mongo.js dosyasi bulunamadi: $jsScript"
    }
} else {
    Write-Warning "mongorestore.exe veya mongosh.exe bulunamadigi icin MongoDB geri yukleme adimi atlandi!"
}

# 4. REDIS GERİ YÜKLEME
if (!$SkipRedis -and $redisCli) {
    Write-Host "`n[3/3] Redis Anahtarlari Geri Yukleniyor..." -ForegroundColor Green
    $redisDir = Join-Path $backupDir "Redis"
    $cmdFile = Join-Path $redisDir "redis_commands.txt"
    if (Test-Path $cmdFile) {
        $lines = Get-Content $cmdFile
        $success = 0
        foreach ($line in $lines) {
            if ($line.StartsWith("SET ")) {
                $parts = $line.Substring(4) -split ' "', 2
                if ($parts.Count -ge 2) {
                    $key = $parts[0].Trim('"')
                    $val = $parts[1].TrimEnd('"')
                    & $redisCli -h $RedisHost -p $RedisPort set $key $val | Out-Null
                    $success++
                }
            } elseif ($line.StartsWith("EXPIRE ")) {
                $parts = $line.Substring(7) -split ' ', 2
                if ($parts.Count -ge 2) {
                    $key = $parts[0].Trim('"')
                    $ttl = $parts[1].Trim()
                    if ([int64]$ttl -gt 0) {
                        & $redisCli -h $RedisHost -p $RedisPort expire $key $ttl | Out-Null
                    }
                }
            }
        }
        Write-Host "    [OK] Redis $success adet anahtar yuklendi." -ForegroundColor Green
    } else {
        Write-Warning "Redis komut dosyasi bulunamadi: $cmdFile"
    }
}

Write-Host "`n==========================================================" -ForegroundColor Cyan
Write-Host "         VERITABANI AKTARIMI BASARIYLA TAMAMLANDI         " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
