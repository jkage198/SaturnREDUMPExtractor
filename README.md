# SaturnREDUMPExtractor
Quick tool to extract REDUMP Saturn set.

Supports/generates CSV file containing list of all archives. Rows set to TRUE will be batch extracted.
Supports multi-disc extraction under a single folder.
Supports splitting by region sub-folders.


Usage:
(run exe from same directory as zippped games)

1. Extract all games from current working directory to target directory, split by category<br />
```SaturnREDUMPExtractor.exe -extractAll -o F:\Saturn -r```

2. Generate game list CSV file<br />
```SaturnREDUMPExtractor.exe -generateCSV mygamelist.csv```

3. Extract all games from current working directory to target directory based on input CSV file, split by category<br />
```SaturnREDUMPExtractor.exe -extract -s mygamelist.csv -o F:\Saturn -r```


Examples

Extract every zip file in the REDUMP folder to path F:\Saturn
1. Copy executable into REDUMP folder
2. Run executable through command line using the following:<br/>
```SaturnREDUMPExtractor.exe -extractAll -o F:\Saturn```

Extract only some zip files from the REDUMP to path F:\Saturn and categorize by region
1. Copy executable into REDUMP folder
2. Run executable through command line using the following:<br/>
```SaturnREDUMPExtractor.exe -generateCSV mygamelist.csv```
3. Open mygamelist.csv with your editor of choice and set to TRUE or FALSE the games to extract. Save and close.
4. Run executable through command line using the following:<br/>
```SaturnREDUMPExtractor.exe -extract -s mygamelist.csv -o F:\Saturn -r```

