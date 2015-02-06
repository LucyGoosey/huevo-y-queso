import os
import hashlib
import tarfile
import lzma

from datetime import datetime

# Grab the original checksums and put them in a dictionary with their filepath as the key
def GetOrigChecksum(filename):
    origChecksums = {};
    
    fChecksum = open(checksumFilename, "r");
    for line in fChecksum:
        sLine = line.split(":") # Separate the path and MD5 checksum
        for path in compress:
            if sLine[0] == path:
                origChecksums[path] = sLine[1][:-1]; # Remove the newline character
    fChecksum.close();
    
    return origChecksums;

# Grabs all the files we might want to compress
def GetFilesToCompress():
    compress = [];
    
    # Grab the path's of the files we need to compress from the Assets folder
    for root, dirs, files in os.walk("Assets"):
        for file in files:
            if file[-3:].lower() != ".cs":  # Compress everything but .cs files
                compress.append((root + "/" + file).replace("\\", "/"));
                    
    # Now grab the project settings
    for root, dirs, files in os.walk("ProjectSettings"):
        for file in files:
            compress.append((root + "/" + file).replace("\\", "/"));
            
    return compress;
    
# Checks the files we might want to compress to see if they have changed, and returns a list of changed files
def GetChangedFiles(compress, origChecksums):
    changed = {};
    
    # For every file we might want to compress...
    for path in compress:
        f = open(path, "rb");
        m = hashlib.md5();
        
        # Generate an MD5 checksum for the file
        while True:
            chunk = f.read(128);
            if chunk == b"":
                break;
            else:
                m.update(chunk);
        f.close();
        cs = m.hexdigest();
        
        # If we have no record of the file, or if the checksum has changed, add it to the changed list
        if (not path in origChecksums)\
        or (origChecksums[path] != str(cs)):
            changed[path] = cs;
        
    return changed;

# Updates the checksum file with the changes to file checksums    
def UpdateChecksumFile(changed):
    # If the checksum file exists, we want to read it...
    if os.path.isfile(checksumFilename):
        fChecksum = open(checksumFilename, "r");
    else:   # otherwise we make a new one
        fChecksum = open(checksumFilename, "w+");
        
    # Create a temporary file to store changes
    newChecksum = open("_" + checksumFilename, "w");
    
    # Check each line of the old checksum file to see if we have a change
    for line in fChecksum:
        flag  = False;
        path = line.split(":")[0];
        
        # And if a file is changed, write the new checksum to the temp file
        if path in changed:
            newChecksum.write(path + ":" + changed[path] + "\n");
            del changed[path];
            flag = True;
                
        # If a file isn't changed, just write the same line to the temp file
        if not flag:
            newChecksum.write(line);
            
    # Record any new files in the checksum file
    for key in changed:
        newChecksum.write(key + ":" + changed[key] + "\n");
                
    fChecksum.close();
    newChecksum.close();
    
    # Remove the old checksum file and replace it with the temp one
    os.remove(checksumFilename);
    os.rename("_" + checksumFilename, checksumFilename);
    
def CreateZipFile():
    # Name the file ProjectName_HourMinuteWeekdayMonthDayMonth and add _Seconds if the file already exists
    filename = "zip_" + os.path.split(os.getcwd())[1] + datetime.now().strftime("-%H%M%a%d%b") + ".tar.xz";
    if os.path.isfile(filename):
        filename =  filename[:-7] + datetime.now().strftime("_%S") + ".tar.xz";
    
    # Open an lzma file to perform compression, and a tar file as we have to compress from memory
    print("\nCreating " + filename);
    xzFile = lzma.LZMAFile(filename, mode="w");
    with tarfile.open(mode="w", fileobj=xzFile) as tarFile:                
        for file in compress:
            print("Adding " + file);
            tarFile.add(file);

        # Update the checksum file with the new checksums
        UpdateChecksumFile(changed)
        # Add this updated checksum to the tarFile
        tarFile.add(checksumFilename);
    xzFile.close()

# The filename of the MD5 checksum store
checksumFilename = ".checksums"

# Get a list of all the files we might compress
compress = GetFilesToCompress()

# Check if we have a checksums file, and if we do grab the checksums
origChecksums = {}
if os.path.isfile(checksumFilename):
    origChecksums = GetOrigChecksum(checksumFilename)

# Check the checksums from the file compared with the new checksums
changed = GetChangedFiles(compress, origChecksums)
compress = changed.keys()

# If we have new files to compress, compress 'em! Otherwise report that nothing has changed.
if len(compress) != 0:
    CreateZipFile()
    print("All done!\n");
else:
    print("No files changed, nothing to zip up!")
     
# We're all done! Nothing to do but wait for the user to press enter, just so they have time to view the lovely output.
if os.name == "nt":
    os.system("pause");
else:
    input("\nPress [Enter] to close...");