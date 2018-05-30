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

sns.set(context='paper', style={'axes.axisbelow': True,
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
    'legend.frameon': True,
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
    'ytick.minor.size': 0.0}, font_scale = 2)

flatui = ['#28aad5', '#b24d94', '#38ae97' ,'#ec7545']

def minutes_second_formatter(value, tick_number):
    m, s = divmod(value / 1000.0, 60)
    return '%02d:%02d' % (m, s)

def second_formatter(value, tick_number):
    v =  int(value / 1000.0)
    if value == 0 or v > 0:
        return v
    else:
         return '{:.1f}'.format(value / 1000.0)
        
def loadRawData(fn, dataLabel, yLabel):
    d = {'Batch': [], 'Run': [], yLabel : []}
    data = json.load(open(fn))
    run_cnt = 0
    for run in data['Runs']:
        data = run[dataLabel]
        
        for i in range(len(data)):
            d[yLabel].append(np.mean(data[i]))
            d['Batch'].append(i)
            d['Run'].append(run_cnt)
        run_cnt += 1 
    return d
    
       
def plot_ts(df, ax, color, yLabel, xLabel, width, label, linestyle = '-', showMax = False, displayCumsum = False):
    valid_y1 = []
    valid_y2 = []
    valid_x = []
    line_x = []
    line_y = []
    single_x = []
    single_y = []
    count_never = True
    max_batch = df[xLabel].max() + 1
    min_batch = df[xLabel].min()
    steps = []

    if width > 0:
        steps = range(min_batch, max_batch, width)
    else:
        steps = [t for t in df[xLabel]]

    cumsum = 0
    for b in steps:
        q = df.query(xLabel + ' >= ' + str(b) + ' and ' + xLabel + ' < ' + str(b + width))
        if width == -1:
            q = df.query(xLabel + ' == ' + str(b))
        mean = q[yLabel].mean()
        if displayCumsum:
            cumsum = cumsum + mean
            mean = cumsum
        max = q[yLabel].max()
        std = q[yLabel].std()
        count = q[yLabel].count()
        if count > 1:
            if not showMax:
                valid_x.append(b)
                valid_y1.append(mean - 1.96 * (std / math.sqrt(count)))
                valid_y2.append(mean + 1.96 * (std / math.sqrt(count)))
                line_y.append(mean)
                line_x.append(b)   
            else:
                line_y.append(max)
                line_x.append(b) 

    if not showMax:
        ax.fill_between(valid_x, valid_y1, valid_y2, facecolor=color, alpha=0.4, interpolate=True)
    ax.plot(line_x, line_y, color=color, linewidth=3, linestyle=linestyle, label=label)#, label=yLabel)
    

def main(id, file_id, title, show):
    prefixspan = '../ps-' + id + '.json'
    iprefixspan = '../u_ips-' + id + '.json'
    
    # RuntimePerBatch
    f, (ax4) = plt.subplots(1, 1, sharey=False, figsize=(5, 3.5)) 
    
    d = loadRawData(iprefixspan, 'RuntimePerBatch', 'RuntimePerBatch')
    df = pd.DataFrame(data=d)  
    df = df.sort_values(by=['Batch'])
    plot_ts(df, ax4, flatui[0], 'RuntimePerBatch', 'Batch', 1, 'ProSecCo', displayCumsum=False, linestyle = '-')

    ax4.set_ylabel('Time (s)')
    ax4.set_xlabel('Block')
    ax4.yaxis.set_major_formatter(plt.FuncFormatter(second_formatter))
    legend = ax4.legend(loc='best')
    legend.get_frame().set_facecolor('#ffffff')
    legend.get_frame().set_linewidth(0.0)
    ax4.set_ylim(bottom=0)
  
    f.savefig('../fig/' + file_id + '-batch_runtime.pdf', bbox_inches='tight')
    plt.tight_layout()
   
    if show:
        plt.show()
    plt.close('all')
    

if __name__== '__main__':
    show = True
    main('seq-accidents-lg-0.80', 'accidents-5-0_80', 'ACCIDENTS-0.80', show)
    main('seq-accidents-lg-0.85', 'accidents-5-0_85', 'ACCIDENTS-0.85', show)
    main('seq-accidents-lg-0.90', 'accidents-5-0_90', 'ACCIDENTS-0.90', show)

    main('seq-bms-lg-0.03', 'bms-100-0_03', 'BMS-0.03', show)
    main('seq-bms-lg-0.04', 'bms-100-0_04', 'BMS-0.04', show)
    main('seq-bms-lg-0.05', 'bms-100-0_05', 'BMS-0.05', show)

    main('seq-kosarak-lg-0.05', 'kosarak-50-0_05', 'KORSARAK-0.05', show)
    main('seq-kosarak-lg-0.10', 'kosarak-50-0_10', 'KORSARAK-0.10', show)
    main('seq-kosarak-lg-0.15', 'kosarak-50-0_15', 'KORSARAK-0.15', show)

    main('seq-sign-lg-0.40', 'sign-200-0_40', 'SIGN-0.40', show)
    main('seq-sign-lg-0.50', 'sign-200-0_50', 'SIGN-0.50', show)
    main('seq-sign-lg-0.60', 'sign-200-0_60', 'SIGN-0.60', show)
    
    main('seq-bible-lg-0.40', 'bible-200-0_40', 'BIBLE-0.40', show)
    main('seq-bible-lg-0.50', 'bible-200-0_50', 'BIBLE-0.50', show)
    main('seq-bible-lg-0.60', 'bible-200-0_60', 'BIBLE-0.60', show)
    
    main('seq-fifa-lg-0.3', 'fifa-50-0_30', 'FIFA-0.30', show)
    main('seq-fifa-lg-0.35', 'fifa-50-0_35', 'FIFA-0.35', show)
    main('seq-fifa-lg-0.4', 'fifa-50-0_40', 'FIFA-0.40', show)