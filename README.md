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

### A note on file formats:
Both algorithms expect input files to be in SPMF format. See http://www.philippe-fournier-viger.com/spmf/


# License

Copyright (c) 2018 Sacha Servan-Schreiber

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
