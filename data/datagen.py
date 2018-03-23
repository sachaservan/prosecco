#!/usr/bin/python
import csv
from random import randint
import sys
import operator

def generate(num_rows):

    with open('dataset.csv', 'w+') as f:
        wr = csv.writer(f, quoting=csv.QUOTE_ALL)
        #wr.writerow(['Operating System', 'Semester Level', 'University'])

        options1 = ["yes", "no", "maybe", "why", "don't know"]

        options2 = ["bread", "bread", "bread", \
        "milk", "eggs", "apples", "oranges", "sugar", \
        "pear", "yogurt", "seeds", "pineapple", "poptarts", \
        "banana", "salad", "meat", "gum"]

        options3 = ["new york", "providence", "providence", \
        "boston", "chicago", "france", "paris", "LA", "SF", "berkeley",\
        "palo alto", "brooklyn", "bronx", "long island", "hamptons", \
        "pawtucket", "mars", "moon", "galaxy", "milky way", \
        "earth", "planet x"]

        options4 = ["monday", "tuesday", "wednesday", \
        "thursday", "friday", "saturday", "sunday"]
    

        options5 = ["wholefoods", "walmart", "traderjoes", \
        "aldi", "pops", "brown", "carfour", "giant eagle", \
        "savers", "shoppers","food court", "natural store", \
        "foodsome", "other"]

        options6 = []

        for x in range(0, 100):
        	options6.append(str(x))

        sets = {}

        # randomly make rows
        for i in range(0, num_rows):
            opt1 = options1[randint(0, len(options1) - 1)]
            opt2 = options2[randint(0, len(options2) - 1)]
            opt3 = options3[randint(0, len(options3) - 1)]
            opt4 = options4[randint(0, len(options4) - 1)]
            opt5 = options5[randint(0, len(options5) - 1)]
            opt6 = options6[randint(0, len(options6) - 1)]

            row = []
            key = "{" 

            row.append(opt1)
            key += opt1 + ", "

            if randint(1, 1) == 1:
            	key += opt4 + ", "
            	row.append(opt4)

            if randint(1, 1) == 1:
            	key += opt2 + ", "
            	row.append(opt2)

            if randint(1, 1) == 1:
            	key += opt3 + ", "
            	row.append(opt3)

            if randint(1, 1) == 1:
    		key += opt6 + ", "
            	row.append(opt6)

            row.append(opt5)
            key += opt5 + "}"
 
            if key in sets.keys():
                sets[key] += 1
            else:
                sets[key] = 1

            wr.writerow(row)

        sorted_sets = sorted(sets.items(), key=operator.itemgetter(1))

        print "\nFrequent Itemsets: "
        for sorted_tuple in sorted_sets:
            print sorted_tuple[0] + \
                    " support: " + str(sorted_tuple[1] / float(num_rows)) + " occurences: " + str(sorted_tuple[1])

if __name__ == "__main__":
    if (len(sys.argv) < 2 or int(sys.argv[1]) == 0):
        print "Usage: ./datagen.py n"
        sys.exit(1)

    if sys.argv[1] < 100:
        print "n must be > 100"
        sys.exit(1)

    generate(int(sys.argv[1]))
