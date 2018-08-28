# ProSecCo
This project contains the C# implementation of the ProSecCo algorithm for frequent sequence mining. 
See __prosecco.pdf__ for theoretical and experimental results as well as the algorithm description. 

#### Implementation

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

### A note on file formats:
Both algorithms expect input files to be in SPMF format. See http://www.philippe-fournier-viger.com/spmf/


# License

Copyright (c) 2018 Sacha Servan-Schreiber, Matteo Riondato, Emanuel Zgraggen. 

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this library except in compliance with the License. You may obtain a copy of the License at

www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

