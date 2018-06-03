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
        

def loadPrecisionRecallData(fn):
    d = {'Batch': [], 'Run': [], 'Precision' : [], 'Recall' : [] }
    data = json.load(open(fn))
    run_cnt = 0
    for run in data['Runs']:
        tp = run['TruePositivesPerBatch']
        fp = run['FalsePositivesPerBatch']
        fn = run['FalseNegativesPerBatch']
        
        for i in range(len(tp)):
            d['Precision'].append(float(tp[i]) / (float(tp[i]) + float(fp[i])))
            d['Recall'].append(float(tp[i]) / (float(tp[i]) + float(fn[i])))
            d['Batch'].append(i)
            d['Run'].append(run_cnt)
        run_cnt += 1    
    return d

def loadMemoryData(fn):
    d = {'Time': [], 'Memory': [], 'Run': []}
    data = json.load(open(fn))
    run_cnt = 0
    step = 2000
    for run in data['Runs']:
        bt = run['MemoryUsagePerTimeInMillis']
        interval = 0
        interval_values = []
        times = [float(t) for t in bt]
        min_t = 0
        max_t = np.max(times)

        d['Run'].append(run_cnt)
        d['Time'].append(0)
        d['Memory'].append(0)

        for bin in np.arange(0, max_t, step):
            values = []
            for t in bt:
                tf = float(t)
                gb = bt[t] / 1073741824.0
                
                if tf < bin and tf >= bin - step:
                    values.append(gb)
                    
            if len(values) > 0:
                d['Run'].append(run_cnt)
                d['Time'].append(int((bin)))
                d['Memory'].append(np.mean(values))
        run_cnt += 1    
    return d

def loadTruePositives(fn):
    data = json.load(open(fn))
    return data['Runs'][0]['TruePositivesPerBatch']   
    

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
    
    
def loadErrorData(fn):
    d = {'Batch': [], 'Run': [], 'MeanError' : []}
    data = json.load(open(fn))
    run_cnt = 0
    for run in data['Runs']:
        data = run['Errors']
        
        for i in range(len(data)):
            d['MeanError'].append(data[i] * 100.0)
            d['Batch'].append(i)
            d['Run'].append(run_cnt)
        run_cnt += 1 
    return d    
    
def loadAbsoluteErrorData(fn):
    d = {'Batch': [], 'Run': [], 'MeanAbsoluteError' : [], 'MaxAbsoluteError' : [], 'TheoreticalAbsoluteError' : []}
    data = json.load(open(fn))
    run_cnt = 0
    for run in data['Runs']:
        data = run['AbsoluteErrorsPerBatch']
        for i in range(len(data)):
            dat = [x for x in data[i]]
            d['MeanAbsoluteError'].append(np.mean(dat))
            d['MaxAbsoluteError'].append(np.max(dat))
            d['TheoreticalAbsoluteError'].append(run['Errors'][i] / 2.0)
            d['Batch'].append(i)
            d['Run'].append(run_cnt)
        run_cnt += 1 
    return d

    
def loadNormalizedErrorData(fn):
    d = {'Batch': [], 'Run': [], 'MeanNormalizedError' : [], 'MaxNormalizedError' : []}
    data = json.load(open(fn))
    run_cnt = 0
    for run in data['Runs']:
        data = run['NormalizedErrorsPerBatch']
        
        for i in range(len(data)):
            dat = [x * 100.0 for x in data[i]]
            d['MeanNormalizedError'].append(np.mean(dat))
            d['MaxNormalizedError'].append(np.max(dat))
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
    
def getRuntime(fn):
    data = json.load(open(fn))
    runtimes = []
    for run in data['Runs']:
        runtimes.append(run['TotalRuntimeInMillis'])
        
    return runtimes
    

def main(runtimes, id, file_id, title, show):
    prefixspan = '../n_ps-' + id + '-20k.json'
    iprefixspan = '../n_ips-' + id + '-40k.json'
    
    # eval runtime
    runtimes_ps = getRuntime(prefixspan)
    runtimes_ips = getRuntime(iprefixspan)
    
    print(title, np.mean(runtimes_ps), np.std(runtimes_ps), np.mean(runtimes_ips), np.std(runtimes_ips))

    runtimes['prefixspan'][title] = runtimes_ps
    runtimes['prosecco'][title] = runtimes_ips
    
    
    tp = loadTruePositives(prefixspan)
    file_id = file_id + '-' + str(tp[0])

    # MEMORY
    f, (ax1) = plt.subplots(1, 1, sharey=False, figsize=(5, 3.5)) 
    
    d = loadMemoryData(iprefixspan)
    df = pd.DataFrame(data=d)   
    df = df.sort_values(by=['Time'])
    plot_ts(df, ax1, flatui[0], 'Memory', 'Time', -1, 'ProSecCo', linestyle = '-')

    d = loadMemoryData(prefixspan)
    df = pd.DataFrame(data=d)       
    df = df.sort_values(by=['Time'])
    plot_ts(df, ax1, flatui[1], 'Memory', 'Time', -1, 'PrefixSpan', linestyle = ':')

    ax1.set_ylabel('Memory (GB)')
    ax1.set_xlabel('Time (mm:ss)')
    ax1.xaxis.set_major_formatter(plt.FuncFormatter(minutes_second_formatter))
    #if title == 'ACCIDENTS-0.80'
    #    lgegend = ax1.legend(loc='upper right')
    legend = ax1.legend(loc='best')
    
    legend.get_frame().set_facecolor('#ffffff')
    legend.get_frame().set_linewidth(0.0)
    
    f.savefig('../fig/' + file_id + '-memory.pdf', bbox_inches='tight')
    #f.savefig('../fig/' + file_id + '-memory.svg', bbox_inches='tight')
    plt.tight_layout()
   
    if show:
        plt.show()
    plt.close('all')
    

    # ERROR
    f, (ax2) = plt.subplots(1, 1, sharey=False, figsize=(5, 3.5)) 
    
    d = loadNormalizedErrorData(iprefixspan)
    df = pd.DataFrame(data=d)   
    df = df.sort_values(by=['Batch'])
    plot_ts(df, ax2, flatui[0], 'MeanNormalizedError', 'Batch', 1, 'Mean', '-')
    plot_ts(df, ax2, flatui[0], 'MaxNormalizedError', 'Batch', 1, 'Max', ':', True)
   
    ax2.set_ylabel('Relative Percentage Error')
    ax2.set_xlabel('Block')
    legend = ax2.legend(loc='upper right')   
    legend.get_frame().set_facecolor('#ffffff')
    legend.get_frame().set_linewidth(0.0)
    
    f.savefig('../fig/' + file_id + '-error.pdf', bbox_inches='tight')
    plt.tight_layout()
   
    if show:
        plt.show()
    plt.close('all')  
    
       
    # Precision Recall
    f, (ax3) = plt.subplots(1, 1, sharey=False, figsize=(5, 3.5)) 
    
    d = loadPrecisionRecallData(iprefixspan)
    df = pd.DataFrame(data=d)   
    df = df.sort_values(by=['Batch'])
    plot_ts(df, ax3, flatui[0], 'Precision', 'Batch', 1, 'Precision')
    plot_ts(df, ax3, flatui[0], 'Recall', 'Batch', 1, 'Recall', ':')

    ax3.set_ylabel('Precision and Recall')
    ax3.set_xlabel('Block')
    legend = ax3.legend(loc='lower right')
    legend.get_frame().set_facecolor('#ffffff')
    legend.get_frame().set_linewidth(0.0)
    ax3.set_ylim([0.0, 1.1])
        
    f.savefig('../fig/' + file_id + '-precision_recall.pdf', bbox_inches='tight')
    plt.tight_layout()
   
    if show:
        plt.show()
    plt.close('all')    

    # RuntimePerBatch
    if False:
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
    
    # ABS-ERROR
    f, (ax5) = plt.subplots(1, 1, sharey=False, figsize=(5, 3.5)) 
    
    d = loadAbsoluteErrorData(iprefixspan)
    df = pd.DataFrame(data=d)   
    df = df.sort_values(by=['Batch'])
    plot_ts(df, ax5, flatui[0], 'MeanAbsoluteError', 'Batch', 1, 'Mean', '-')
    plot_ts(df, ax5, flatui[0], 'MaxAbsoluteError', 'Batch', 1, 'Max', ':', True)
    plot_ts(df, ax5, flatui[0], 'TheoreticalAbsoluteError', 'Batch', 1, 'Theoretical', '--', True)
   
    ax5.set_ylabel('Absolute Error')
    ax5.set_xlabel('Block')
    legend = ax5.legend(loc='upper right')
    legend.get_frame().set_facecolor('#ffffff')
    legend.get_frame().set_linewidth(0.0)
    
    f.savefig('../fig/' + file_id + '-absolute_error.pdf', bbox_inches='tight')
    plt.tight_layout()
   
    if show:
        plt.show()
    plt.close('all')  

    return (runtimes_ps, runtimes_ips)
    

if __name__== '__main__':
    show = True
    
    runtimes = {'prefixspan': {}, 'prosecco': {}}

    print('Dataset', 'PS-Mean', 'PS-STD', 'IPS-Mean', 'IPS-STD')


    (s, p) = main(runtimes, 'seq-accidents-lg-0.80', 'accidents-5-0_80', 'ACCIDENTS-0.80', show)
    (s, p) = main(runtimes, 'seq-accidents-lg-0.85', 'accidents-5-0_85', 'ACCIDENTS-0.85', show)
    (s, p) = main(runtimes, 'seq-accidents-lg-0.90', 'accidents-5-0_90', 'ACCIDENTS-0.90', show)

    (s, p) = main(runtimes, 'seq-bms-lg-0.03', 'bms-100-0_03', 'BMS-0.03', show)
    (s, p) = main(runtimes, 'seq-bms-lg-0.04', 'bms-100-0_04', 'BMS-0.04', show)
    (s, p) = main(runtimes, 'seq-bms-lg-0.05', 'bms-100-0_05', 'BMS-0.05', show)

    (s, p) = main(runtimes, 'seq-kosarak-lg-0.05', 'kosarak-50-0_05', 'KORSARAK-0.05', show)
    (s, p) = main(runtimes, 'seq-kosarak-lg-0.10', 'kosarak-50-0_10', 'KORSARAK-0.10', show)
    (s, p) = main(runtimes, 'seq-kosarak-lg-0.15', 'kosarak-50-0_15', 'KORSARAK-0.15', show)

    (s, p) = main(runtimes, 'seq-sign-lg-0.40', 'sign-200-0_40', 'SIGN-0.40', show)
    (s, p) = main(runtimes, 'seq-sign-lg-0.50', 'sign-200-0_50', 'SIGN-0.50', show)
    (s, p) = main(runtimes, 'seq-sign-lg-0.60', 'sign-200-0_60', 'SIGN-0.60', show)
    
    (s, p) = main(runtimes, 'seq-bible-lg-0.40', 'bible-200-0_40', 'BIBLE-0.40', show)
    (s, p) = main(runtimes, 'seq-bible-lg-0.50', 'bible-200-0_50', 'BIBLE-0.50', show)
    (s, p) = main(runtimes, 'seq-bible-lg-0.60', 'bible-200-0_60', 'BIBLE-0.60', show)
    
    (s, p) = main(runtimes, 'seq-fifa-lg-0.3', 'fifa-50-0_30', 'FIFA-0.30', show)
    (s, p) = main(runtimes, 'seq-fifa-lg-0.35', 'fifa-50-0_35', 'FIFA-0.35', show)
    (s, p) = main(runtimes, 'seq-fifa-lg-0.4', 'fifa-50-0_40', 'FIFA-0.40', show)

    
    with open('runtimes.pickle', 'wb') as handle:
        pickle.dump(runtimes, handle, protocol=pickle.HIGHEST_PROTOCOL)
    