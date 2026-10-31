import re

file_path = r'C:\User\Projects\Store\FrontEnds\Store.WebUI\Areas\Admin\Views\Product\CreateOrUpdateProduct.cshtml'
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Replace Table Header
old_header = r'<th><i class=\"icofont icofont-tag\"></i> Fiyat Tipi</th>\s*<th><i class=\"icofont icofont-money\"></i> Tutar</th>\s*<th><i class=\"icofont icofont-world\"></i> Para Birimi</th>\s*<th><i class=\"icofont icofont-calendar\"></i> Başlangıç</th>\s*<th><i class=\"icofont icofont-calendar\"></i> Bitiş</th>'
new_header = '''<th><i class=\"icofont icofont-ui-check\"></i> Durum</th>
                                                <th><i class=\"icofont icofont-calendar\"></i> Başlangıç</th>
                                                <th><i class=\"icofont icofont-calendar\"></i> Bitiş</th>
                                                <th><i class=\"icofont icofont-tag\"></i> Fiyat Tipi</th>
                                                <th><i class=\"icofont icofont-money\"></i> Tutar</th>
                                                <th><i class=\"icofont icofont-world\"></i> Para Birimi</th>
                                                <th><i class=\"icofont icofont-settings\"></i> İşlemler</th>'''
content = re.sub(old_header, new_header, content)

# Replace Table Body Row
old_row = r'<td><span class=\"badge @\(p\.PriceType == 1 \? \"badge-info\" : \"badge-warning\"\)\">@\(p\.PriceType == 1 \? \"Satış\" : \"Kiralama\"\)</span></td>\s*<td class=\"font-weight-bold text-dark\">@p\.Amount\.ToString\(\"N2\"\)</td>\s*@\{\s*var currencies = ViewBag\.Currencies as List<Microsoft\.AspNetCore\.Mvc\.Rendering\.SelectListItem>;\s*var currencyName = currencies\?\.FirstOrDefault\(c => string\.Equals\(c\.Value, p\.CurrencyId\.ToString\(\), StringComparison\.OrdinalIgnoreCase\)\)\?\.Text \?\? p\.CurrencyId\.ToString\(\);\s*\}\s*<td>@currencyName</td>\s*<td>@\(p\.ValidFrom\?\.ToString\(\"dd\.MM\.yyyy - HH:mm:ss\"\) \?\? \"-\"\)</td>\s*<td>@\(p\.ValidTo\?\.ToString\(\"dd\.MM\.yyyy - HH:mm:ss\"\) \?\? \"-\"\)</td>'

new_row = '''<td><span class=\"badge @(p.IsActive ? \"badge-success\" : \"badge-danger\")\">@(p.IsActive ? \"Aktif\" : \"Pasif\")</span></td>
                                                        <td>@(p.ValidFrom?.ToString(\"dd.MM.yyyy - HH:mm:ss\") ?? \"-\")</td>
                                                        <td>@(p.ValidTo?.ToString(\"dd.MM.yyyy - HH:mm:ss\") ?? \"-\")</td>
                                                        <td><span class=\"badge @(p.PriceType == 1 ? \"badge-info\" : \"badge-warning\")\">@(p.PriceType == 1 ? \"Satış\" : \"Kiralama\")</span></td>
                                                        <td class=\"font-weight-bold text-dark\">@p.Amount.ToString(\"N2\")</td>
                                                        @{
                                                            var currencies = ViewBag.Currencies as List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>;
                                                            var currencyName = currencies?.FirstOrDefault(c => string.Equals(c.Value, p.CurrencyId.ToString(), StringComparison.OrdinalIgnoreCase))?.Text ?? p.CurrencyId.ToString();
                                                        }
                                                        <td>@currencyName</td>
                                                        <td>
                                                            <button type=\"button\" class=\"btn btn-outline-danger btn-sm btn-delete-price\" data-id=\"@p.Id\"><i class=\"icofont icofont-ui-delete\"></i></button>
                                                        </td>'''

content = re.sub(old_row, new_row, content)

# Replace JS reloadPriceTable
old_js = r'function reloadPriceTable\(productId\) \{.*?initPriceDataTable\(\);\s*\}\);\s*\}'
new_js = '''function reloadPriceTable(productId) {
            $.getJSON('/Admin/Product/GetPricesForProduct?productId=' + productId, function (prices) {
                if ($.fn.DataTable.isDataTable('#priceTable')) {
                    #priceTable.DataTable().destroy();
                }
                var tbody = #priceTableBody;
                tbody.empty();
                if (prices && prices.length > 0) {
                    prices.forEach(function (p) {
                        var statusBadge = p.isActive ? '<span class=\"badge badge-success\">Aktif</span>' : '<span class=\"badge badge-danger\">Pasif</span>';
                        var badgeClass = p.priceType === 1 ? 'badge-info' : 'badge-warning';
                        var priceTypeName = p.priceType === 1 ? 'Satış' : 'Kiralama';
                        var row = '<tr>' +
                            '<td>' + statusBadge + '</td>' +
                            '<td>' + p.startDate + '</td>' +
                            '<td>' + p.endDate + '</td>' +
                            '<td><span class=\"badge ' + badgeClass + '\">' + priceTypeName + '</span></td>' +
                            '<td class=\"font-weight-bold text-dark\">' + p.amount + '</td>' +
                            '<td>' + p.currencyName + '</td>' +
                            '<td>' +
                            '<button type=\"button\" class=\"btn btn-outline-danger btn-sm btn-delete-price\" data-id=\"'+p.id+'\"><i class=\"icofont icofont-ui-delete\"></i></button>' +
                            '</td>' +
                            '</tr>';
                        tbody.append(row);
                    });
                }
                initPriceDataTable();
            });
        }

        // ==========================================
        // FİYAT SİLME İŞLEMİ
        // ==========================================
        .on('click', '.btn-delete-price', function() {
            var id = .data('id');
            var productId = #priceProductId.val();
            Swal.fire({
                title: 'Emin misiniz?',
                text: "Bu fiyat tanımı silinecektir!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3498db',
                confirmButtonText: 'Evet, Sil!',
                cancelButtonText: 'Vazgeç'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Admin/Product/DeletePrice',
                        type: 'POST',
                        data: { id: id },
                        success: function(r) {
                            if (r.success) {
                                Swal.fire({ icon: 'success', title: 'Silindi!', timer: 1000, showConfirmButton: false });
                                reloadPriceTable(productId);
                            } else {
                                Swal.fire({ icon: 'error', title: 'Hata', html: r.message });
                            }
                        }
                    });
                }
            });
        });
'''
content = re.sub(old_js, new_js, content, flags=re.DOTALL)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)
