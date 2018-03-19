#!/bin/bash
cd server
dotnet restore
dotnet build -c Release

cd ../client
dotnet restore
dotnet build -c Release


cd ../server
dotnet run  -c Release -- -p 50006 &

sleep 15


cd ../client

dotnet run -c Release -- -p 50006 -f ~/data/sequence/seq-bible-lg.txt -s 0.40 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 5455350 -i ps-seq-bible-lg-0.40
dotnet run -c Release -- -p 50006 -f ~/data/sequence/seq-bible-lg.txt -s 0.40 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 5455350 -i u_ips-seq-bible-lg-0.40

dotnet run -c Release -- -p 50006 -f ~/data/sequence/seq-bible-lg.txt -s 0.50 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 5455350 -i ps-seq-bible-lg-0.50
dotnet run -c Release -- -p 50006 -f ~/data/sequence/seq-bible-lg.txt -s 0.50 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 5455350 -i u_ips-seq-bible-lg-0.50

dotnet run -c Release -- -p 50006 -f ~/data/sequence/seq-bible-lg.txt -s 0.60 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 5455350 -i ps-seq-bible-lg-0.60
dotnet run -c Release -- -p 50006 -f ~/data/sequence/seq-bible-lg.txt -s 0.60 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 5455350 -i u_ips-seq-bible-lg-0.60

