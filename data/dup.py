#!/usr/bin/python
import csv
import sys

def convert(infile, outfile, n):
    with open(infile) as fin, open(outfile, 'w') as fout:
        for line in fin:
            # duplicate each row n times
            for i in range(0, n):
                fout.write(line)


if __name__ == "__main__":
    if (len(sys.argv) < 4):
        print ("Converts a space delimited file to CSV format. \
                Specify integer <n> to duplicate each row n times.")
        print ("Usage: dup.py <file in> <file out> <n>")
        sys.exit(1)

    sys.argv.pop(0)
    fin = sys.argv.pop(0)
    fout = sys.argv.pop(0)
    n = 1
    if  len(sys.argv) > 0:
        n = sys.argv.pop()
    convert(fin, fout, int(n))
