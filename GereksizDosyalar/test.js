
        function validateField(input, rules) {
            var val = $(input).val() ? $(input).val().trim() : '';
            var $parent = $(input).closest('.form-group, div');
            var $error = $parent.find('.validation-error-msg');
            if ($error.length === 0) {
                $error = $('<small class="text-danger validation-error-msg" style="display:none; font-weight:600; margin-top:4px; width:100%;"></small>');
                $parent.append($error);
            }
            $error.hide().text('');

            if (rules.required && !val) {
                $error.text(rules.fieldName + ' alaný boþ geçilemez.').show();
                return false;
            }
            if (rules.lettersOnly && val) {
                var letterRegex = /^[a-zA-ZçÐýÖÞÜÇðÝöþü\s]+$/;
                if (!letterRegex.test(val)) {
                    $error.text('Lütfen sadece harf giriniz.').show();
                    return false;
                }
            }
            if (rules.numbersOnly && val) {
                var numberRegex = /^\d+$/;
                if (!numberRegex.test(val)) {
                    $error.text('Lütfen sadece sayý giriniz.').show();
                    return false;
                }
            }
            return true;
        }

        function reloadPriceTable(productId) {
            $.getJSON('/Admin/Product/GetPricesForProduct?productId=' + productId, function (prices) {
                if ($.fn.DataTable.isDataTable('#priceTable')) {
                    $('#priceTable').DataTable().destroy();
                }
                var tbody = $('#priceTableBody');
                tbody.empty();
                if (prices && prices.length > 0) {
                    prices.forEach(function (p) {
                        var statusBadge = p.isActive ? '<span class="badge badge-success">Aktif</span>' : '<span class="badge badge-danger">Pasif</span>';
                        var badgeClass = p.priceType === 1 ? 'badge-info' : 'badge-warning';
                        var priceTypeName = p.priceType === 1 ? 'Satýþ' : 'Kiralama';
                        var row = '<tr>' +
                            '<td>' + statusBadge + '</td>' +
                            '<td>' + p.startDate + '</td>' +
                            '<td>' + p.endDate + '</td>' +
                            '<td><span class="badge ' + badgeClass + '">' + priceTypeName + '</span></td>' +
                            '<td class="font-weight-bold text-dark">' + p.amount + '</td>' +
                            '<td>' + p.currencyName + '</td>' +
                            '<td>' +
                            '<button type="button" class="btn btn-outline-danger btn-sm btn-delete-price" data-id="'+p.id+'"><i class="icofont icofont-ui-delete"></i> Sil</button>' +
                            '</td>' +
                            '</tr>';
                        tbody.append(row);
                    });
                }
                initPriceDataTable();
            });
        }

        $(document).on('click', '.btn-delete-price', function() {
            var id = $(this).data('id');
            var productId = $('#priceProductId').val();
            Swal.fire({
                title: 'Emin misiniz?',
                text: "Bu fiyat tanýmý silinecektir!",
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

        function initPriceDataTable() {
            if ($.fn.DataTable.isDataTable('#priceTable')) {
                $('#priceTable').DataTable().destroy();
            }
            $('#priceTable').DataTable({
                dom: 'Bfrtip',
                buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
                order: [],
                language: {
                    processing:   'Ýþleniyor...',
                    search:       'Ara:',
                    lengthMenu:   'Sayfada _MENU_ kayýt göster',
                    info:         'Gösterilen _START_ - _END_ arasý, toplamda _TOTAL_ kayýt',
                    infoEmpty:    'Kayýt bulunamadý',
                    infoFiltered: '(_MAX_ kayýt içinden filtrelendi)',
                    paginate: { first: 'Ýlk', previous: 'Önceki', next: 'Sonraki', last: 'Son' },
                    zeroRecords:  'Kayýt bulunamadý',
                    emptyTable:   'Tabloda herhangi bir veri mevcut deðil'
                }
            });
        }

        function deleteImage(id, btnElement) {
            Swal.fire({
                title: 'Emin misiniz?',
                text: "Bu görsel kalýcý olarak silinecektir!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3498db',
                confirmButtonText: 'Evet, Sil!',
                cancelButtonText: 'Vazgeç'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Admin/Product/DeleteImage',
                        type: 'POST',
                        data: { id: id },
                        success: function(r) {
                            if (r.success) {
                                $(btnElement).closest('.image-card-container').fadeOut(300, function() {
                                    $(this).remove();
                                });
                                Swal.fire({ icon: 'success', title: 'Silindi!', timer: 1000, showConfirmButton: false });
                            } else {
                                Swal.fire({ icon: 'error', title: 'Hata', html: r.message });
                            }
                        }
                    });
                }
            });
        }

        function setMainImage(imageId, productId, btnElement) {
            var btn = $(btnElement);
            btn.prop('disabled', true);

            $.ajax({
                url: '/Admin/Product/SetMainImage',
                type: 'POST',
                data: { imageId: imageId, productId: productId },
                success: function(r) {
                    if (r.success) {
                        var cardContainer = btn.closest('.image-card-container');
                        $('.main-badge').removeClass('badge-success').addClass('badge-secondary').text('Ek Resim');
                        $('.btn-make-main').show();
                        cardContainer.find('.main-badge').removeClass('badge-secondary').addClass('badge-success').text('Ana Resim');
                        btn.hide();
                        cardContainer.slideUp(300, function() {
                            $('#imageGallery').prepend(cardContainer);
                            cardContainer.slideDown(300);
                        });
                        Swal.fire({ icon: 'success', title: 'Harika!', text: 'Ana resim güncellendi.', timer: 1500, showConfirmButton: false });
                    } else {
                        Swal.fire({ icon: 'error', title: 'Hata', html: r.message });
                    }
                    btn.prop('disabled', false);
                },
                error: function() {
                    Swal.fire({ icon: 'error', title: 'Sistem Hatasý', text: 'Ýþlem gerçekleþtirilemedi.' });
                    btn.prop('disabled', false);
                }
            });
        }

        function enableNextTab(tabId) {
            var nextTab = $(tabId);
            nextTab.removeClass('disabled').attr('data-toggle', 'tab');
            nextTab.tab('show');
        }

        $(document).ready(function() {
            $('#generalForm').on('submit', function(e) {
                e.preventDefault();
                var form = $(this);
                if (!this.checkValidity()) {
                    this.reportValidity();
                    return false;
                }
                var btn = form.find('button[type="submit"]');
                btn.prop('disabled', true);
                $.ajax({
                    url: form.attr('action'),
                    type: 'POST',
                    data: form.serialize(),
                    success: function(r) {
                        if (r.success) {
                            var productId = r.id || $('#productId').val();
                            $('#productId, #detailProductId, #priceProductId, #imageProductId').val(productId);
                            Swal.fire({
                                icon: 'success', title: 'Baþarýlý', text: 'Genel bilgiler kaydedildi!',
                                timer: 1500, showConfirmButton: false
                            }).then(() => {
                                enableNextTab('#tab-detay-link'); 
                            });
                        } else {
                            Swal.fire({ icon: 'error', title: 'Hata', html: r.message });
                        }
                        btn.prop('disabled', false);
                    },
                    error: function() {
                        Swal.fire({ icon: 'error', title: 'Sistem Hatasý', text: 'Ýþlem gerçekleþtirilemedi.' });
                        btn.prop('disabled', false);
                    }
                });
            });

            $('#detailForm').on('submit', function(e) {
                e.preventDefault();
                var form = $(this);
                if (!this.checkValidity()) {
                    this.reportValidity();
                    return false;
                }
                var btn = form.find('button[type="submit"]');
                btn.prop('disabled', true);
                $.ajax({
                    url: form.attr('action'),
                    type: 'POST',
                    data: form.serialize(),
                    success: function(r) {
                        if (r.success) {
                            Swal.fire({ icon: 'success', title: 'Baþarýlý', text: 'Detaylar kaydedildi!', timer: 1500, showConfirmButton: false })
                                .then(() => {
                                    enableNextTab('#tab-fiyat-link'); 
                                });
                        } else {
                            Swal.fire({ icon: 'error', title: 'Hata', html: r.message });
                        }
                        btn.prop('disabled', false);
                    }
                });
            });

            $('#priceForm').on('submit', function(e) {
                e.preventDefault();
                var form = $(this);
                var startVal = $('input[name="ValidFrom"]').val();
                var endVal = $('input[name="ValidTo"]').val();
                var endInput = $('input[name="ValidTo"]')[0];
                if (startVal && endVal) {
                    var startDate = new Date(startVal.replace(' ', 'T'));
                    var endDate = new Date(endVal.replace(' ', 'T'));
                    if (endDate < startDate) {
                        if (endInput) {
                            endInput.setCustomValidity("Bitiþ tarihi baþlangýç tarihinden önce olamaz.");
                            endInput.reportValidity();
                        }
                        return false;
                    }
                }
                if (!this.checkValidity()) {
                    this.reportValidity();
                    return false;
                }
                var btn = form.find('button[type="submit"]');
                btn.prop('disabled', true);
                $.ajax({
                    url: form.attr('action'),
                    type: 'POST',
                    data: form.serialize(),
                    success: function(r) {
                        if (r.success) {
                            Swal.fire({ icon: 'success', title: 'Fiyat Eklendi', timer: 1500, showConfirmButton: false });
                            var pid = $('#priceProductId').val();
                            reloadPriceTable(pid);
                            form[0].reset();
                            $('#priceProductId').val(pid);
                            if(typeof startPicker !== 'undefined') startPicker.clear();
                            if(typeof endPicker !== 'undefined') endPicker.clear();
                            if(typeof startPicker !== 'undefined') startPicker.set('maxDate', null);
                            if(typeof endPicker !== 'undefined') endPicker.set('minDate', null);
                        } else {
                            Swal.fire({ icon: 'error', title: 'Hata', html: r.message });
                        }
                        btn.prop('disabled', false);
                    },
                    error: function() {
                        Swal.fire({ icon: 'error', title: 'Hata', text: 'Fiyat eklenirken bir hata oluþtu.' });
                        btn.prop('disabled', false);
                    }
                });
            });

            $('#btnUpload').on('click', function() {
                var form = $('#imageForm');
                var formData = new FormData(form[0]);
                var btn = $(this);
                btn.prop('disabled', true);
                $.ajax({
                    url: '/Admin/Product/UploadImage',
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function(r) {
                        if (r.success) {
                            Swal.fire({ icon: 'success', title: 'Görsel Yüklendi!', timer: 1500, showConfirmButton: false });
                            var isMain = form.find('input[name="isMain"]').is(':checked');
                            var badge = isMain ? '<span class="badge badge-success m-b-5">Ana Resim</span>' : '<span class="badge badge-secondary m-b-5">Ek Resim</span>';
                            var newImg = `<div class="col-md-3 col-sm-6 mb-4"><div class="card h-100 shadow-sm border overflow-hidden"><img src="${r.url}" class="card-img-top" style="height: 180px; object-fit: cover;"><div class="card-body p-2 text-center bg-white">${badge}<div class="m-t-5"><button class="btn btn-secondary btn-sm" disabled>Yeni</button></div></div></div></div>`;
                            $('#no-image-msg').remove();
                            $('#imageGallery').append(newImg);
                            var pid = $('#imageProductId').val();
                            form[0].reset();
                            $('#imageProductId').val(pid);
                        } else {
                            Swal.fire({ icon: 'error', title: 'Hata', html: r.message });
                        }
                        btn.prop('disabled', false);
                    }
                });
            });

            $('.btn-wizard-next').on('click', function(e) {
                if ($(this).attr('type') !== 'submit') {
                    var target = $(this).closest('.tab-pane').next().attr('id');
                    if (target) {
                        var tabLink = $('.wizard-tabs a[href="#' + target + '"]');
                        tabLink.removeClass('disabled').attr('data-toggle', 'tab');
                        tabLink.tab('show');
                    }
                }
            });

            $('.btn-wizard-prev').on('click', function() {
                var target = $(this).closest('.tab-pane').prev().attr('id');
                if (target) {
                    $('.wizard-tabs a[href="#' + target + '"]').tab('show');
                }
            });

            $('a[href="#prices"]').on('shown.bs.tab', function () {
                if (!$.fn.DataTable.isDataTable('#priceTable')) {
                    initPriceDataTable();
                }
            });

            var tagInput = document.querySelector('input[name="Tag"]');
            if (tagInput) {
                try { var tagify = new Tagify(tagInput); tagify.on('change', function() { tagInput.setCustomValidity(""); }); } catch(err) { console.error("Tagify error:", err); }
            }

            var startPicker = flatpickr('input[name="ValidFrom"]', {
                enableTime: true,
                dateFormat: "Y-m-d H:i",
                time_24hr: true,
                locale: "tr",
                onChange: function(selectedDates, dateStr, instance) {
                    endPicker.set('minDate', dateStr);
                    var endInput = $('input[name="ValidTo"]')[0];
                    if (endInput) endInput.setCustomValidity("");
                    var startInput = $('input[name="ValidFrom"]')[0];
                    if (startInput) startInput.setCustomValidity("");
                }
            });
            var endPicker = flatpickr('input[name="ValidTo"]', {
                enableTime: true,
                dateFormat: "Y-m-d H:i",
                time_24hr: true,
                locale: "tr",
                onChange: function(selectedDates, dateStr, instance) {
                    startPicker.set('maxDate', dateStr);
                    var endInput = $('input[name="ValidTo"]')[0];
                    if (endInput) endInput.setCustomValidity("");
                }
            });

        });
    
