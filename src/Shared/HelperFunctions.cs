//**************************************************************************
// Collection of useful functions related to frequent pattern mining
//**************************************************************************
using System.Collections.Generic;
using System;
using System.Diagnostics;
public class HelperFunctions 
{
    public static int PatternSorter(Pattern a, Pattern b) 
    {
        var ret = b.Support.CompareTo(a.Support);
        if (ret == 0) 
            ret = a.CompareTo(b);

        if (ret == 0)
            ret = b.GetHashCode().CompareTo(a.GetHashCode());

        return ret;    
    }

    public static List<Pattern> ExtractClosedPatterns(List<Pattern> patterns) 
    {        
        Dictionary<int, List<Pattern>> equivalentSupport = new Dictionary<int, List<Pattern>>();
        foreach(var pattern in patterns) 
        {
            if (equivalentSupport.ContainsKey(pattern.Support))
            {
                equivalentSupport[pattern.Support].Add(pattern);

                // sort from shortest to longest
                equivalentSupport[pattern.Support].Sort((a, b) => a.NumItems.CompareTo(b.NumItems));
            }
            else 
            {
                var newList = new List<Pattern>();
                newList.Add(pattern);
                equivalentSupport.Add(pattern.Support, newList);
            }
        } 

        List<Pattern> closedPatterns = new List<Pattern>();

        foreach (var patternList in equivalentSupport.Values) 
        {
            for (int i = 0; i < patternList.Count; i++)
            {
                var closedPattern = patternList[i];
                for (int k = i; k < patternList.Count; k++)
                {
                    Debug.Assert(patternList[i].NumItems <= patternList[k].NumItems);
                    if (patternList[i].NumItems < patternList[k].NumItems 
                            && patternList[i].IsSubsetOf(patternList[k]))
                    {
                        // always keep the longest pattern
                        closedPattern = patternList[k];
                    }
                }

                if (!closedPatterns.Contains(closedPattern))
                {
                   closedPatterns.Add(closedPattern);
                }
            }
        }

        return closedPatterns;
    }
}