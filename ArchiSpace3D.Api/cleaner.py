import os

def clean_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    
    new_lines = []
    in_head = False
    
    for line in lines:
        if line.startswith('<<<<<<< HEAD'):
            in_head = True
            continue
        if line.startswith('======='):
            in_head = False
            continue
        if line.startswith('>>>>>>>'):
            continue
            
        if not in_head:
            new_lines.append(line)
            
    with open(filepath, 'w', encoding='utf-8') as f:
        f.writelines(new_lines)

files = [
    r"C:\Users\angel\OneDrive\Desktop\Archie\Backend\ArchiSpace3D.Api\ArchiSpace3D.Api\Controllers\usuarioController.cs",
    r"C:\Users\angel\OneDrive\Desktop\Archie\Backend\ArchiSpace3D.Api\ArchiSpace3D.Api\Models\Usuario.cs",
    r"C:\Users\angel\OneDrive\Desktop\Archie\Backend\ArchiSpace3D.Api\ArchiSpace3D.Api\Program.cs"
]

for f in files:
    clean_file(f)

print("Cleaned!")
