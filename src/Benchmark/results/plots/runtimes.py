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
import matplotlib.patches as patches

def second_formatter(value, tick_number):
    return int(value / 1000.0)

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
    'ytick.minor.size': 0.0}, font_scale = 1.2)

flatui = ['#28aad5', '#C171A9', '#38ae97' ,'#ec7545']

def compute(dataset, fsize, name):
    runtimes = {}
    with open('runtimes.pickle', 'rb') as handle:
        runtimes = pickle.load(handle)
    
    prosecco_means = []
    prosecco_std = []
    prefixspan_means = []
    prefixspan_std = []

    prefixspan = runtimes['prefixspan']
    prosecco = runtimes['prosecco']
    allprosecco = []
    allprefixspan = []
    for ds in datasets:
        r_ps = prefixspan[ds]

        allprefixspan.append(r_ps)
        prefixspan_means.append(np.array(r_ps).mean())
        b = np.array(r_ps).std() / math.sqrt(len(r_ps))
        prefixspan_std.append(1.96 * b)

        r_pr = prosecco[ds]

        allprosecco.append(r_pr)
        prosecco_means.append(np.array(r_pr).mean())    
        b = np.array(r_pr).std() / math.sqrt(len(r_pr))
        prosecco_std.append(1.96 * b)

    print('prosecco = ' + '{:.2f}'.format(np.array(allprosecco).mean() / 1000.0))
    print('prefixsp = ' + '{:.2f}'.format(np.array(allprefixspan).mean() / 1000.0))
    print(len(prosecco_std), len(prefixspan_std))
    print(datasets)


    ind = np.arange(len(prosecco_means))  # the x locations for the groups
    width = 0.35  # the width of the bars

    f, (ax) = plt.subplots(1, 1, sharey=False, figsize=fsize) 
    #f.subplots_adjust(bottom=0.3)

    rects1 = ax.bar(ind - width/2, prosecco_means, width, yerr=prosecco_std,
                    color=flatui[0], label='ProSecCo', zorder=10)
    rects2 = ax.bar(ind + width/2, prefixspan_means, width, yerr=prefixspan_std,
                    color=flatui[1], label='PrefixSpan', hatch='//', zorder=10)

    # Add some text for labels, title and custom x-axis tick labels, etc.
    ax.set_ylabel('Total Runtime (s)')
    ax.set_xticks(ind)
    ax.set_ylim([0.0, 800000])

    lookup = {'BMS' : 'BMS-WebView1', 'KORSARAK' : 'Kosarak', 'FIFA' : 'FIFA', 'BIBLE' : 'Bible', 'SIGN' : 'Sign', 'ACCIDENTS' : 'Accidents'}

    dslabels = []
    for i, ds in enumerate(datasets):
        dss = ds.split('-')
        if (i - 1) % 3 == 0:
            dslabels.append(dss[1] + '\n' + lookup[dss[0]])
        else:
            dslabels.append(dss[1])

        w = rects1[0].get_width()
        if i % 3 == 0:
            x = i -w
            y = -130000
            plt.plot([x, x], [0, y], 'k-', lw=0.5, color='.8', clip_on=False)
        if i % 3 == 2:
            x = i + w
            y = -130000
            plt.plot([x, x], [0, y], 'k-', lw=0.5, color='.8', clip_on=False)
            
    ax.xaxis.grid(False)
    ax.set_xticklabels(dslabels)
    locs, labels = plt.xticks() 
    legend = ax.legend(loc='upper right')
    legend.get_frame().set_facecolor('#ffffff')
    legend.get_frame().set_linewidth(0.0)
    ax.yaxis.set_major_formatter(plt.FuncFormatter(second_formatter))

    autolabel(rects1, rects2, prosecco, prefixspan, datasets, ax, "center")

    f.savefig('../fig/' + name + '.pdf', bbox_inches='tight')
    plt.tight_layout()
    #plt.show()


def autolabel(rects1, rects2, prosecco, prefixspan, datasets, ax, xpos='center'):
    """
    Attach a text label above each bar in *rects*, displaying its height.

    *xpos* indicates which side to place the text w.r.t. the center of
    the bar. It can be one of the following {'center', 'right', 'left'}.
    """

    xpos = xpos.lower()  # normalize the case of the parameter
    ha = {'center': 'center', 'right': 'left', 'left': 'right'}
    offset = {'center': 0.5, 'right': 0.57, 'left': 0.43}  # x_txt = x + w*off

    newlist1 = sorted(rects1, key=lambda x: x.get_x() , reverse=False)
    newlist2 = sorted(rects2, key=lambda x: x.get_x() , reverse=False)

    i = 0
    for idx, rect in enumerate(newlist1):
        height = rect.get_height()
        height = max(height, newlist2[idx].get_height())
        text = np.mean(prosecco[datasets[i]]) / np.mean(prefixspan[datasets[i]])

        x = rect.get_x() + rect.get_width()*offset[xpos]
        y = 1.05*height + 3

        t = ax.text(x, y, '{:.2f}'.format(text) + 'x', ha=ha[xpos], va='bottom', fontsize=11,
        backgroundcolor=(1.0, 1.0, 1.0, 0.5))
        
        i += 1



    
datasets = ['ACCIDENTS-0.80', 'ACCIDENTS-0.85', 'ACCIDENTS-0.90',
            'BIBLE-0.40', 'BIBLE-0.50', 'BIBLE-0.60', #remove 
            'BMS-0.03', 'BMS-0.04', 'BMS-0.05', 
            'FIFA-0.30', 'FIFA-0.35', 'FIFA-0.40', 
            'KORSARAK-0.05', 'KORSARAK-0.10', 'KORSARAK-0.15']
compute(datasets, (12, 3.0), 'runtime')

datasets = ['ACCIDENTS-0.80', 'ACCIDENTS-0.85', 'ACCIDENTS-0.90']
compute(datasets, (5, 3.5), 'runtime_accidents')

datasets = ['BMS-0.03', 'BMS-0.04', 'BMS-0.05']
compute(datasets, (5, 3.5), 'runtime_bms')

datasets = ['FIFA-0.30', 'FIFA-0.35', 'FIFA-0.40']
compute(datasets, (5, 3.5), 'runtime_fifa')

datasets = ['KORSARAK-0.05', 'KORSARAK-0.10', 'KORSARAK-0.15']
compute(datasets, (5, 3.5), 'runtime_kosarak')