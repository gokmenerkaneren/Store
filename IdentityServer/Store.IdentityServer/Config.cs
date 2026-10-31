// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Store.IdentityServer
{
    public static class Config
    {

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("ResourceCatalog"){Scopes={"CatalogFullPermission", "CatalogReadPermission"}},
            new ApiResource("ResourceDiscount"){Scopes={"DiscountFullPermission"}},
            new ApiResource("ResourceOrder"){Scopes={"OrderFullPermission"}},
            new ApiResource("ResourceLocation"){Scopes={"LocationFullPermission"}},
            new ApiResource("ResourceBasket"){Scopes={"BasketFullPermission"}},
            new ApiResource("ResourceFavorite"){Scopes={"FavoriteFullPermission"}},
            new ApiResource("ResourceB2B"){Scopes={"B2BFullPermission"}},
            new ApiResource("ResourceRental"){Scopes={"RentalFullPermission"}},
            // IdentityServerConstants.LocalApi.ScopeName ile yerel API için erişim yetkisi veriyorum.
            // Bu sayede IdentityServer kendi tokenını alabiliyor.
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName) //Yerelde token alıyorum.
        };
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId(),      //Herkese açık olan ID ye erişim sağlayabilsin.
            new IdentityResources.Email(),       //Email bilgisine erişim sağlayabilsin.
            new IdentityResources.Profile(),     //Profil bilgisine erişim sağlayabilsin.
            new IdentityResources.Phone()        //Telefon bilgisine erişim sağlayabilsin.
        };

        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            // Catalog API için erişim yetkileri düzenleme.
            new ApiScope("CatalogFullPermission","Catalog API için full erişim yetkisi vardır."),
            new ApiScope("CatalogReadPermission","Catalog API için okuma yetkisi vardır."),            
            
            // Discount API için erişim yetkileri düzenleme.
            new ApiScope("DiscountFullPermission","Discount API için full erişim yetkisi vardır."),

            // Order API için erişim yetkileri düzenleme.
            new ApiScope("OrderFullPermission","Order API için full erişim yetkisi vardır."),

            // Location API için erişim yetkileri düzenleme.
            new ApiScope("LocationFullPermission","Location API için full erişim yetkisi vardır."),

            // Basket API için erişim yetkileri düzenleme.
            new ApiScope("BasketFullPermission","Basket API için full erişim yetkisi vardır."),
            
            // Favorite API için erişim yetkileri düzenleme.
            new ApiScope("FavoriteFullPermission","Favorite API için full erişim yetkisi vardır."),

            // B2B API için erişim yetkileri
            new ApiScope("B2BFullPermission","B2B API için full erişim yetkisi vardır."),

            // Rental API için erişim yetkileri
            new ApiScope("RentalFullPermission","Rental API için full erişim yetkisi vardır."),

            new ApiScope(IdentityServerConstants.LocalApi.ScopeName,"Yerel API için erişim yetkisi vardır.")
        };

        public static IEnumerable<Client> Clients => new Client[]
        {
            // Visitor ile kullanıcı girişi yapmadan sadece katalog ve indirimleri görüntüleyebilsin.
            new Client
            {
                ClientId="StoreVisitorId",
                ClientName="Store Visitor User",
                ClientSecrets={new Secret("storesecret".Sha256())},
                AllowedGrantTypes=GrantTypes.ClientCredentials,
                AllowedScopes={ "LocationFullPermission" }
            },
                    
            // Manager kullanıcı girişi yapabilsin ve tüm API lere erişim sağlayabilsin.
            new Client
            {
                ClientId="StoreManagerId",
                ClientName="Store Manager User",
                ClientSecrets={ new Secret("storesecret".Sha256())},
                AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                AllowedScopes={ "LocationFullPermission", "DiscountFullPermission" }
            },

            // Admin kullanıcı girişi yapabilsin ve tüm API lere erişim sağlayabilsin.
            new Client
            {
                ClientId="StoreAdminId",
                ClientName="Store Admin User",
                ClientSecrets={ new Secret("storesecret".Sha256())},
                // ROPC for standard login, Implicit for external provider callback
                AllowedGrantTypes = { GrantType.ResourceOwnerPassword, GrantType.Implicit },
                RedirectUris = { "http://localhost:5079/Uyelik/ExternalLoginCallback" },
                AllowAccessTokensViaBrowser = true,
                AllowedScopes={ "CatalogFullPermission","CatalogReadPermission","DiscountFullPermission",
                                "OrderFullPermission","LocationFullPermission","BasketFullPermission",
                                "FavoriteFullPermission","B2BFullPermission","RentalFullPermission",
                    IdentityServerConstants.LocalApi.ScopeName,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Phone,
                    IdentityServerConstants.StandardScopes.Profile
                    },
                AllowOfflineAccess = true,
                AccessTokenLifetime = 30 * 24 * 60 * 60 // 30 gün boyunca geçerli oturum
            }
        };
    }
}