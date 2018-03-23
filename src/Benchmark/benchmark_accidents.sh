#!/bin/bash
cd server
dotnet run -c Release -- -p 50003 &

sleep 15


cd ../client

dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.05 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1249951 -i ps-seq-kosarak-lg-0.05
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.05 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i u_ips-seq-kosarak-lg-0.05

dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.10 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1249951 -i ps-seq-kosarak-lg-0.10
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.10 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i u_ips-seq-kosarak-lg-0.10

dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.15 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1249951 -i ps-seq-kosarak-lg-0.15
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.15 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i u_ips-seq-kosarak-lg-0.15
