#!/bin/bash
cd server
dotnet restore
dotnet build -c Release

cd ../client
dotnet restore
dotnet build -c Release


cd ../server
dotnet run -c Release -- -p 50004 &

sleep 15


cd ../client

dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.35 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1022500 -i ps-seq-fifa-lg-0.35
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.35 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i u_ips-seq-fifa-lg-0.35

dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.3 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1022500 -i ps-seq-fifa-lg-0.3
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.3 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i u_ips-seq-fifa-lg-0.3

dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.4 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1022500 -i ps-seq-fifa-lg-0.4
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.4 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i u_ips-seq-fifa-lg-0.4


