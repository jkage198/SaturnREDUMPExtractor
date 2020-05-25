# SaturnREDUMPExtractor
Quick tool to extract REDUMP Saturn set.

Supports/generates CSV file containing list of all archives. Archives set to TRUE will be batch extracted.
Supports multi-disc extraction under a single folder.
Supports splitting by region sub-folders.


Typical Usage:
(run exe from same directory as zippped games)

1. Extract all games from current working directory to target directory, split by category
```SaturnREDUMPExtractor.exe - extractAll -o F:\Saturn -r```

2. Generate game list CSV file
```SaturnREDUMPExtractor.exe -generateCSV mygamelist.csv```

3. Extract all games from current working directory to target directory based on input CSV file, split by category
```SaturnREDUMPExtractor.exe -extract -s mygamelist.csv -o F:\Saturn -r```
