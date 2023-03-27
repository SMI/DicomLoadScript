# DicomLoadScript

This is a simple load script for iteratively calling the [RDMP DLE](https://github.com/HicServices/RDMP) with monthly batches of dicom images.

It expects folders in the following hierarchy

```
yyyy
  └ mm
     └ dd
```

for example 

```
2010
  └ 01
     └ 01
     └ 02
     └ 03
```

Each day folder should contain dicom files.  This script does not work alone and requires [RDMP](https://github.com/HicServices/RDMP) and [RDMP.Dicom Plugin](https://github.com/SMI/RdmpDicom) to work
