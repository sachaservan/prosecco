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
from matplotlib.ticker import MaxNLocator

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

def second_formatter(value, tick_number):
    return value / 1000
        
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
    

def main(id, supports, samplesize, file_id, show):
    linestyles = ['-', ':', '--']
    f, (ax4) = plt.subplots(1, 1, sharey=False, figsize=(5, 3.5)) 

    for idx, support in enumerate(supports):        
        iprefixspan = '../n_ips-' + id + '-' + support + '-' + samplesize + '.json'
        d = loadRawData(iprefixspan, 'RuntimePerBatch', 'RuntimePerBatch')
        df = pd.DataFrame(data=d)  
        df = df.sort_values(by=['Batch'])
        l = len(df['Batch'].unique())
        w = -1
        if (l > 60):
            w = 5
        plot_ts(df, ax4, flatui[idx], 'RuntimePerBatch', 'Batch', w, support, displayCumsum=False, linestyle = linestyles[idx])

    ax4.set_ylabel('Time (s)')
    ax4.set_xlabel('Block')
    ax4.yaxis.set_major_formatter(plt.FuncFormatter(second_formatter))
    ax4.xaxis.set_major_locator(MaxNLocator(integer=True))
    legend = ax4.legend(loc='best')
    legend.get_frame().set_facecolor('#ffffff')
    legend.get_frame().set_linewidth(0.0)
    ax4.set_ylim(bottom=0)
  
    f.savefig('../fig/' + file_id + '-batch_runtime.pdf', bbox_inches='tight')
    plt.tight_layout()
    print(file_id)

    if show:
        plt.show()
    plt.close('all')
    

if __name__== '__main__':
    show = False

    main("seq-kosarak-lg", ["0.05", "0.10", "0.15"], "10k", 'kosarak-50-10k', show)
    main("seq-kosarak-lg", ["0.05", "0.10", "0.15"], "20k", 'kosarak-50-20k', show)
    main("seq-kosarak-lg", ["0.05", "0.10", "0.15"], "40k", 'kosarak-50-40k', show)
    main("seq-kosarak-lg", ["0.05", "0.10", "0.15"], "60k", 'kosarak-50-60k', show)
    main("seq-kosarak-lg", ["0.05", "0.10", "0.15"], "80k", 'kosarak-50-80k', show)
   
    main("seq-bms-lg", ["0.03", "0.04", "0.05"], "10k", 'bms-100-10k', show)
    main("seq-bms-lg", ["0.03", "0.04", "0.05"], "20k", 'bms-100-20k', show)
    main("seq-bms-lg", ["0.03", "0.04", "0.05"], "40k", 'bms-100-40k', show)
    main("seq-bms-lg", ["0.03", "0.04", "0.05"], "80k", 'bms-100-80k', show)

    main("seq-accidents-lg", ["0.80", "0.85", "0.90"], "10k", 'accidents-5-10k', show)
    main("seq-accidents-lg", ["0.80", "0.85", "0.90"], "20k", 'accidents-5-20k', show)
    main("seq-accidents-lg", ["0.80", "0.85", "0.90"], "40k", 'accidents-5-40k', show)
    main("seq-accidents-lg", ["0.80", "0.85", "0.90"], "60k", 'accidents-5-60k', show)
    main("seq-accidents-lg", ["0.80", "0.85", "0.90"], "80k", 'accidents-5-80k', show)
    
    main("seq-sign-lg", ["0.40", "0.50", "0.60"], "10k", 'sign-200-10k', show)
    main("seq-sign-lg", ["0.40", "0.50", "0.60"], "20k", 'sign-200-20k', show)
    main("seq-sign-lg", ["0.40", "0.50", "0.60"], "40k", 'sign-200-40k', show)
    main("seq-sign-lg", ["0.40", "0.50", "0.60"], "60k", 'sign-200-60k', show)
    main("seq-sign-lg", ["0.40", "0.50", "0.60"], "80k", 'sign-200-80k', show)

    main("seq-bible-lg", ["0.40", "0.50", "0.60"], "10k", 'bible-200-10k', show)
    main("seq-bible-lg", ["0.40", "0.50", "0.60"], "20k", 'bible-200-20k', show)
    main("seq-bible-lg", ["0.40", "0.50", "0.60"], "40k", 'bible-200-40k', show)
    main("seq-bible-lg", ["0.40", "0.50", "0.60"], "60k", 'bible-200-60k', show)
    main("seq-bible-lg", ["0.40", "0.50", "0.60"], "80k", 'bible-200-80k', show)

    main("seq-fifa-lg", ["0.3", "0.35", "0.4"], "10k", 'fifa-50-10k', show)   
    main("seq-fifa-lg", ["0.3", "0.35", "0.4"], "20k", 'fifa-50-20k', show)    
    main("seq-fifa-lg", ["0.3", "0.35", "0.4"], "40k", 'fifa-50-40k', show)    
    main("seq-fifa-lg", ["0.3", "0.35", "0.4"], "40k", 'fifa-50-60k', show)  
    main("seq-fifa-lg", ["0.3", "0.35", "0.4"], "80k", 'fifa-50-80k', show)
