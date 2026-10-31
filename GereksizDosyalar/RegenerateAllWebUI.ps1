$entities = @("Branch", "BranchAddress", "Company", "ContactInfo", "Dealer", "UserAddress", "Warehouse", "District", "Neighborhood")
$dtoPath = "c:\User\Projects\Store\Store.DtoLayer\LocationDtos"
$controllersPath = "c:\User\Projects\Store\FrontEnds\Store.WebUI\Areas\Admin\Controllers"
$viewsPath = "c:\User\Projects\Store\FrontEnds\Store.WebUI\Areas\Admin\Views"

foreach ($entity in $entities) {
    $resultDtoPath = "$dtoPath\$($entity)Dtos\Result$($entity)Dto.cs"
    if (-not (Test-Path $resultDtoPath)) { continue }

    $dtoContent = Get-Content -Path $resultDtoPath -Raw
    $matches = [regex]::Matches($dtoContent, "(?ms)^\s*public\s+([A-Za-z0-9_<>?\[\]]+)\s+([A-Za-z0-9_]+)\s*\{\s*get;\s*set;\s*\}")
    
    $props = @()
    foreach ($m in $matches) {
        $props += @{ Type = $m.Groups[1].Value; Name = $m.Groups[2].Value }
    }

    $controllerContent = @"
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store.DtoLayer.LocationDtos.$($entity)Dtos;
using System.Collections.Generic;
using System.Text;

namespace Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class $($entity)Controller : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public $($entity)Controller(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            var client = _httpClientFactory.CreateClient();
            var url = `"https://localhost:7303/api/$($entity)?showDeleted=`" + showDeleted.ToString().ToLower();
            var response = await client.GetAsync(url);
            ViewBag.ShowDeleted = showDeleted;
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<Result$($entity)Dto>>(json);
                return View(values);
            }
            return View(new List<Result$($entity)Dto>());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create$($entity)Dto dto)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7303/api/$($entity)", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = err });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Update$($entity)Dto dto)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("https://localhost:7303/api/$($entity)", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true });
            var err = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = err });
        }

        public async Task<IActionResult> Delete$($entity)(Guid id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7303/api/$($entity)/soft-delete/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
"@
    [System.IO.File]::WriteAllText("$controllersPath\$($entity)Controller.cs", $controllerContent, [System.Text.Encoding]::UTF8)

    $entityViewPath = "$viewsPath\$entity"
    if (-not (Test-Path $entityViewPath)) {
        New-Item -ItemType Directory -Path $entityViewPath | Out-Null
    }

    $tableHeaders = ""
    $tableRows = ""
    $createInputs = ""
    $editInputs = ""
    $detailRows = ""
    $editJsAssignments = ""
    $dataAttributes = ""
    
    foreach ($p in $props) {
        $n = $p.Name
        $t = $p.Type
        $lowerN = $n.Substring(0,1).ToLower() + $n.Substring(1)
        
        $dataAttributes += " data-$lowerN=`"@item.$n`""

        if ($n -notin @("Id", "IsDeleted", "DeleteUserId", "DeleteDate", "InsertUserId", "UpdateUserId")) {
            $tableHeaders += "                        <th>$n</th>`r`n"
            if ($t -eq "bool") {
                $tableRows += "                                <td><span class=`"label label-@(item.$n ? `"success`" : `"danger`")`">@(item.$n ? `"Evet`" : `"Hayır`")</span></td>`r`n"
            } else {
                $tableRows += "                                <td>@item.$n</td>`r`n"
            }
        }
        
        $detailRows += "                        <div class=`"col-sm-6`"><strong>$($n):</strong> <p id=`"det-$lowerN`" class=`"text-muted`"></p></div>`r`n"
        
        if ($n -notin @("Id", "InsertUserId", "InsertDate", "UpdateUserId", "UpdateDate", "IsDeleted", "DeleteUserId", "DeleteDate")) {
            if ($t -eq "bool") {
                $createInputs += "                        <div class=`"form-group`">
                            <label>$n</label>
                            <select class=`"form-control`" name=`"$n`">
                                <option value=`"true`">Evet</option>
                                <option value=`"false`">Hayır</option>
                            </select>
                        </div>`r`n"
                $editInputs += "                        <div class=`"form-group`">
                            <label>$n</label>
                            <select class=`"form-control`" name=`"$n`" id=`"edit-$lowerN`">
                                <option value=`"true`">Evet</option>
                                <option value=`"false`">Hayır</option>
                            </select>
                        </div>`r`n"
            } else {
                $createInputs += "                        <div class=`"form-group`">
                            <label>$n</label>
                            <input type=`"text`" class=`"form-control`" name=`"$n`" required />
                        </div>`r`n"
                $editInputs += "                        <div class=`"form-group`">
                            <label>$n</label>
                            <input type=`"text`" class=`"form-control`" name=`"$n`" id=`"edit-$lowerN`" required />
                        </div>`r`n"
            }
        }

        if ($t -eq "bool") {
            $editJsAssignments += "        var $lowerN = button.data('$lowerN') === 'True' || button.data('$lowerN') === true || button.data('$lowerN') === 'true';
        modal.find('#edit-$lowerN').val($lowerN.toString());
        modal.find('#det-$lowerN').text($lowerN ? 'Evet' : 'Hayır');`r`n"
        } else {
            $editJsAssignments += "        modal.find('#edit-$lowerN').val(button.data('$lowerN'));
        modal.find('#det-$lowerN').text(button.data('$lowerN'));`r`n"
        }
    }

    $viewContent = @"
@model List<Store.DtoLayer.LocationDtos.$($entity)Dtos.Result$($entity)Dto>
@{
    ViewData["Title"] = "$entity Listesi";
    Layout = "~/Areas/Admin/Views/AdminLayout/Index.cshtml";
}

<h2 style="color:#007bff; font-weight:bold;">$entity İşlemleri</h2>
<div class="card">
    <div class="card-block">
        <div class="dt-responsive table-responsive">
            <table id="basic-btn" class="table table-striped table-bordered nowrap">
                <thead>
                    <tr>
$tableHeaders                        <th>İşlemler</th>
                    </tr>
                </thead>
                <tbody>
                    @if (Model != null)
                    {
                        foreach (var item in Model)
                        {
                            <tr style="@(item.IsDeleted ? "opacity:0.5;background-color:#f8d7da;" : "")">
$tableRows                                <td>
                                    <div class="btn-group btn-group-sm">
                                        <button type="button" class="btn btn-grd-info waves-effect waves-light" style="float: none;margin: 5px;"
                                                data-toggle="modal" data-target="#editModal"
                                                $dataAttributes>
                                            <span class="feather icon-edit"></span>
                                        </button>
                                        <button type="button" class="btn btn-grd-success waves-effect waves-light" style="float: none;margin: 5px;"
                                                data-toggle="modal" data-target="#detailsModal"
                                                $dataAttributes>
                                            <i class="icofont icofont-file-document"></i>
                                        </button>
                                        <a href="/Admin/$($entity)/Delete$($entity)/@item.Id" class="btn btn-grd-danger waves-effect waves-light" style="float: none;margin: 5px;" onclick="return confirm('Silmek istediğinize emin misiniz?')">
                                            <span class="feather icon-trash"></span>
                                        </a>
                                    </div>
                                </td>
                            </tr>
                        }
                    }
                </tbody>
            </table>
            <button type="button" class="btn btn-grd-primary waves-effect waves-light mt-3" data-toggle="modal" data-target="#createModal">
                Yeni $entity Ekle
            </button>
            <a href="/Admin/$($entity)/Index?showDeleted=@(ViewBag.ShowDeleted != null && ViewBag.ShowDeleted ? "false" : "true")" class="btn btn-grd-warning waves-effect waves-light mt-3">
                @(ViewBag.ShowDeleted != null && ViewBag.ShowDeleted ? "Aktif Kayıtları Göster" : "Silinmiş Kayıtları Göster")
            </a>
        </div>
    </div>
</div>

<!-- Create Modal -->
<div class="modal fade" id="createModal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <form id="createForm">
                <div class="modal-header">
                    <h5 class="modal-title">Yeni $entity Ekle</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
$createInputs                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-inverse" data-dismiss="modal">İptal</button>
                    <button type="button" class="btn btn-primary" onclick="submitCreate()">Kaydet</button>
                </div>
            </form>
        </div>
    </div>
</div>

<!-- Edit Modal -->
<div class="modal fade" id="editModal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <form id="editForm">
                <input type="hidden" name="Id" id="edit-id" />
                <div class="modal-header">
                    <h5 class="modal-title">$entity Güncelle</h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
$editInputs                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-inverse" data-dismiss="modal">İptal</button>
                    <button type="button" class="btn btn-info" onclick="submitEdit()">Güncelle</button>
                </div>
            </form>
        </div>
    </div>
</div>

<!-- Details Modal -->
<div class="modal fade" id="detailsModal" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header" style="background-color: #2ed8b6; color: white;">
                <h5 class="modal-title"><i class="feather icon-info"></i> $entity Detayları</h5>
                <button type="button" class="close" data-dismiss="modal" style="color: white;">&times;</button>
            </div>
            <div class="modal-body p-4">
                <div class="row">
$detailRows                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-inverse" data-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>

<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script>
    `$('#editModal').on('show.bs.modal', function (event) {
        var button = `$(`event.relatedTarget);
        var modal = `$(`this);
        modal.find('#edit-id').val(button.data('id'));
$editJsAssignments    });

    `$('#detailsModal').on('show.bs.modal', function (event) {
        var button = `$(`event.relatedTarget);
        var modal = `$(`this);
$editJsAssignments    });

    function submitCreate() {
        var data = {};
        `$('#createForm').serializeArray().map(function(x){data[x.name] = x.value;});
        `$.post('/Admin/$($entity)/Create', data, function (res) {
            if (res.success) location.reload();
            else alert('Hata: ' + res.message);
        });
    }

    function submitEdit() {
        var data = {};
        `$('#editForm').serializeArray().map(function(x){data[x.name] = x.value;});
        `$.post('/Admin/$($entity)/Edit', data, function (res) {
            if (res.success) location.reload();
            else alert('Hata: ' + res.message);
        });
    }
</script>
"@
    [System.IO.File]::WriteAllText("$entityViewPath\Index.cshtml", $viewContent, [System.Text.Encoding]::UTF8)
    Write-Host "Scaffolded $entity"
}
