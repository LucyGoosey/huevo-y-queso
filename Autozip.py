import os
import tarfile
import lzma

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
	
xzFile.close()
print("All done!")