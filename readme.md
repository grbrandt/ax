# Ax Application

## Purpose
The Ax application is a command-line tool designed to create and extract archives using zip compression.
It is specifically made to archive folders with a large number of duplicate files and uses the high-performance `xxHash64` algorithm to detect duplicates.

It was originally created to archive the modules folder of an AXIN installation, and if no source folder is specified for the pack command the application attempts to find the default installation folder for AXIN.

## Features
- **Pack**: Create an archive from a specified folder.
- **Unpack**: Extract an archive to a specified destination folder.

## Usage

### Pack Command
The `pack` command is used to create an archive from a specified folder.

#### Syntax
```
ax pack [sourcefolder] [options] 
```

#### Options
- `-o, --output` (Optional): Output file name (defaults to `axin-dd-MM-yyyy-HH-mm.axz`).

#### Examples
- Pack the default folder:
 ```ax pack```
- Pack a specified folder:
 ```ax pack c:\myfolder```
- Pack a specified folder with a custom output file name:
   ```ax pack c:\myfolder -o myfolder.axz```
### Unpack Command
The `unpack` command is used to extract an archive to a specified destination folder.

#### Syntax
```
ax unpack <inputFile> [options]
```

#### Options
- `-o, --output` (Optional): Destination folder.

#### Examples
- Unpack an archive to the current working folder:
   ```ax unpack c:\temp\myarchive.axz```
- Unpack an archive to a specified output folder:
   ```ax unpack c:\temp\myarchive.axz -o c:\target```
- 
## Authors
- Geir Rune Brandt

## License
This project is licensed under the MIT License.
