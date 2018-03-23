#!/usr/bin/python
import csv
import sys
def count(f):
    with open(f, 'r') as f:
        reader = csv.reader(f)
        count = 0
        for row in reader:
            count += 1
        print(count)


if __name__ == "__main__":
    if (len(sys.argv) < 2):
        print ("Usage: rowcount.py <file>")
        sys.exit(1)

    sys.argv.pop(0)
    f = sys.argv.pop(0)
    count(f)
