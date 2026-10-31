// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace Store.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("ResourceCatalog") { Scopes = { "CatalogFullPermission", "CatalogReadPermission" }, UserClaims = { "role", ClaimTypes.Role, "sub", "name" } },
            new ApiResource("ResourceDiscount") { Scopes = { "DiscountFullPermission" }, UserClaims = { "role", ClaimTypes.Role, "sub", "name" } },
            new ApiResource("ResourceOrder") { Scopes = { "OrderFullPermission" }, UserClaims = { "role", ClaimTypes.Role, "sub", "name" } },
            new ApiResource("ResourceLocation") { Scopes = { "LocationFullPermission" }, UserClaims = { "role", ClaimTypes.Role, "sub", "name" } },
            new ApiResource("ResourceBasket") { Scopes = { "BasketFullPermission" }, UserClaims = { "role", ClaimTypes.Role, "sub", "name" } },
            new ApiResource("ResourceFavorite") { Scopes = { "FavoriteFullPermission" }, UserClaims = { "role", ClaimTypes.Role, "sub", "name" } },
            new ApiResource("ResourceB2B") { Scopes = { "B2BFullPermission" }, UserClaims = { "role", ClaimTypes.Role, "sub", "name" } },
            new ApiResource("ResourceRental") { Scopes = { "RentalFullPermission" }, UserClaims = { "role", ClaimTypes.Role, "sub", "name" } },
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName) { UserClaims = { "role", ClaimTypes.Role, "sub", "name" } }
        };

        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile(),
            new IdentityResources.Phone(),
            new IdentityResource("roles", "Kullanıcı Rolleri", new[] { "role", ClaimTypes.Role })
        };

        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            new ApiScope("CatalogFullPermission","Catalog API için full erişim yetkisi vardır."),
            new ApiScope("CatalogReadPermission","Catalog API için okuma yetkisi vardır."),            
            new ApiScope("DiscountFullPermission","Discount API için full erişim yetkisi vardır."),
            new ApiScope("OrderFullPermission","Order API için full erişim yetkisi vardır."),
            new ApiScope("LocationFullPermission","Location API için full erişim yetkisi vardır."),
            new ApiScope("BasketFullPermission","Basket API için full erişim yetkisi vardır."),
            new ApiScope("FavoriteFullPermission","Favorite API için full erişim yetkisi vardır."),
            new ApiScope("B2BFullPermission","B2B API için full erişim yetkisi vardır."),
            new ApiScope("RentalFullPermission","Rental API için full erişim yetkisi vardır."),
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName,"Yerel API için erişim yetkisi vardır.")
        };

        public static IEnumerable<Client> Clients => new Client[]
        {
            // Ziyaretçi Client (Kimliksiz katalog ve konum okuma)
            new Client
            {
                ClientId = "StoreVisitorId",
                ClientName = "Store Visitor User",
                ClientSecrets = { new Secret("store_visitor_secret_2026!".Sha256()), new Secret("storesecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "LocationFullPermission", "CatalogReadPermission" }
            },

            // WebUI B2C Müşteri Client'ı (Admin yetkileri ve LocalApi izole edilmiştir)
            new Client
            {
                ClientId = "StoreWebClientId",
                ClientName = "Store Web Client (B2C Customers)",
                ClientSecrets = { new Secret("store_web_secret_2026!".Sha256()), new Secret("storesecret".Sha256()) },
                AllowedGrantTypes = { GrantType.ResourceOwnerPassword, GrantType.Implicit },
                RedirectUris = { "http://localhost:5079/Uyelik/ExternalLoginCallback" },
                AllowAccessTokensViaBrowser = true,
                AllowedScopes = {
                    "CatalogReadPermission",
                    "DiscountFullPermission",
                    "OrderFullPermission",
                    "LocationFullPermission",
                    "BasketFullPermission",
                    "FavoriteFullPermission",
                    "RentalFullPermission",
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Phone,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles"
                },
                AllowOfflineAccess = true,
                AccessTokenLifetime = 24 * 60 * 60 // 1 gün
            },
                    
            // B2B Bayi Client'ı
            new Client
            {
                ClientId = "StoreB2BClientId",
                ClientName = "Store B2B Dealer User",
                ClientSecrets = { new Secret("store_b2b_secret_2026!".Sha256()), new Secret("storesecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {
                    "CatalogReadPermission",
                    "DiscountFullPermission",
                    "OrderFullPermission",
                    "LocationFullPermission",
                    "BasketFullPermission",
                    "B2BFullPermission",
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Phone,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles"
                },
                AllowOfflineAccess = true,
                AccessTokenLifetime = 24 * 60 * 60
            },

            // Admin Client'ı (Yalnızca yönetim paneli için tam yetkili)
            new Client
            {
                ClientId = "StoreAdminId",
                ClientName = "Store Admin User",
                ClientSecrets = { new Secret("store_admin_secret_2026_super!".Sha256()), new Secret("storesecret".Sha256()) },
                AllowedGrantTypes = { GrantType.ResourceOwnerPassword, GrantType.Implicit },
                RedirectUris = { "http://localhost:5079/Uyelik/ExternalLoginCallback" },
                AllowAccessTokensViaBrowser = true,
                AllowedScopes = {
                    "CatalogFullPermission",
                    "CatalogReadPermission",
                    "DiscountFullPermission",
                    "OrderFullPermission",
                    "LocationFullPermission",
                    "BasketFullPermission",
                    "FavoriteFullPermission",
                    "B2BFullPermission",
                    "RentalFullPermission",
                    IdentityServerConstants.LocalApi.ScopeName,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Phone,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles"
                },
                AllowOfflineAccess = true,
                AccessTokenLifetime = 30 * 24 * 60 * 60
            }
        };
    }
}
