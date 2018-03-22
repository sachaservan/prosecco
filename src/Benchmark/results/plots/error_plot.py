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

def loadRawData(d, fn):
    data = json.load(open(fn))
    run_cnt = 0
    for run in data['Runs']:
        errors = run['NormalizedErrorsPerBatch']
        
        for i in range(len(errors)):
            d['NormalizedError'].append(np.mean(errors[i]))
            d['Batch'].append(i)
            d['Run'].append(run_cnt)
        run_cnt += 1


def plot_ts(df, ax, color, label):
    valid_y1 = []
    valid_y2 = []
    valid_x = []
    line_x = []
    line_y = []
    single_x = []
    single_y = []
    count_never = True
    max_batch = df['Batch'].max() + 1
    min_batch = df['Batch'].min()
    w = 1
    for b in range(min_batch, max_batch, w):
        q = df.query('Batch >= ' + str(b) + ' and Batch < ' + str(b + w))
        mean = q[label].mean()
        std = q[label].std()
        count = q[label].count()
        if count > 1:
            valid_x.append(b)
            valid_y1.append(mean - 1.96 * (std / math.sqrt(count)))
            valid_y2.append(mean + 1.96 * (std / math.sqrt(count)))
            line_y.append(mean)
            line_x.append(b)    
    
        
    #print(df)
    ax1.fill_between(valid_x, valid_y1, valid_y2, facecolor=color, alpha=0.2, interpolate=True)
    #ax1.plot(single_x, single_y, marker='o',color=color, linewidth=0, markersize=8)
    
    ax1.plot(line_x, line_y, color=color, linewidth=1, label=label)
    #df = pd.DataFrame({'value': np.random.rand(31), 'time': range(31)})
    #df['subject'] = 0
    #sns.tsplot(data=df, value='value', time='time', unit='subject', ax=ax1)


f, (ax1) = plt.subplots(1, 1, sharey=True, figsize=(5.5, 3))    


d = {'Batch': [], 'Run': [], 'NormalizedError' : []}
loadRawData(d, '../ips-seq-kosarak-lg-0.05.json')

df = pd.DataFrame(data=d)   
df = df.sort(['Batch'])
plot_ts(df, ax1, flatui[0], 'NormalizedError')
print(df)

#plt.ylim([0.0, 1.1])
ax1.set_ylabel('')
ax1.set_xlabel('Iteration')
plt.title('bms1_spmf_200')

plt.legend()

plt.show()