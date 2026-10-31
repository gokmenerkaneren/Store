(function () {
    "use strict";

    // Global state
    var currentFavorites = [];

    // Çekmeceyi Aç (Tüm popup elemanlarını ve CSS kurallarını anında uygular)
    function openWishlistDrawer() {
        var popups = document.querySelectorAll('.popup.js-popup');
        var wishlistBodies = document.querySelectorAll('[data-js-popup-name="wishlist"]');
        var bgs = document.querySelectorAll('.popup__bg, [data-js-popup-bg]');

        popups.forEach(function (popup) {
            popup.querySelectorAll('[data-js-popup-name]').forEach(function (el) {
                if (el.getAttribute('data-js-popup-name') !== 'wishlist') {
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

        wishlistBodies.forEach(function (body) {
            body.classList.remove('d-none');
            body.classList.add('show', 'visible');
            body.style.display = 'flex';

            var content = body.querySelector('[data-popup-content]');
            if (content) {
                content.style.opacity = '1';
                content.style.transform = 'none';
            }
        });

        document.body.classList.add('overflow-hidden');

        // Shella tema motoru varsa onu da bilgilendir
        if (window.theme && window.theme.Popups && typeof window.theme.Popups.callByName === 'function') {
            try {
                window.theme.Popups.callByName('wishlist');
            } catch (ex) {
                console.warn('theme.Popups.callByName(wishlist) uyarısı:', ex);
            }
        }
    }

    // Çekmeceyi Kapat
    function closeWishlistDrawer() {
        var popups = document.querySelectorAll('.popup.js-popup');
        var wishlistBodies = document.querySelectorAll('[data-js-popup-name="wishlist"]');
        var bgs = document.querySelectorAll('.popup__bg, [data-js-popup-bg]');

        popups.forEach(function (popup) {
            popup.classList.remove('active', 'show');
            popup.classList.add('d-none');
            popup.style.display = 'none';
        });

        wishlistBodies.forEach(function (body) {
            body.classList.remove('show', 'visible');
            body.classList.add('d-none');
            body.style.display = 'none';

            var content = body.querySelector('[data-popup-content]');
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
                window.theme.Popups.closeByName('wishlist');
            } catch (ex) { }
        }
    }

    // Favorileri ve Çekmece İçeriğini Ekrana Bas
    function renderWishlist(data) {
        var items = (data && data.items) ? data.items : [];
        var count = (data && typeof data.count === 'number') ? data.count : items.length;
        currentFavorites = items.map(function (x) { return String(x.productId).toLowerCase(); });

        // Header ve Popup Sayaçlarını Güncelle
        document.querySelectorAll('[data-js-wishlist-count]').forEach(function (el) {
            el.textContent = count;
            el.setAttribute('data-js-wishlist-count', count);
        });

        document.querySelectorAll('[data-js-wishlist-count-desktop]').forEach(function (el) {
            el.textContent = 'Favorilerim (' + count + ')';
            el.setAttribute('data-js-wishlist-count-desktop', count);
        });

        document.querySelectorAll('[data-js-popup-wishlist-count], .js-popup-wishlist-count').forEach(function (el) {
            el.textContent = '(' + count + ')';
        });

        var itemsContainers = document.querySelectorAll('.js-popup-wishlist-items');
        var contentWrappers = document.querySelectorAll('.js-popup-wishlist-content-wrapper');
        var emptyMessages = document.querySelectorAll('.js-popup-wishlist-empty');

        if (items.length === 0) {
            itemsContainers.forEach(function (c) { c.innerHTML = ''; });
            contentWrappers.forEach(function (w) { w.classList.add('d-none'); });
            emptyMessages.forEach(function (m) { m.classList.remove('d-none'); });
        } else {
            emptyMessages.forEach(function (m) { m.classList.add('d-none'); });
            contentWrappers.forEach(function (w) { w.classList.remove('d-none'); });

            var itemsHtml = '';
            items.forEach(function (item) {
                var imgSrc = item.imageUrl || '/images/no-image.jpg';
                var productUrl = item.productUrl || ('/Product/Index/' + item.productId);

                itemsHtml += `
                    <div class="product-store-lists d-flex flex-row align-items-start mb-20 pb-15 border-bottom" data-product-id="${item.productId}">
                        <div class="product-store-lists__image mr-15" style="width: 70px; min-width: 70px;">
                            <a href="${productUrl}" class="d-block">
                                <img src="${imgSrc}" alt="${item.productName}" style="width: 100%; height: 75px; object-fit: cover; border-radius: 4px; border: 1px solid #eee;">
                            </a>
                        </div>
                        <div class="product-store-lists__content d-flex flex-column align-items-start flex-grow-1">
                            <div class="product-store-lists__title mb-1">
                                <h3 class="h6 m-0" style="font-size: 13px; font-weight: 600; line-height: 1.3;">
                                    <a href="${productUrl}">${item.productName}</a>
                                </h3>
                            </div>
                            <div class="product-store-lists__price my-1" style="font-size: 13px;">
                                <span class="font-weight-bold text-dark">${item.formattedPrice}</span>
                            </div>
                            <div class="mt-1">
                                <a href="javascript:void(0);" class="product-store-lists__remove js-wishlist-item-remove text-danger d-inline-flex align-items-center" data-id="${item.productId}" style="font-size: 12px; cursor: pointer; text-decoration: none;">
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

        // Sayfadaki kalp ve favori butonlarını güncelle (FAVORİ ÜRÜNÜNÜZ / FAVORİLERE EKLE)
        if (typeof $ !== 'undefined') {
            $('.js-btn-favorite, [data-js-wishlist-button]').each(function () {
                var $btn = $(this);
                var pid = String($btn.data('product-id') || $btn.attr('data-product-id') || '').toLowerCase();
                if (!pid) {
                    var $parent = $btn.closest('[data-product-id], [data-js-product]');
                    if ($parent.length) {
                        pid = String($parent.data('product-id') || $parent.attr('data-product-variant-id') || '').toLowerCase();
                    }
                }
                if (!pid) {
                    var hiddenInput = document.getElementById('product-detail-id');
                    if (hiddenInput) pid = String(hiddenInput.value).toLowerCase();
                }

                var isFav = currentFavorites.indexOf(pid) !== -1;
                var $textSpan = $btn.find('.btn__text');

                if (isFav) {
                    $btn.addClass('active is-favorite');
                    $btn.attr('data-is-fav', 'true');
                    $btn.find('svg path').css('fill', 'red');
                    if ($textSpan.length) {
                        $textSpan.text('FAVORİ ÜRÜNÜNÜZ');
                    }
                    if ($btn[0] && $btn[0]._tippy) {
                        $btn[0]._tippy.setContent('Favori Ürününüz');
                    }
                } else {
                    $btn.removeClass('active is-favorite');
                    $btn.removeAttr('data-is-fav');
                    $btn.find('svg path').css('fill', '');
                    if ($textSpan.length) {
                        $textSpan.text('FAVORİLERE EKLE');
                    }
                    if ($btn[0] && $btn[0]._tippy) {
                        $btn[0]._tippy.setContent('Favorilere Ekle');
                    }
                }
            });
        }
    }

    // Sunucudan Güncel Favorileri Çek
    function fetchFavorites(onSuccess, onError) {
        if (typeof $ === 'undefined') {
            return;
        }

        $.ajax({
            url: '/Favorite/GetFavoritesDetail',
            type: 'GET',
            dataType: 'json',
            success: function (res) {
                if (res && res.success) {
                    renderWishlist(res);
                    if (typeof onSuccess === 'function') onSuccess(res);
                }
            },
            error: function (err) {
                console.warn('fetchFavorites hatası:', err);
                if (typeof onError === 'function') onError(err);
            }
        });
    }

    // Başlatıcı Fonksiyon
    function initFavoriteLogic() {
        if (typeof $ === 'undefined') {
            setTimeout(initFavoriteLogic, 50);
            return;
        }

        $(document).ready(function () {
            // Sayfa açıldığında favorileri yükle
            fetchFavorites();

            // Header'daki Kalp Butonuna Tıklanınca Çekmeceyi Aç
            $(document).on('click', '.header__btn-wishlist, [data-js-popup-button="wishlist"]', function (e) {
                e.preventDefault();
                fetchFavorites(function () {
                    openWishlistDrawer();
                });
            });

            // Çekmece Kapat Butonu
            $(document).on('click', '.js-popup-wishlist-close, [data-js-popup-name="wishlist"] [data-js-popup-close]', function (e) {
                e.preventDefault();
                closeWishlistDrawer();
            });

            // Çekmece Arka Planına Tıklanınca Kapat
            $(document).on('click', '.popup__bg, [data-js-popup-bg]', function () {
                closeWishlistDrawer();
            });

            // Ürün Kartlarındaki / Sayfasındaki Kalp Butonuna Tıklanınca
            $(document).on('click', '.js-btn-favorite', function (e) {
                e.preventDefault();
                var $btn = $(this);
                var productId = $btn.data('product-id');
                if (!productId) return;

                var isFavorite = !$btn.hasClass('active'); // toggle logic

                $.ajax({
                    url: '/Favorite/Toggle',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ ProductId: String(productId), IsFavorite: isFavorite }),
                    success: function (response) {
                        if (response && response.success) {
                            if (isFavorite) {
                                $btn.addClass('active');
                                $btn.find('svg path').css('fill', 'red');
                                // Eklendiğinde çekmeceyi güncelle ve kullanıcıya anında göster
                                fetchFavorites(function () {
                                    openWishlistDrawer();
                                });
                            } else {
                                $btn.removeClass('active');
                                $btn.find('svg path').css('fill', '');
                                fetchFavorites();
                            }
                        }
                    },
                    error: function (err) {
                        if (err.status === 401) {
                            alert("Favorilere eklemek için giriş yapmalısınız.");
                            window.location.href = '/Uyelik/Index';
                        } else {
                            console.error("Favori işlemi başarısız: ", err);
                            alert("Favori işlemi başarısız oldu! (Hata Kodu: " + err.status + ")");
                        }
                    }
                });
            });

            // Çekmece İçindeki "Kaldır" Butonuna Tıklanınca
            $(document).on('click', '.js-wishlist-item-remove', function (e) {
                e.preventDefault();
                var $removeBtn = $(this);
                var productId = $removeBtn.data('id');
                if (!productId) return;

                $.ajax({
                    url: '/Favorite/Remove',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ ProductId: String(productId) }),
                    success: function (response) {
                        if (response && response.success) {
                            // Ekrandaki ürün kartlarının kalp ikonunu söndür
                            $('.js-btn-favorite').each(function () {
                                var $btn = $(this);
                                if (String($btn.data('product-id')).toLowerCase() === String(productId).toLowerCase()) {
                                    $btn.removeClass('active');
                                    $btn.find('svg path').css('fill', '');
                                }
                            });

                            // Listeyi ve sayaçları güncelle
                            fetchFavorites();
                        }
                    },
                    error: function (err) {
                        console.error("Favori silme hatası:", err);
                        alert("Favoriden çıkarılırken bir hata oluştu.");
                    }
                });
            });
        });
    }

    initFavoriteLogic();
})();
