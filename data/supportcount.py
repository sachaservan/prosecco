#!/usr/bin/python

import csv
import sys

def count(f, items):
    with open(f, 'rb') as f:
        reader = csv.reader(f)
        count = 0
        for row in reader:
            match = True
            for item in items:
                if item not in row:
                    match = False
                    break
            
            if match:
                count += 1

    print count


if __name__ == "__main__":
    if (len(sys.argv) < 3):
        print "Usage: supportcount.py <file> <item 1>...<item n>"
        sys.exit(1)

    sys.argv.pop(0)
    f = sys.argv.pop(0)
    count(f, sys.argv)
