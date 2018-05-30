#!/bin/bash
cd server
dotnet run -c Release -- -p 50003 &

sleep 15


cd ../client

dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.05 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1249951 -i n_ps-seq-kosarak-lg-0.05-20k
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.05 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i n_ips-seq-kosarak-lg-0.05-20k

dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.10 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1249951 -i n_ps-seq-kosarak-lg-0.10-20k
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.10 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i n_ips-seq-kosarak-lg-0.10-20k

dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.15 -z 20000 -e 0.05 -a PrefixSpan -r 5 -d 1249951 -i n_ps-seq-kosarak-lg-0.15-20k
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.15 -z 20000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i n_ips-seq-kosarak-lg-0.15-20k


dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.05 -z 40000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i n_ips-seq-kosarak-lg-0.05-40k
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.10 -z 40000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i n_ips-seq-kosarak-lg-0.10-40k
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.15 -z 40000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i n_ips-seq-kosarak-lg-0.15-40k

dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.05 -z 60000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i n_ips-seq-kosarak-lg-0.05-60k
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.10 -z 60000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i n_ips-seq-kosarak-lg-0.10-60k
dotnet run -c Release -- -p 50003 -f ~/data/sequence/seq-kosarak-lg.txt -s 0.15 -z 60000 -e 0.05 -a ProSecCo -r 5 -d 1249951 -i n_ips-seq-kosarak-lg-0.15-60k