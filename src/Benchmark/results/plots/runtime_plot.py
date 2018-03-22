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

sns.set(context="paper", style={'axes.axisbelow': True,
    'axes.edgecolor': '.8',
    'axes.facecolor': 'white',
    'axes.grid': True,
    'axes.labelcolor': '.15',
    'axes.linewidth': 0.5,
    'figure.facecolor': 'white',
    'font.family': [u'sans-serif'],
    'font.sans-serif': [u'Abel'],
    'font.weight' : u'light',
    'grid.color': '.8',
    'grid.linestyle': u'-',
    'image.cmap': u'Greys',
    'legend.frameon': False,
    'legend.numpoints': 1,
    'legend.scatterpoints': 1,
    'lines.solid_capstyle': u'butt',
    'text.color': '.15',
    'xtick.color': '.15',
    'xtick.direction': u'out',
    'xtick.major.size': 0.0,
    'xtick.minor.size': 0.0,
    'ytick.color': '.15',
    'ytick.direction': u'out',
    'ytick.major.size': 0.0,
    'ytick.minor.size': 0.0}, font_scale = 1.3)

flatui = ["#28aad5", "#b24d94", "#38ae97" ,"#ec7545"]

def minutes_second_formatter(value, tick_number):
    m, s = divmod(value, 60)
    return "%02d:%02d" % (m, s)

def loadRawData(d, fn, algorithm, dataset):
    data = json.load(open(fn))
    run_cnt = 0
    for run in data['Runs']:
        runtime = run['TotalRuntimeInMillis']
        
        d['Dataset'].append(dataset)
        d['Algorithm'].append(algorithm)
        d['Runtime'].append(runtime / 1000.0)
        d['Run'].append(run_cnt)
        run_cnt += 1


f, (ax1) = plt.subplots(1, 1, sharey=True, figsize=(5.5, 3))    


d = {'Runtime': [], 'Run': [], 'Algorithm' : [], 'Dataset' : []}

#loadRawData(d, 'ps_bms1_spmf_200_0.05.json', 'PrefixSpan', 'BMS1_200')
#loadRawData(d, 'ips_bms1_spmf_200_0.05.json', 'IPrefixSpan', 'BMS1_200')

loadRawData(d, '../ps-seq-bms-lg-0.01.json', 'PrefixSpan', 'KOSARAK')
loadRawData(d, '../ips-seq-bms-lg-0.01.json', 'IPrefixSpan', 'KOSARAK')

df = pd.DataFrame(data=d)   
print(df)

sns.boxplot(x="Dataset", y="Runtime", hue="Algorithm", data=df, palette={"PrefixSpan": flatui[0], "IPrefixSpan": flatui[1]})
ax1.yaxis.set_major_formatter(plt.FuncFormatter(minutes_second_formatter))
plt.legend()

plt.show()