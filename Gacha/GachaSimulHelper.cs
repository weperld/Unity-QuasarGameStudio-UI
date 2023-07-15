using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public static class GachaSimulHelper
{
    public delegate uint GetAdditiveCorrectionalProbDelegate(uint count, StackModel stackModel);

    public class StackModel
    {
        public string name;
        public float probCorrection;
        public uint stackStart;
        public uint stackEnd;
        public uint stack100;

        public bool _IsAvailable
        {
            get
            {
                return probCorrection >= 0f
                    && stackStart > 0
                    && stack100 >= stackStart + 2
                    && (stackEnd > stackStart && stackEnd < stack100);
            }
        }

        public StackModel(gacha_stack_modelTable data)
        {
            name = data._Enum_Id;
            probCorrection = data._A_Prob_Correction;
            stackStart = data._Stack_Correction_Start;
            stackEnd = data._Stack_Correction_End;
            stack100 = data._Stack_100;
        }
        public StackModel(string name, float probCorrection, uint stackStart, uint stackEnd, uint stack100)
        {
            this.name = name;
            this.probCorrection = probCorrection;
            this.stackStart = stackStart;
            this.stackEnd = stackEnd;
            this.stack100 = stack100;
        }
    }
    public class SimulIndividualResultData
    {
        public string enumId;
        public Data.Enum.Gacha_Pool_Grade grade;
        public uint appearCount;

        public characterTable _CharData => Data._characterTable.GetDataFromTable(enumId);

        public SimulIndividualResultData(string enumId, Data.Enum.Gacha_Pool_Grade grade)
        {
            this.enumId = enumId;
            this.grade = grade;
            appearCount = 0;
        }

        public void Appear()
        {
            appearCount++;
        }
    }
    public class SimulationFinalResultData
    {
        public double expectedCount;
        public uint tryCount;
        public Dictionary<string, SimulIndividualResultData> dict_AppearableCharacter = new Dictionary<string, SimulIndividualResultData>();

        public void SetAppearableCharacterData(gacha_classTable gachaClassData)
        {
            dict_AppearableCharacter.Clear();
            if (gachaClassData == null) return;

            for (int i = gachaClassData._Pool_Group_Data.Length - 1; i >= 0; i--)
            {
                var pg = gachaClassData._Pool_Group_Data[i];
                if (pg == null) continue;

                var pgr = new GachaHelper.PoolGroupRateData(pg, (Data.Enum.Gacha_Pool_Grade)i);
                var tmpList = pgr._List_AllAppearableCharacter;
                if (tmpList == null || tmpList.Count == 0) continue;

                foreach (var c in tmpList)
                {
                    var key = c?._Enum_Id;
                    if (string.IsNullOrEmpty(key) || dict_AppearableCharacter.ContainsKey(key)) continue;
                    dict_AppearableCharacter.Add(key, new SimulIndividualResultData(key, (Data.Enum.Gacha_Pool_Grade)i));
                }
            }
        }
        public void SetExpectationValue(gacha_classTable gachaClassData, StackModel stackModel, GetAdditiveCorrectionalProbDelegate getAdditiveCorrectionalProb)
        {
            expectedCount = 0f;
            tryCount = 0;
            if (gachaClassData == null || stackModel == null) return;

            tryCount = stackModel.stack100;
            float defDropProb = gachaClassData._Prob[3];
            float probCorrection = stackModel.probCorrection;
            double prevOccurProb = 1d;
            double sumOfOccurProb = prevOccurProb;
            double prevSuccessProb = defDropProb;

            uint stackCount = 2;
            while (stackCount < tryCount)
            {
                prevOccurProb = prevOccurProb * Math.Max(1d - prevSuccessProb, 0d);
                sumOfOccurProb += prevOccurProb;

                prevSuccessProb = defDropProb + probCorrection * getAdditiveCorrectionalProb(stackCount, stackModel);
                prevSuccessProb = Math.Min(prevSuccessProb, 1d);
                stackCount++;
            }

            prevOccurProb = prevOccurProb * Math.Max(1d - prevSuccessProb, 0d);
            expectedCount = sumOfOccurProb + prevOccurProb;
        }

        public void ClearAppearCountData()
        {
            foreach (var v in dict_AppearableCharacter.Values)
                v.appearCount = 0;
        }
    }

    public static uint GetAdditiveCorrectionalProbCount(uint count, StackModel stackModel)
    {
        if (stackModel == null) return 0;
        uint additiveCount = 0;

        if (count >= stackModel.stackStart)
        {
            count = Math.Min(count, stackModel.stackEnd);
            additiveCount = count - stackModel.stackStart + 1;
        }

        return additiveCount;
    }

    public static int GetAdditiveCorrectionalProbCount(int tryRound, gacha_stack_modelTable stackModelData)
    {
        if (stackModelData == null) return 0;
        return (int)GetAdditiveCorrectionalProbCount((uint)tryRound, new StackModel(stackModelData));
    }

    public static float GetAdditiveCorrectionProbTotalValue(int tryRound, gacha_stack_modelTable stackModelData)
    {
        if (stackModelData == null) return 0f;
        var additiveCount = GetAdditiveCorrectionalProbCount(tryRound, stackModelData);

        return additiveCount * stackModelData._A_Prob_Correction;
    }
}