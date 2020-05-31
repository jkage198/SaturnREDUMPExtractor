# SaturnREDUMPExtractor
Quick tool to extract REDUMP Saturn/Dreamcast zipped/7zipped set.

* *Supports/generates CSV file containing list of all archives. Rows set to TRUE will be batch extracted.*
* *Supports multi-disc extraction under a single folder.*
* *Supports splitting by region sub-folders (EU/US/JP/Other).*
<br/>

**Requirement:**
.Net Core Framework Runtime
<br/>

**Usage:**

(run exe from same directory as zippped games)

1. Extract all games from source directory to target directory, split by category<br />
```SaturnREDUMPExtractor.exe extractAll -i <source directory> -o <target directory> -r```

2. Generate game list CSV file<br />
```SaturnREDUMPExtractor.exe generateCSV -i <source directory> -c mygamelist.csv```

3. Extract all games from source directory to target directory based on input CSV file, split by category<br />
```SaturnREDUMPExtractor.exe extract -i <source> -o <target> -s mygamelist.csv -r```
<br/>

**Examples**

To extract every zip file in the REDUMP folder to path F:\Saturn
1. Run executable through command line using the following:<br/>
```SaturnREDUMPExtractor.exe -extractAll -i C:\REDUMP -o F:\Saturn```

Use a CSV to extract only select zip files from the REDUMP to path F:\Saturn and categorize by region
1. Run executable through command line using the following:<br/>
```SaturnREDUMPExtractor.exe -generateCSV -i C:\REDUMP -c mygamelist.csv```
2. Open mygamelist.csv with your editor of choice and set to TRUE or FALSE the games to extract. Save and close.

![CSV example](https://user-images.githubusercontent.com/3223801/83320953-6988ab00-a24c-11ea-9c61-093e0b21817f.PNG "CSV Example")


3. Run executable through command line using the following:<br/>
```SaturnREDUMPExtractor.exe -extract -i C:\REDUMP -o F:\Saturn -s mygamelist.csv -r```

