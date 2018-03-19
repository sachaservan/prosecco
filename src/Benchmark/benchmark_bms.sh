#!/bin/bash
cd server
dotnet restore
dotnet build -c Release

cd ../client
dotnet restore
dotnet build -c Release


cd ../server
dotnet run -c Release -- -p 50002 &

sleep 15


cd ../client
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.05 -z 20000 -e 0.03 -a PrefixSpan -r 5 -d 5960001 -i ps-seq-bms-lg-0.05
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.05 -z 20000 -e 0.03 -a ProSecCo -r 5 -d 5960001 -i u_ips-seq-bms-lg-0.05

dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.03 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 5960001 -i ps-seq-bms-lg-0.03
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.03 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i u_ips-seq-bms-lg-0.03

dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.04 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 5960001 -i ps-seq-bms-lg-0.04
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.04 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i u_ips-seq-bms-lg-0.04
