import sys

if len(sys.argv) < 3:
	print('specify input and output file: spmf_formatter <in_file> <out_file> -ibm <if input is from IBM data gen>')

else:
	ibm_format = False
	if len(sys.argv) > 3 and sys.argv[3] == '-ibm':
		ibm_format = True

	with open(sys.argv[1], 'r') as ins:
		with open(sys.argv[2], 'w') as outs:
			for line in ins:
				entries  = line.strip().split(' ' )
				transactions = []
				if not ibm_format:
					transactions = entries
				else:
					entries = entries[1:]
					while len(entries) > 0:
						c = int(entries[0])
						entries = entries[1:]
						items = []
						for i in range(c):
							items.append(entries[i])
						entries = entries[c:]
						transactions.append(' '.join(items))
						
				
				outs.write(' -1 '.join(transactions) + ' -1 -2\n')