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

dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.35 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1022500 -i n_ps-seq-fifa-lg-0.35-20k
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.35 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i n_ips-seq-fifa-lg-0.35-20k

dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.3 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1022500 -i n_ps-seq-fifa-lg-0.3-20k
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.3 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i n_ips-seq-fifa-lg-0.3-20k

dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.4 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1022500 -i n_ps-seq-fifa-lg-0.4-20k
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.4 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i n_ips-seq-fifa-lg-0.4-20k


dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.35 -z 40000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i n_ips-seq-fifa-lg-0.35-40k
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.3 -z 40000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i n_ips-seq-fifa-lg-0.3-40k
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.4 -z 40000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i n_ips-seq-fifa-lg-0.4-40k

dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.35 -z 60000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i n_ips-seq-fifa-lg-0.35-60k
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.3 -z 60000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i n_ips-seq-fifa-lg-0.3-60k
dotnet run -c Release -- -p 50004 -f ~/data/sequence/seq-fifa-lg.txt -s 0.4 -z 60000 -e 0.05 -a ProSecCo -r 5 -d 1022500 -i n_ips-seq-fifa-lg-0.4-60k


