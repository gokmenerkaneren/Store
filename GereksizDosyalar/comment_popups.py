import os

files = [
    r"C:\User\Projects\Store\FrontEnds\Store.WebUI\Views\Shared\Components\_defScriptsUILayoutComponentPartial\Default.cshtml",
    r"C:\User\Projects\Store\FrontEnds\Store.WebUI\Views\Shared\Components\_proPopupUILayoutComponentPartial\Default.cshtml"
]

for file_path in files:
    with open(file_path, "r", encoding="utf-8") as f:
        content = f.read()

    # 1. Newsletter popup
    sub_start_tag = 'data-js-popup-name="subscription"'
    if sub_start_tag in content:
        # Find opening <div before this tag
        idx_attr = content.find(sub_start_tag)
        idx_start = content.rfind("<div", 0, idx_attr)
        # Find closing </div> and Loader.require for popup_subscription
        marker_sub = 'name: "popup_subscription"'
        idx_marker = content.find(marker_sub, idx_attr)
        idx_end = content.find("</div>", idx_marker) + len("</div>")
        
        sub_block = content[idx_start:idx_end]
        replacement = f"<!-- Newslette Popup Başlangıcı -->\n        @*\n        {sub_block}\n        *@"
        content = content[:idx_start] + replacement + content[idx_end:]
        print(f"Newsletter commented in {os.path.basename(file_path)}")
    else:
        print(f"Newsletter not found in {os.path.basename(file_path)}")

    # 2. Product popup
    prod_marker = 'js-notification-products'
    if prod_marker in content:
        idx_prod = content.find(prod_marker)
        # Find outer <div class="notification position-relative js-notification">
        outer_div = content.rfind('<div class="notification position-relative js-notification">', 0, idx_prod)
        # Find loader require notifications
        marker_notif = 'name: "notifications"'
        idx_notif = content.find(marker_notif, idx_prod)
        idx_p_end = content.find("</script>", idx_notif) + len("</script>")
        
        prod_block = content[outer_div:idx_p_end]
        replacement_p = f"<!-- Ürün Popup -->\n            @*\n            {prod_block}\n            *@"
        content = content[:outer_div] + replacement_p + content[idx_p_end:]
        print(f"Product popup commented in {os.path.basename(file_path)}")
    else:
        print(f"Product popup not found in {os.path.basename(file_path)}")

    with open(file_path, "w", encoding="utf-8") as f:
        f.write(content)

print("Completed successfully!")

