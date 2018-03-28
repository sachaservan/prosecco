#!/bin/bash
cd server
dotnet run -c Release -- -p 50030 &

sleep 15


cd ../client

dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.80 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1700915 -i ps-seq-accidents-lg-0.80
#dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.80 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1700915 -i u_ips-seq-accidents-lg-0.80

#dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.85 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1700915 -i ps-seq-accidents-lg-0.85
#dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.85 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1700915 -i u_ips-seq-accidents-lg-0.85

#dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.90 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1700915 -i ps-seq-accidents-lg-0.90
#dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.90 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1700915 -i u_ips-seq-accidents-lg-0.90

