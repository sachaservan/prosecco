import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import matplotlib    
import matplotlib.pyplot as plt
import seaborn as sns
import numpy as np
import math
import json
import datetime
import matplotlib.dates as mdates
import os.path
import pickle


def loadBreakdown(fn):
    d = {'ReadingAndParsing': 0, 'PrefixSpan': 0, 'SubsequenceMatching' : 0}
    data = json.load(open(fn))
    print(fn)
    run_cnt = 0
    for run in data['Runs']:
        a = run['LastBlockFileReadingAndParsingTimePerBatch']
        b = run['LastBlockPrefixSpanRuntimePerBatch']
        c = run['LastBlockSubsequenceMatchingRuntimePerBatch']
        batch = run['RuntimePerBatch']
        runtime = run['TotalRuntimeInMillis']

        sum = np.array(a).sum() + np.array(b).sum() + np.array(c).sum()
        d['ReadingAndParsing'] = np.array(a).sum() / sum
        d['PrefixSpan'] = np.array(b).sum() / sum
        d['SubsequenceMatching'] = np.array(c).sum() / sum
        #print('--')
        #print(np.array(a).sum(), np.array(b).sum(), np.array(c).sum())
        #print(np.array(a).sum() + np.array(b).sum() + np.array(c).sum())
        #print(runtime)
        #for i in range(len(a)):
        #    #print((a[i] + b[i] + c[i]), batch[i])
        #run_cnt += 1    

    return d



def main(dict, id, file_id, title, show):
    prefixspan = '../ps-' + id + '.json'
    iprefixspan = '../u_ips-' + id + '.json'

    d = loadBreakdown(iprefixspan)
    dict['ReadingAndParsing'].append(d['ReadingAndParsing'])    
    dict['PrefixSpan'].append(d['PrefixSpan'])
    dict['SubsequenceMatching'].append(d['SubsequenceMatching'])
    print (d)

if __name__== '__main__':
    show = False
    d = {'ReadingAndParsing' : [], 'PrefixSpan' : [], 'SubsequenceMatching' : []}
    
    #main('seq-accidents-lg-0.80', 'accidents-5-0_80', 'ACCIDENTS-0.80', show)
    #main('seq-accidents-lg-0.85', 'accidents-5-0_85', 'ACCIDENTS-0.85', show)
    #main('seq-accidents-lg-0.90', 'accidents-5-0_90', 'ACCIDENTS-0.90', show)

    #main('seq-bms-lg-0.03', 'bms-100-0_03', 'BMS-0.03', show)
    #main('seq-bms-lg-0.04', 'bms-100-0_04', 'BMS-0.04', show)
    #main('seq-bms-lg-0.05', 'bms-100-0_05', 'BMS-0.05', show)

    main(d, 'seq-kosarak-lg-0.05', 'kosarak-50-0_05', 'KORSARAK-0.05', show)
    main(d, 'seq-kosarak-lg-0.10', 'kosarak-50-0_10', 'KORSARAK-0.10', show)
    main(d, 'seq-kosarak-lg-0.15', 'kosarak-50-0_15', 'KORSARAK-0.15', show)

    #main('seq-sign-lg-0.40', 'sign-200-0_40', 'SIGN-0.40', show)
    #main('seq-sign-lg-0.50', 'sign-200-0_50', 'SIGN-0.50', show)
    #main('seq-sign-lg-0.60', 'sign-200-0_60', 'SIGN-0.60', show)
    
    #main('seq-bible-lg-0.40', 'bible-200-0_40', 'BIBLE-0.40', show)
    #main('seq-bible-lg-0.50', 'bible-200-0_50', 'BIBLE-0.50', show)
    #main('seq-bible-lg-0.60', 'bible-200-0_60', 'BIBLE-0.60', show)
    
    #main('seq-fifa-lg-0.3', 'fifa-50-0_30', 'FIFA-0.30', show)
    #main('seq-fifa-lg-0.35', 'fifa-50-0_35', 'FIFA-0.35', show)
    #main('seq-fifa-lg-0.4', 'fifa-50-0_40', 'FIFA-0.40', show)

    for k in d:
        print(k, np.array(d[k]).mean())

        