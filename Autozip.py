import os
import tarfile
import lzma

from datetime import datetime

# Name the file ProjectName_HourMinuteWeekdayMonthDayMonth and add _Seconds if the file already exists
filename = "zip_" + os.path.split(os.getcwd())[1] + datetime.now().strftime("-%H%M%a%d%b") + ".tar.xz";
if os.path.isfile(filename):
    filename =  filename[:-7] + datetime.now().strftime("_%S") + ".tar.xz";
print("Creating " + filename + "\n");
    
# Grab the path's of the files we need to compress
compress = [];
for root, dirs, files in os.walk("Assets"):
    for file in files:
        if file[-3:].lower() != ".cs":  # Compress everything but .cs files
                compress.append(root + "/" + file);
for root, dirs, files in os.walk("ProjectSettings"):
    for file in files:
        compress.append(root + "/" + file);

# Open an lzma file to perform compression, and a tar file as we have to compress from memory
xzFile = lzma.LZMAFile(filename, mode="w");
with tarfile.open(mode="w", fileobj=xzFile) as tarFile:                
    for file in compress:
        print("Adding " + file.replace("\\", "/"));
        tarFile.add(file);
xzFile.close();

# We're all done! Nothing to do but wait for the user to press enter, just so they have time to view the lovely output.
print("All done!\n");
input("Press any key to close...");