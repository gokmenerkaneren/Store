with open('FrontEnds/Store.WebUI/wwwroot/ZZShella/styles/theme.css', 'r', encoding='utf-8', errors='ignore') as f:
    text = f.read()

import re
matches = [m.group(0) for m in re.finditer(r'[^{}]*popup__body[^{}]*\{[^}]*\}', text)]
for m in matches:
    print(' '.join(m.split()))
