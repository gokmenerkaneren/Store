# STORE.SLN - TAM VERİTABANI YEDEKLEME SCRİPTİ (BACKUP-ALL.PS1)
# ==========================================================
# Tarih: 25 Ekim 2026
# Hazırlayan: Agent Gravity & DBA Agent
# Amaç: Store.sln veritabanlarının C:\User\Projects\Database altına eksiksiz yedeklenmesi

[CmdletBinding()]
param(
    [string]$TargetDir = "C:\User\Projects\Database",
    [string]$PgUser = "postgres",
    [string]$PgPassword = "sa435558",
    [string]$PgHost = "localhost",
    [int]$PgPort = 5432,
    [string]$MongoUri = "mongodb://localhost:27017",
    [string]$MongoDb = "StoreMD",
    [string]$RedisHost = "localhost",
    [int]$RedisPort = 6379
)

$ErrorActionPreference = "Continue"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   STORE.SLN TUM VERITABANLARI YEDEKLEME ISLEMI BASLADI   " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "Hedef Dizin:       $TargetDir"
Write-Host "PostgreSQL Hedef:  ${PgHost}:${PgPort} (User: $PgUser)"
Write-Host "MongoDB Hedef:     $MongoUri ($MongoDb)"
Write-Host "Redis Hedef:       ${RedisHost}:${RedisPort}"
Write-Host "----------------------------------------------------------"

# Dizinleri Güvenceye Al
$pgTarget = Join-Path $TargetDir "PostgreSQL"
$mongoTarget = Join-Path $TargetDir "MongoDB"
$redisTarget = Join-Path $TargetDir "Redis"
$scriptsTarget = Join-Path $TargetDir "Scripts"

@($pgTarget, $mongoTarget, $redisTarget, $scriptsTarget) | ForEach-Object {
    if (-not (Test-Path $_)) { New-Item -ItemType Directory -Force -Path $_ | Out-Null }
}

# -------------------------------------------------------------
# 1. POSTGRESQL YEDEKLEME
# -------------------------------------------------------------
Write-Host "`n[1/3] POSTGRESQL YEDEKLERI ALINIYOR..." -ForegroundColor Green
$env:PGPASSWORD = $PgPassword
$pgDump = "C:\Program Files\PostgreSQL\17\bin\pg_dump.exe"
$pgDumpAll = "C:\Program Files\PostgreSQL\17\bin\pg_dumpall.exe"
$psql = "C:\Program Files\PostgreSQL\17\bin\psql.exe"

$pgDatabases = @(
    "StoreIdentityDB",
    "StoreLocationDb",
    "StoreB2BDb",
    "StoreDiscountDb",
    "StoreFavoriteDb",
    "StoreOrderDb",
    "TeacherJobsIdentityDB",
    "TeacherJobsLocationDb",
    "TeacherJobsB2BDb"
)

# Tek tek veritabanı yedekleri (Hem .sql hem .dump formatında)
foreach ($db in $pgDatabases) {
    Write-Host "  -> $db yedekleniyor..." -ForegroundColor Yellow
    $sqlFile = Join-Path $pgTarget "$db.sql"
    $dumpFile = Join-Path $pgTarget "$db.dump"

    # SQL formatı (--clean --if-exists --create --encoding=UTF8)
    & $pgDump -U $PgUser -h $PgHost -p $PgPort --clean --if-exists --create --encoding=UTF8 -f $sqlFile $db 2>&1 | Out-Null
    
    # Custom dump formatı (-F c)
    & $pgDump -U $PgUser -h $PgHost -p $PgPort -F c -f $dumpFile $db 2>&1 | Out-Null

    if (Test-Path $sqlFile) {
        $size = (Get-Item $sqlFile).Length / 1KB
        Write-Host "     [OK] $db.sql olusturuldu ($([math]::Round($size, 2)) KB)" -ForegroundColor DarkGreen
    }
    if (Test-Path $dumpFile) {
        $size = (Get-Item $dumpFile).Length / 1KB
        Write-Host "     [OK] $db.dump olusturuldu ($([math]::Round($size, 2)) KB)" -ForegroundColor DarkGreen
    }
}

# Cluster seviyesi tüm kullanıcılar ve veritabanları yedeği
Write-Host "  -> PostgreSQL Cluster (all_databases_cluster.sql) yedekleniyor..." -ForegroundColor Yellow
$clusterSql = Join-Path $pgTarget "all_databases_cluster.sql"
& $pgDumpAll -U $PgUser -h $PgHost -p $PgPort --clean --if-exists -f $clusterSql 2>&1 | Out-Null
if (Test-Path $clusterSql) {
    $size = (Get-Item $clusterSql).Length / 1KB
    Write-Host "     [OK] all_databases_cluster.sql olusturuldu ($([math]::Round($size, 2)) KB)" -ForegroundColor DarkGreen
}

# -------------------------------------------------------------
# 2. MONGODB YEDEKLEME
# -------------------------------------------------------------
Write-Host "`n[2/3] MONGODB ($MongoDb) YEDEKLERI ALINIYOR..." -ForegroundColor Green
$mongodump = "C:\Program Files\MongoDB\Tools\100\bin\mongodump.exe"
$mongoexport = "C:\Program Files\MongoDB\Tools\100\bin\mongoexport.exe"
$mongosh = "C:\Program Files\mongosh\mongosh.exe"

# 2.1 Sıkıştırılmış Arşiv (StoreMD_archive.gz)
$archiveFile = Join-Path $mongoTarget "$($MongoDb)_archive.gz"
Write-Host "  -> $MongoDb sikistirilmis mongodump arsivi aliniyor..." -ForegroundColor Yellow
& $mongodump --uri="$MongoUri" --db=$MongoDb --archive="$archiveFile" --gzip 2>&1 | Out-Null
if (Test-Path $archiveFile) {
    $size = (Get-Item $archiveFile).Length / 1KB
    Write-Host "     [OK] $($MongoDb)_archive.gz olusturuldu ($([math]::Round($size, 2)) KB)" -ForegroundColor DarkGreen
}

# 2.2 Koleksiyon Bazlı BSON Dump
$bsonOutDir = Join-Path $mongoTarget "bson_dump"
Write-Host "  -> BSON formatinda koleksiyon dump'i cikariliyor..." -ForegroundColor Yellow
& $mongodump --uri="$MongoUri" --db=$MongoDb --out="$bsonOutDir" 2>&1 | Out-Null
if (Test-Path (Join-Path $bsonOutDir $MongoDb)) {
    Write-Host "     [OK] bson_dump\$MongoDb klasorune cikarildi." -ForegroundColor DarkGreen
}

# 2.3 Koleksiyon Bazlı JSON Dışa Aktarımı
$jsonOutDir = Join-Path $mongoTarget "json_exports"
if (-not (Test-Path $jsonOutDir)) { New-Item -ItemType Directory -Force -Path $jsonOutDir | Out-Null }
Write-Host "  -> Koleksiyonlar teftis icin JSON olarak export ediliyor..." -ForegroundColor Yellow

$collectionsJson = & $mongosh "$MongoUri/$MongoDb" --quiet --eval "JSON.stringify(db.getCollectionNames())"
$collections = $collectionsJson | ConvertFrom-Json

foreach ($col in $collections) {
    $colJsonPath = Join-Path $jsonOutDir "$col.json"
    & $mongoexport --uri="$MongoUri" --db=$MongoDb --collection=$col --out="$colJsonPath" --jsonArray --pretty 2>&1 | Out-Null
    if (Test-Path $colJsonPath) {
        $count = (Get-Item $colJsonPath).Length / 1KB
        Write-Host "     [OK] $col.json export edildi ($([math]::Round($count, 2)) KB)" -ForegroundColor DarkGreen
    }
}

# -------------------------------------------------------------
# 3. REDIS YEDEKLEME
# -------------------------------------------------------------
Write-Host "`n[3/3] REDIS (SEPET VERILERI) YEDEKLENIYOR..." -ForegroundColor Green
$redisCli = "C:\Program Files\Another\Redis-x64-3.0.504\redis-cli.exe"
$redisBinDir = "C:\Program Files\Another\Redis-x64-3.0.504"

# 3.1 Redis SAVE Tetiklemesi ve dump.rdb Kopyalama
Write-Host "  -> Redis SAVE komutu veriliyor..." -ForegroundColor Yellow
& $redisCli -h $RedisHost -p $RedisPort SAVE 2>&1 | Out-Null
Start-Sleep -Seconds 1

$srcRdb = Join-Path $redisBinDir "dump.rdb"
$destRdb = Join-Path $redisTarget "dump.rdb"
if (Test-Path $srcRdb) {
    Copy-Item -Path $srcRdb -Destination $destRdb -Force
    $size = (Get-Item $destRdb).Length / 1KB
    Write-Host "     [OK] dump.rdb kopyalandi ($([math]::Round($size, 2)) KB)" -ForegroundColor DarkGreen
}

# 3.2 Redis Anahtarlarını JSON ve TXT Komut Listesi Olarak Çıkarma
$keysRaw = & $redisCli -h $RedisHost -p $RedisPort keys "*"
$keys = $keysRaw | Where-Object { $_ -and -not $_.StartsWith("ERR") }

$redisCommandsFile = Join-Path $redisTarget "redis_commands.txt"
$redisJsonFile = Join-Path $redisTarget "redis_data.json"

$commands = @()
$dataList = @()

foreach ($key in $keys) {
    $keyTrim = $key.Trim()
    $val = & $redisCli -h $RedisHost -p $RedisPort get $keyTrim
    $ttl = & $redisCli -h $RedisHost -p $RedisPort ttl $keyTrim
    
    $commands += "SET `"$keyTrim`" `"$val`""
    if ($ttl -gt 0) {
        $commands += "EXPIRE `"$keyTrim`" $ttl"
    }

    $dataList += [PSCustomObject]@{
        Key = $keyTrim
        Value = $val
        TTL = $ttl
    }
}

$commands | Out-File -FilePath $redisCommandsFile -Encoding utf8
$dataList | ConvertTo-Json -Depth 5 | Out-File -FilePath $redisJsonFile -Encoding utf8

Write-Host "     [OK] $($keys.Count) adet sepet anahtari redis_commands.txt ve redis_data.json dosyasina yazildi." -ForegroundColor DarkGreen

# -------------------------------------------------------------
# 4. KOPYA VE TAMAMLAMA
# -------------------------------------------------------------
Write-Host "`n==========================================================" -ForegroundColor Cyan
Write-Host "           TUM YEDEKLEME ISLEMLERI TAMAMLANDI!            " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
