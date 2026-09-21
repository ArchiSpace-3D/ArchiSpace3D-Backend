import re
import sys

def resolve_file(filepath):
    with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()

    # handle \r\n or \n
    pattern = r'<<<<<<< HEAD\r?\n(.*?)\r?\n=======\r?\n(.*?)\r?\n>>>>>>> .*?(?=\r?\n|$)'
    
    # We want to replace the conflict block with JUST the incoming block (group 2)
    resolved_content = re.sub(pattern, r'\2', content, flags=re.DOTALL)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(resolved_content)

files = [
    r"C:\Users\angel\OneDrive\Desktop\Archie\Backend\ArchiSpace3D.Api\ArchiSpace3D.Api\Controllers\usuarioController.cs",
    r"C:\Users\angel\OneDrive\Desktop\Archie\Backend\ArchiSpace3D.Api\ArchiSpace3D.Api\Models\Usuario.cs",
    r"C:\Users\angel\OneDrive\Desktop\Archie\Backend\ArchiSpace3D.Api\ArchiSpace3D.Api\Program.cs"
]

for f in files:
    resolve_file(f)

print("Conflicts resolved v2.")
