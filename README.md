# ProSecCo

#### ProSecCo (algorithm implementation)
This project contains the C# implementation of the ProSecCo algorithm for frequent sequence mining. 

To run:  
```
cd src/ProSecCo
dotnet restore
dotnet run -- --help
dotnet run -- --file <file-path> (--support <min-support> | --top-k <k>) --size <sample-size> --db-size <database size>
```
    
#### PrefixSpan (sequence mining algorithm)
This project contains a C# implementation of the PrefixSpan algorithm for frequent sequence mining. Note: this implementation is used by ProSecCo in the sequence mining phase. 

To run:  
```
cd src/PrefixSpan
dotnet restore
dotnet run -- --help
dotnet run -- PrefixSpan --file <file-path> --support <min-support>
```

### Note
Both algorithms expect input files to be in SPMF format. See http://www.philippe-fournier-viger.com/spmf/
