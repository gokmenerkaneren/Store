/**
 * Store.WebUI - Basket (Sepet) & Offcanvas Drawer Management
 * 
 * Saf Vanilla JavaScript ile yazılmıştır (jQuery bağımlılığı yoktur).
 * 
 * Özellikler:
 * 1. "Sepete Ekle" butonuna basıldığında ürünü ekler ve sağ çekmeceyi (4 numara) açar.
 * 2. Header "Bag" ikonuna basıldığında ANINDA çekmeceyi açar ve güncel sepet verilerini sunucudan çeker.
 * 3. Çekmece içinde adet artırma (+), azaltma (-) ve silme (Kaldır).
 * 4. Satış vs Kiralama Çakışma Yönetimi (409 Conflict).
 * 5. Yetkisiz Kullanıcı (401) Kontrolü -> /Uyelik/Index yönlendirmesi.
 */

(function () {
    'use strict';

    // Para formatlayıcı (Örn: 125,00 ₺)
    function formatMoney(amount) {
        return Number(amount || 0).toLocaleString('tr-TR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }) + ' ₺';
    }

    // Fiyat parse edici
    function parseLocalizedPrice(str) {
        if (typeof str === 'number') return str;
        str = (str || '').toString().trim();
        str = str.replace(/[^\d.,]/g, '');
        if (str.indexOf(',') !== -1 && str.indexOf('.') !== -1) {
            str = str.replace(/\./g, '').replace(',', '.');
        } else if (str.indexOf(',') !== -1) {
            str = str.replace(',', '.');
        }
        return parseFloat(str) || 0;
    }

    // Çekmeceyi Aç (Tüm popup elemanlarını ve CSS kurallarını anında uygular)
    function openCartDrawer() {
        var popups = document.querySelectorAll('.popup.js-popup');
        var cartBodies = document.querySelectorAll('[data-js-popup-name="cart"]');
        var bgs = document.querySelectorAll('.popup__bg, [data-js-popup-bg]');

        popups.forEach(function (popup) {
            popup.querySelectorAll('[data-js-popup-name]').forEach(function (el) {
                if (el.getAttribute('data-js-popup-name') !== 'cart') {
                    el.classList.remove('show', 'visible');
                    el.classList.add('d-none');
                    el.style.display = 'none';
                }
            });
            popup.classList.remove('d-none');
            popup.classList.add('active', 'show');
            popup.style.display = 'block';
        });

        bgs.forEach(function (bg) {
            bg.classList.add('visible', 'show');
            bg.style.display = 'block';
            bg.style.opacity = '0.5';
            bg.style.pointerEvents = 'auto';
        });

        cartBodies.forEach(function (cartBody) {
            cartBody.classList.remove('d-none');
            cartBody.classList.add('show', 'visible');
            cartBody.style.display = 'flex';

            var content = cartBody.querySelector('[data-popup-content]');
            if (content) {
                content.style.opacity = '1';
                content.style.transform = 'none';
            }
        });

        document.body.classList.add('overflow-hidden');

        // Varsa Shella tema motorunu da tetikle
        if (window.theme && window.theme.Popups && typeof window.theme.Popups.callByName === 'function') {
            try {
                window.theme.Popups.callByName('cart');
            } catch (ex) {
                console.warn('theme.Popups.callByName hatası:', ex);
            }
        }
    }

    // Çekmeceyi Kapat
    function closeCartDrawer() {
        var popups = document.querySelectorAll('.popup.js-popup');
        var cartBodies = document.querySelectorAll('[data-js-popup-name="cart"]');
        var bgs = document.querySelectorAll('.popup__bg, [data-js-popup-bg]');

        popups.forEach(function (popup) {
            popup.classList.remove('active', 'show');
            popup.classList.add('d-none');
            popup.style.display = 'none';
        });

        cartBodies.forEach(function (cartBody) {
            cartBody.classList.remove('show', 'visible');
            cartBody.classList.add('d-none');
            cartBody.style.display = 'none';

            var content = cartBody.querySelector('[data-popup-content]');
            if (content) {
                content.style.opacity = '';
                content.style.transform = '';
            }
        });

        bgs.forEach(function (bg) {
            bg.classList.remove('visible', 'show');
            bg.style.opacity = '';
            bg.style.pointerEvents = '';
            bg.style.display = 'none';
        });

        document.body.classList.remove('overflow-hidden');

        if (window.theme && window.theme.Popups && typeof window.theme.Popups.closeByName === 'function') {
            try {
                window.theme.Popups.closeByName('cart');
            } catch (ex) { }
        }
    }

    // Ürün butonlarının sepet durumunu güncelle (ÜRÜN SEPETİNİZDE / SEPETE EKLE)
    function updateCartButtons(cartProductIds) {
        try {
            if (!Array.isArray(cartProductIds)) cartProductIds = [];
            var cartSet = {};
            cartProductIds.forEach(function (id) {
                if (id) cartSet[String(id).toLowerCase()] = true;
            });

            // 1. Detay sayfası sepete ekle butonu
            var detailBtn = document.getElementById('btn-add-to-cart');
            var detailPidInput = document.getElementById('product-detail-id');
            if (detailBtn && detailPidInput && detailPidInput.value) {
                var detailPid = detailPidInput.value.trim().toLowerCase();
                var isInCart = !!cartSet[detailPid];
                var textEl = detailBtn.querySelector('.btn__text') || detailBtn;
                if (isInCart) {
                    detailBtn.classList.add('is-in-cart');
                    detailBtn.style.backgroundColor = '#28a745';
                    detailBtn.style.borderColor = '#28a745';
                    detailBtn.style.color = '#ffffff';
                    textEl.textContent = 'ÜRÜN SEPETİNİZDE';
                } else {
                    detailBtn.classList.remove('is-in-cart');
                    detailBtn.style.backgroundColor = '';
                    detailBtn.style.borderColor = '';
                    detailBtn.style.color = '';
                    textEl.textContent = 'SEPETE EKLE';
                }
            }

            // 2. Sayfadaki tüm kart butonları
            document.querySelectorAll('.js-product-button-add-to-cart, [data-js-product-button-add-to-cart]').forEach(function (btn) {
                var pid = (btn.getAttribute('data-product-id') || '').toLowerCase();
                if (!pid) {
                    var card = btn.closest('.product-collection, .product-card, [data-product-handle]');
                    if (card) {
                        pid = (card.getAttribute('data-product-id') || card.getAttribute('data-product-variant-id') || (card.getAttribute('data-product-handle') || '').replace('product-', '')).toLowerCase();
                    }
                }
                if (pid) {
                    var inCart = !!cartSet[pid];
                    var spanText = btn.querySelector('.btn__text');
                    if (inCart) {
                        btn.classList.add('is-in-cart');
                        btn.style.backgroundColor = '#28a745';
                        btn.style.borderColor = '#28a745';
                        btn.style.color = '#ffffff';
                        if (spanText) spanText.textContent = 'ÜRÜN SEPETİNİZDE';
                    } else {
                        btn.classList.remove('is-in-cart');
                        btn.style.backgroundColor = '';
                        btn.style.borderColor = '';
                        btn.style.color = '';
                        if (spanText) spanText.textContent = 'SEPETE EKLE';
                    }
                }
            });
        } catch (ex) {
            console.warn('[StoreBasket] updateCartButtons hatası:', ex);
        }
    }

    // Sepet Verisini ve Çekmece İçeriğini Ekrana Bas
    function renderBasket(basket) {
        var items = (basket && basket.basketItems) ? basket.basketItems : [];
        var totalCount = 0;
        var totalPrice = 0;

        items.forEach(function (item) {
            totalCount += item.quantity;
            totalPrice += (item.quantity * item.price);
        });

        // Sayaçları güncelle
        document.querySelectorAll('[data-js-cart-count-desktop]').forEach(function (el) {
            el.textContent = 'Sepetim (' + totalCount + ')';
        });
        document.querySelectorAll('[data-js-cart-count-mobile]').forEach(function (el) {
            el.textContent = totalCount;
        });
        document.querySelectorAll('[data-js-popup-cart-count]').forEach(function (el) {
            el.textContent = '(' + totalCount + ')';
        });

        // Sayfadaki ürün butonlarını sepet durumuna göre güncelle (ÜRÜN SEPETİNİZDE / SEPETE EKLE)
        var cartProductIds = items.map(function (item) {
            return String(item.productId).toLowerCase();
        });
        updateCartButtons(cartProductIds);

        var itemsContainers = document.querySelectorAll('.js-popup-cart-items');
        var contentWrappers = document.querySelectorAll('.js-popup-cart-content-wrapper');
        var emptyMessages = document.querySelectorAll('.js-popup-cart-empty');
        var subtotalElements = document.querySelectorAll('.js-popup-cart-subtotal');

        subtotalElements.forEach(function (el) {
            el.textContent = formatMoney(totalPrice);
        });

        if (items.length === 0) {
            itemsContainers.forEach(function (c) { c.innerHTML = ''; });
            contentWrappers.forEach(function (w) { w.classList.add('d-none'); });
            emptyMessages.forEach(function (m) { m.classList.remove('d-none'); });
            return;
        }

        emptyMessages.forEach(function (m) { m.classList.add('d-none'); });
        contentWrappers.forEach(function (w) { w.classList.remove('d-none'); });

        var itemsHtml = '';
        items.forEach(function (item) {
            var imgSrc = item.imageUrl || '/images/no-image.jpg';
            var typeBadge = item.isRental
                ? '<span class="badge badge-warning text-dark px-2 py-1 mb-1" style="font-size: 11px; background-color: #ffeeba; border-radius: 4px;">Kiralama</span>'
                : '<span class="badge badge-light text-muted px-2 py-1 mb-1" style="font-size: 11px; background-color: #f8f9fa; border: 1px solid #dee2e6; border-radius: 4px;">Satış</span>';

            itemsHtml += `
                <div class="product-cart d-flex flex-row align-items-start mb-20 pb-15 border-bottom" data-product-id="${item.productId}">
                    <div class="product-cart__image mr-15" style="width: 70px; min-width: 70px;">
                        <a href="/Product/Index/${item.productId}" class="d-block">
                            <img src="${imgSrc}" alt="${item.productName}" style="width: 100%; height: 75px; object-fit: cover; border-radius: 4px; border: 1px solid #eee;">
                        </a>
                    </div>
                    <div class="product-cart__content d-flex flex-column align-items-start flex-grow-1">
                        <div class="product-cart__title mb-2">
                            <h3 class="h6 m-0" style="font-size: 13px; font-weight: 600; line-height: 1.3;">
                                <a href="/Product/Index/${item.productId}">${item.productName}</a>
                            </h3>
                        </div>
                        ${typeBadge}
                        <div class="product-cart__price mt-1 mb-10" style="font-size: 13px;">
                            <span class="font-weight-bold text-dark">${formatMoney(item.price)}</span>
                        </div>
                        <div class="d-flex align-items-center justify-content-between w-100 mt-5">
                            <div class="d-flex align-items-center border rounded" style="background: #fbfbfb;">
                                <button type="button" class="btn btn-sm btn-light py-0 px-2 js-cart-item-minus" data-id="${item.productId}" style="height: 28px; border: none; font-weight: bold; cursor: pointer;">-</button>
                                <span class="px-2 font-weight-bold" style="min-width: 26px; text-align: center; font-size: 13px;">${item.quantity}</span>
                                <button type="button" class="btn btn-sm btn-light py-0 px-2 js-cart-item-plus" data-id="${item.productId}" style="height: 28px; border: none; font-weight: bold; cursor: pointer;">+</button>
                            </div>
                            <br/>
                            <a href="javascript:void(0);" class="product-cart__remove js-cart-item-remove text-danger ml-auto" data-id="${item.productId}" style="font-size: 12px; cursor: pointer; text-decoration: none;">
                                <i class="fa fa-trash mr-1"></i>Kaldır
                            </a>
                        </div>
                    </div>
                </div>
            `;
        });

        itemsContainers.forEach(function (c) {
            c.innerHTML = itemsHtml;
        });
    }

    // Sepeti Sunucudan Çek (Vanilla fetch)
    function fetchBasket(onSuccess, onError) {
        fetch('/Basket/GetBasket', {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        })
            .then(function (response) {
                if (response.status === 401) {
                    if (typeof onError === 'function') onError(response);
                    return null;
                }
                return response.json();
            })
            .then(function (res) {
                if (res && res.success) {
                    try {
                        renderBasket(res.data);
                    } catch (renderEx) {
                        console.warn('[StoreBasket] renderBasket hatası:', renderEx);
                    }
                    if (typeof onSuccess === 'function') onSuccess(res.data);
                }
            })
            .catch(function (err) {
                console.error('fetchBasket hatası:', err);
                if (typeof onError === 'function') onError(err);
            });
    }

    // İstek kilitleri (In-flight request guard / Debounce)
    var isAddToCartBusy = false;
    var isQuantityBusy = false;

    // Sepete Ekleme İsteği Gönder (Vanilla fetch)
    function sendAddToCartRequest(payload, btn) {
        if (isAddToCartBusy) {
            console.warn('[StoreBasket] Sepete ekleme işlemi halen devam ediyor, mükerrer istek engellendi.');
            return;
        }

        isAddToCartBusy = true;

        if (btn) {
            btn.setAttribute('disabled', 'disabled');
            btn.classList.add('loading');
            btn.style.pointerEvents = 'none';
        }

        function releaseButton() {
            isAddToCartBusy = false;
            if (btn) {
                btn.removeAttribute('disabled');
                btn.classList.remove('loading');
                btn.style.pointerEvents = '';
            }
        }

        fetch('/Basket/AddToCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(payload)
        })
            .then(function (response) {
                if (response.status === 401) {
                    releaseButton();
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            icon: 'info',
                            title: 'Oturum Açınız',
                            text: 'Sepete ürün eklemek için lütfen önce giriş yapınız.',
                            confirmButtonText: 'Giriş Yap',
                            confirmButtonColor: '#212529'
                        }).then(function () {
                            window.location.href = '/Uyelik/Index';
                        });
                    } else {
                        alert('Sepete ürün eklemek için lütfen önce giriş yapınız.');
                        window.location.href = '/Uyelik/Index';
                    }
                    return null;
                }
                if (response.status === 409) {
                    releaseButton();
                    return response.json().then(function (errData) {
                        var message = (errData && errData.message)
                            ? errData.message
                            : 'Sepetinizdeki ürün tipiyle eklemeye çalıştığınız ürün tipi uyuşmuyor.';
                        if (typeof Swal !== 'undefined') {
                            Swal.fire({
                                icon: 'warning',
                                title: 'Sepet Tipi Değişikliği',
                                text: message,
                                showCancelButton: true,
                                confirmButtonText: 'Evet, Sepeti Boşalt ve Ekle',
                                cancelButtonText: 'Vazgeç',
                                confirmButtonColor: '#212529',
                                cancelButtonColor: '#6c757d'
                            }).then(function (result) {
                                if (result.isConfirmed) {
                                    payload.ClearAndAdd = true;
                                    sendAddToCartRequest(payload, btn);
                                }
                            });
                        } else {
                            if (confirm(message)) {
                                payload.ClearAndAdd = true;
                                sendAddToCartRequest(payload, btn);
                            }
                        }
                    });
                }
                return response.json().then(function (data) {
                    return { ok: response.ok, data: data };
                }).catch(function () {
                    return { ok: response.ok, data: null };
                });
            })
            .then(function (result) {
                if (!result) return;
                releaseButton();
                var res = result.data;
                if (result.ok && res && res.success) {
                    try {
                        renderBasket(res.data);
                    } catch (renderEx) {
                        console.warn('[StoreBasket] renderBasket hatası:', renderEx);
                    }

                    if (btn) {
                        btn.classList.add('added');
                        setTimeout(function () {
                            btn.classList.remove('added');
                        }, 2000);
                    }

                    // Sağ sepet çekmecesini aç
                    openCartDrawer();
                } else {
                    var msg = (res && res.message) ? res.message : 'Sepete ürün eklenirken bir hata oluştu.';
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Sepet Bildirimi',
                            text: msg,
                            confirmButtonText: 'Tamam',
                            confirmButtonColor: '#212529'
                        });
                    } else {
                        alert(msg);
                    }
                }
            })
            .catch(function (err) {
                releaseButton();
                console.error('sendAddToCartRequest hatası:', err);
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: 'Sepet işlemine ulaşılamadı. Lütfen tekrar deneyiniz.',
                        confirmButtonText: 'Tamam',
                        confirmButtonColor: '#212529'
                    });
                } else {
                    alert('Sepet işlemine ulaşılamadı. Lütfen tekrar deneyiniz.');
                }
            });
    }

    // Çekmece İçi Miktar Güncelleme (Vanilla fetch)
    function updateItemQuantity(productId, change) {
        if (isQuantityBusy) return;
        isQuantityBusy = true;

        fetch('/Basket/UpdateQuantity', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({ ProductId: productId, Change: change })
        })
            .then(function (r) { return r.json(); })
            .then(function (res) {
                if (res && res.success) {
                    try {
                        renderBasket(res.data);
                    } catch (renderEx) {
                        console.warn('[StoreBasket] renderBasket hatası:', renderEx);
                    }
                }
            })
            .catch(console.error)
            .finally(function () {
                isQuantityBusy = false;
            });
    }

    // Çekmece İçi Ürün Silme (Vanilla fetch)
    function removeCartItem(productId) {
        if (isQuantityBusy) return;
        isQuantityBusy = true;

        fetch('/Basket/RemoveItem', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({ ProductId: productId })
        })
            .then(function (r) { return r.json(); })
            .then(function (res) {
                if (res && res.success) {
                    try {
                        renderBasket(res.data);
                    } catch (renderEx) {
                        console.warn('[StoreBasket] renderBasket hatası:', renderEx);
                    }
                }
            })
            .catch(console.error)
            .finally(function () {
                isQuantityBusy = false;
            });
    }

    // =========================================================================
    // NATIVE CAPTURE PHASE EVENT LISTENERS
    // (theme.js tarafından yutulmayı kesin olarak önler)
    // =========================================================================
    document.addEventListener('click', function (e) {
        // 1. "Sepete Ekle" Buton Tıklaması
        var addToCartBtn = e.target.closest('.js-product-button-add-to-cart, [data-js-trigger-id="add-to-cart"], [data-js-product-button-add-to-cart]');
        if (addToCartBtn) {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();

            var productId = '';
            var productName = '';
            var price = 0;
            var imageUrl = '';
            var isRental = false;
            var quantity = 1;

            var guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

            // 1. Öncelik: Ürün Detay Sayfası (#product-detail-* elemanları mevcutsa)
            var productIdInput = document.getElementById('product-detail-id');
            var isDetailPage = !!(productIdInput && productIdInput.value) && 
                (!!addToCartBtn.closest('.product-page, .product-page-main, [data-section-type="product-page"], form[data-js-product-form]') || !addToCartBtn.closest('.product-collection, .product-card'));

            if (isDetailPage) {
                productId = (productIdInput.value || '').trim();
                var productNameInput = document.getElementById('product-detail-name');
                var priceInput = document.getElementById('product-detail-price');
                var imageInput = document.getElementById('product-detail-image');
                var isRentalInput = document.getElementById('product-detail-isrental');
                var qtyInput = document.querySelector('.product-page input[name="quantity"], form[data-js-product-form] input[name="quantity"], input[name="quantity"]');

                productName = (productNameInput ? productNameInput.value : (addToCartBtn.getAttribute('data-product-name') || '')).trim();
                var rawPrice = priceInput ? priceInput.value : (addToCartBtn.getAttribute('data-product-price') || '0');
                price = parseLocalizedPrice(rawPrice);
                imageUrl = (imageInput ? imageInput.value : (addToCartBtn.getAttribute('data-product-image') || '')).trim();
                isRental = isRentalInput ? isRentalInput.value === 'true' : (addToCartBtn.getAttribute('data-product-isrental') === 'true');
                quantity = qtyInput ? (parseInt(qtyInput.value, 10) || 1) : 1;
            }
            // 2. Öncelik: Butonun kendi data öznitelikleri (data-product-id)
            else if (addToCartBtn.dataset && (addToCartBtn.dataset.productId || addToCartBtn.getAttribute('data-product-id'))) {
                productId = (addToCartBtn.dataset.productId || addToCartBtn.getAttribute('data-product-id') || '').trim();
                productName = (addToCartBtn.dataset.productName || addToCartBtn.getAttribute('data-product-name') || '').trim();
                var rawPrice = addToCartBtn.dataset.productPrice || addToCartBtn.getAttribute('data-product-price') || '0';
                price = parseLocalizedPrice(rawPrice);
                imageUrl = (addToCartBtn.dataset.productImage || addToCartBtn.getAttribute('data-product-image') || '').trim();
                isRental = (addToCartBtn.dataset.productIsrental || addToCartBtn.getAttribute('data-product-isrental')) === 'true';

                var parentElem = addToCartBtn.closest('.card, .product-collection, .product-card, .product-item, [id^="fav-card-"], form');
                var qtyEl = (productId ? document.getElementById('qty-' + productId) : null) || (addToCartBtn.parentElement ? addToCartBtn.parentElement.querySelector('input[name="quantity"]') : null) || (parentElem ? parentElem.querySelector('input[name="quantity"]') : null);
                var qtyVal = qtyEl ? parseInt(qtyEl.value, 10) : (parseInt(addToCartBtn.getAttribute('data-product-quantity'), 10) || 1);
                quantity = (!isNaN(qtyVal) && qtyVal > 0) ? qtyVal : 1;
            }
            // 3. Öncelik: Kart kapsayıcısı (.product-collection, .product-card)
            else if (addToCartBtn.closest('.product-collection, .product-card, [data-js-product]:not(.product-page-main)')) {
                var card = addToCartBtn.closest('.product-collection, .product-card, [data-js-product]:not(.product-page-main)');
                productId = (card.getAttribute('data-product-id') || card.getAttribute('data-product-variant-id') || (card.getAttribute('data-product-handle') || '').replace('product-', '')).trim();
                var cardTitle = card.querySelector('.product-collection__title a');
                productName = cardTitle ? cardTitle.textContent.trim() : '';
                var cardPrice = card.querySelector('.product-collection__price .price .money');
                price = cardPrice ? parseLocalizedPrice(cardPrice.textContent) : 0;
                var cardImg = card.querySelector('img');
                imageUrl = cardImg ? (cardImg.getAttribute('src') || cardImg.getAttribute('data-src') || '') : '';
                isRental = false;
                var cardQtyEl = card.querySelector('input[name="quantity"]');
                quantity = cardQtyEl ? (parseInt(cardQtyEl.value, 10) || 1) : 1;
            }

            // GUID Doğrulaması ve Fallback Kontrolü
            if (!guidRegex.test(productId)) {
                if (productIdInput && guidRegex.test((productIdInput.value || '').trim())) {
                    productId = productIdInput.value.trim();
                } else {
                    console.warn('[StoreBasket] Geçersiz GUID ürün kimliği:', productId);
                    alert('Geçersiz ürün bilgisi.');
                    return;
                }
            }

            if (!productId) {
                console.warn('[StoreBasket] Sepete eklenecek ürün kimliği (ID) bulunamadı.');
                return;
            }

            if (quantity < 1) quantity = 1;

            var payload = {
                ProductId: productId,
                ProductName: productName,
                Price: price,
                ImageUrl: imageUrl,
                Quantity: quantity,
                IsRental: isRental,
                ClearAndAdd: false
            };

            sendAddToCartRequest(payload, addToCartBtn);
            return;
        }

        // 2. Header Bag İkonu Tıklaması (3 Numara)
        var bagBtn = e.target.closest('.header__btn-cart, [data-js-popup-button="cart"]');
        if (bagBtn) {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();

            // Çekmeceyi ANINDA aç
            openCartDrawer();

            // Arka planda sepeti tazele (yetkisiz ise uyar ve yönlendir)
            fetchBasket(
                null,
                function (err) {
                    if (err && err.status === 401) {
                        closeCartDrawer();
                        alert('Sepetinizi görüntülemek için lütfen önce giriş yapınız.');
                        window.location.href = '/Uyelik/Index';
                    }
                }
            );
            return;
        }

        // 3. Sepet Çekmecesi Açıkken Kapatma ve Dış Alana Tıklama Kontrolü
        var cartPopupEl = document.querySelector('[data-js-popup-name="cart"].show, [data-js-popup-name="cart"].visible, .popup.active, .popup.show');
        var isDrawerOpen = cartPopupEl && window.getComputedStyle(cartPopupEl).display !== 'none';

        if (isDrawerOpen) {
            // A) Çarpı Kapatma Butonuna basıldıysa kapat
            var closeBtn = e.target.closest('.popup-cart__close, [data-js-popup-close]');
            if (closeBtn) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();
                closeCartDrawer();
                return;
            }

            // B) Tıklanan yer sepet panelinin ([data-popup-content] / .popup-cart) İÇİNDE DEĞİLSE
            // Yani kullanıcının kırmızı ile işaretlediği alan, arka plan veya sayfanın herhangi bir dış alanı:
            var insideCartContent = e.target.closest('.popup-cart, [data-popup-content]');
            if (!insideCartContent) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();
                closeCartDrawer();
                return;
            }
        }

        // 4. Detay Sayfası Adet (+/-) butonları theme.js tarafından 1'er 1'er yönetilmektedir.

        // 6. Çekmece İçi Adet Artırma (+)
        var itemPlusBtn = e.target.closest('.js-cart-item-plus');
        if (itemPlusBtn) {
            e.preventDefault();
            var pid = itemPlusBtn.getAttribute('data-id');
            if (pid) updateItemQuantity(pid, 1);
            return;
        }

        // 7. Çekmece İçi Adet Azaltma (-)
        var itemMinusBtn = e.target.closest('.js-cart-item-minus');
        if (itemMinusBtn) {
            e.preventDefault();
            var pid = itemMinusBtn.getAttribute('data-id');
            if (pid) updateItemQuantity(pid, -1);
            return;
        }

        // 8. Çekmece İçi Ürünü Silme (Kaldır)
        var itemRemoveBtn = e.target.closest('.js-cart-item-remove');
        if (itemRemoveBtn) {
            e.preventDefault();
            var pid = itemRemoveBtn.getAttribute('data-id');
            if (pid) removeCartItem(pid);
            return;
        }
    }, true); // Capture Phase!

    // ESC (Escape) tuşuna basıldığında çekmeceyi kapat
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' || e.keyCode === 27) {
            closeCartDrawer();
        }
    });

    // Form submit olayını global olarak yakala ve sayfa yenilemesini / mükerrer gönderimleri engelle
    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (form && (form.hasAttribute('data-js-product-form') || form.querySelector('#product-detail-id') || (form.action && form.action.indexOf('/cart') !== -1))) {
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
    }, true);

    // Sayfa Yüklendiğinde Mevcut Sepet Durumunu Al
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            fetchBasket();
        });
    } else {
        fetchBasket();
    }

    // Global nesne olarak dışa aç
    window.StoreBasket = {
        open: openCartDrawer,
        close: closeCartDrawer,
        refresh: fetchBasket
    };
})();
