#!/usr/bin/python
import csv
import sys

def stats(fin):
    avglen = 0
    items = set()
    median = []
    maxlen = 0
    count = 0
    
    with open(fin) as f:
        for line in f:
            # count stats
            row = line.split("-1")
            items.update(row)
            avglen += len(row)
            median.append(len(row))
            if maxlen < len(row):
                maxlen = len(row)
            count += 1
    
    
    median.sort()
    index = len(median)/2
    medianlen = median[index]

    
    print "Average len: " + str(avglen / float(count))
    print "Median  len: " + str(medianlen)
    print "Max     len: " + str(maxlen)
    print "Num items:   " + str(len(items) - 2)
    print "Num trans:   " + str(count)


if __name__ == "__main__":
    if (len(sys.argv) < 2):
        print "Get info on a sequence data file."
        print "Usage: filestats.py <file>"
        sys.exit(1)

    sys.argv.pop(0)
    fin = sys.argv.pop(0)
    stats(fin)
