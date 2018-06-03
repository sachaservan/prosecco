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
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.05 -z 20000 -e 0.03 -a PrefixSpan -r 5 -d 5960001 -i n_ps-seq-bms-lg-0.05-20k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.05 -z 20000 -e 0.03 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.05-20k

dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.03 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 5960001 -i n_ps-seq-bms-lg-0.03-20k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.03 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.03-20k

dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.04 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 5960001 -i n_ps-seq-bms-lg-0.04-20k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.04 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.04-20k


dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.05 -z 10000 -e 0.03 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.05-10k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.03 -z 10000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.03-10k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.04 -z 10000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.04-10k

dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.05 -z 40000 -e 0.03 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.05-40k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.03 -z 40000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.03-40k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.04 -z 40000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.04-40k

dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.05 -z 60000 -e 0.03 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.05-60k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.03 -z 60000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.03-60k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.04 -z 60000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.04-60k

dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.05 -z 80000 -e 0.03 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.05-80k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.03 -z 80000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.03-80k
dotnet run -c Release -- -p 50002 -f ~/data/sequence/seq-bms-lg.txt -s 0.04 -z 80000 -e 0.05 -a ProSecCo -r 5 -d 5960001 -i n_ips-seq-bms-lg-0.04-80k
