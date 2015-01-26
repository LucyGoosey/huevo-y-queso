import os
import tarfile
import lzma

metas = []

for root, dirs, files in os.walk("Assets"):
    for file in files:
        if ".meta" in file:
            if root == "":
                metas.append(file)
            else:
                metas.append(root + "/" + file)

tarFilename = "ZippedUp.tar.xz"

if(os.path.isfile(tarFilename)):
    print("Old ZippedUp.tar.xz found! Deleting...")
    os.remove(tarFilename)

xzFile = lzma.LZMAFile(tarFilename, mode="w")

with tarfile.open(mode="w", fileobj=xzFile) as tar_xz_file:
    print("Adding project settings...")
    tar_xz_file.add("ProjectSettings/")
    print("Adding scenes...")
    tar_xz_file.add("Assets/Scenes/")
    print("Adding prefabs...")
    tar_xz_file.add("Assets/Prefabs/")
    print("Adding sprites...")
    tar_xz_file.add("Assets/Sprites/")
	
    print("Adding metas...")
    for meta in metas:
        tar_xz_file.add(meta);
	
xzFile.close()
print("All done!")