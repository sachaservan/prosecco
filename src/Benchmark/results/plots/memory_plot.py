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

def loadRawData(d, fn, algorithm):
    data = json.load(open(fn))
    run_cnt = 0
    step = 10000
    for run in data['Runs']:
        bt = run['MemoryUsagePerTimeInMillis']
        interval = 0
        interval_values = []
        times = [float(t) for t in bt]
        min_t = 0
        max_t = np.max(times)

        for bin in np.arange(0, max_t, step):
            values = []
            for t in bt:
                tf = float(t)
                gb = bt[t] / 1073741824.0
                
                if tf < bin and tf >= bin - step:
                    values.append(gb)
                    
            if len(values) > 0:
                d['Run'].append(run_cnt)
                #date = utc_time = datetime.datetime(1970, 1, 1) + datetime.timedelta(milliseconds=(interval + step))
                #print(date)
                d['Time'].append(int((bin - step) / 1000.0))
                d['Memory (GB)'].append(np.mean(values))
                d['Algorithm'].append(algorithm)
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
    times = [float(t) for t in df['Time']]
    for t in times:
        mean = df.query('Time == ' + str(t))['Memory (GB)'].mean()
        std = df.query('Time == ' + str(t))['Memory (GB)'].std()
        count = df.query('Time == ' + str(t))['Memory (GB)'].count()
        if count > 1:
            valid_x.append(t)
            valid_y1.append(mean - 1.96 * (std / math.sqrt(count)))
            valid_y2.append(mean + 1.96 * (std / math.sqrt(count)))
            line_y.append(mean)
            line_x.append(t)    
    
        
    #print(df)
    ax1.fill_between(valid_x, valid_y1, valid_y2, facecolor=color, alpha=0.2, interpolate=True)
    #ax1.plot(single_x, single_y, marker='o',color=color, linewidth=0, markersize=8)
    
    ax1.plot(line_x, line_y, color=color, linewidth=1, label=label)
    #df = pd.DataFrame({'value': np.random.rand(31), 'time': range(31)})
    #df['subject'] = 0
    #sns.tsplot(data=df, value='value', time='time', unit='subject', ax=ax1)


f, (ax1) = plt.subplots(1, 1, sharey=True, figsize=(5.5, 3))    


d = {'Time': [], 'Memory (GB)': [], 'Run': [], 'Algorithm' : []}
loadRawData(d, '../ps-seq-kosarak-lg-0.05.json', 'PrefixSpan')
df = pd.DataFrame(data=d)   
df = df.sort(['Time'])
plot_ts(df, ax1, flatui[0], 'PrefixSpan')
print(df)

d = {'Time': [], 'Memory (GB)': [], 'Run': [], 'Algorithm' : []}
loadRawData(d, '../ips-seq-kosarak-lg-0.05.json', 'ProSecCo')
df = pd.DataFrame(data=d)   
df = df.sort(['Time'])
plot_ts(df, ax1, flatui[1], 'ProSecCo')
print(df)

ax1.xaxis.set_major_formatter(plt.FuncFormatter(minutes_second_formatter))

plt.legend()
plt.title('kosarak')

plt.show()