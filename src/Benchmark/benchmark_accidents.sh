#!/bin/bash
cd server
dotnet run -c Release -- -p 50030 &

sleep 15


cd ../client

dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.70 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1249951 -i ps-seq-accidents-lg-0.70
dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.70 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i u_ips-seq-accidents-lg-0.70

dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.75 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1249951 -i ps-seq-accidents-lg-0.75
dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.75 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i u_ips-seq-accidents-lg-0.75

dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.80 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1249951 -i ps-seq-accidents-lg-0.80
dotnet run -c Release -- -p 50030 -f ~/data/sequence/seq-accidents-lg.txt -s 0.80 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i u_ips-seq-accidents-lg-0.80
