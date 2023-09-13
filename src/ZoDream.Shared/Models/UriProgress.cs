using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class UriProgress
    {
        public int GroupIndex { get; set; }

        public int GroupCount { get; set; }

        public int RuleIndex { get; set; }

        public int RuleCount { get; set; }

        public int StepIndex { get; set; }

        public int StepCount { get; set; }

        public double Value 
        {
            get {
                if (GroupCount <= 0)
                {
                    return 0;
                }
                var perGroup = 100 / GroupCount;
                var val = perGroup * GroupIndex;
                if (RuleCount <= 0)
                {
                    return val;
                }
                var perRule = perGroup / RuleCount;
                val += RuleIndex * perRule;
                if (StepCount <= 0)
                {
                    return val;
                }
                return val + StepIndex * (perRule / StepCount);
            }
        }

        public void UpdateGroup(int index, int count)
        {
            GroupCount = count;
            GroupIndex = index;
            RuleIndex = 0;
            RuleCount = 0;
            StepIndex = 0;
            StepCount = 0;
        }

        public void UpdateRule(int index, int count)
        {
            RuleIndex = index;
            RuleCount = count;
            StepIndex = 0;
            StepCount = 0;
        }

        public void UpdateStep(int index, int count)
        {
            StepIndex = index;
            StepCount = count;
        }
    }
}
