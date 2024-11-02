using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace GameSpace.Core
{
    public class Utils
    {
        public static int ConvertRowColToIndex(int row, int col)
        {
            return (row - 1) * 4 + (col - 1);
        }
        static int[,] matrix = {
            {0, 1, 2, 3},
            {4, 5, 6, 7},
            {8, 9, 10, 11},
            {12, 13, 14, 15}
        };
        public static List<int> GetNeibors(int row,int col)
        {
            List<int> result = new List<int>();
            if (row > 0)
            {
                result.Add(matrix[row - 1, col]);
            }
            if (row < 3)
            {
                result.Add(matrix[row + 1, col]);
            }
            if (col > 0)
            {
                result.Add(matrix[row, col - 1]);
            }
            if (col < 3)
            {
                result.Add(matrix[row, col + 1]);
            }

            return result;
        }

        public static void ClearAllMatEffect(Material m_GameMaterial)
        {
            for(int i = 0; i < 5; i++)
            {
                m_GameMaterial.SetVector("_RectPosition" + i, new Vector4(0, 0));
                m_GameMaterial.SetVector("_RectSize" + i, new Vector4(0, 0));
                m_GameMaterial.SetFloat("_RectRadii" + i, 0);
            }
            for (int i = 0; i < 4; i++)
            {
                m_GameMaterial.SetVector("_RectPosition" + i+"_"+(1+i), new Vector4(0, 0));
                m_GameMaterial.SetVector("_RectSize" + i + "_" + (1 + i), new Vector4(0, 0));
                m_GameMaterial.SetFloat("_RectRadii" + i + "_" + (1 + i), 0);
            }
        }
    }
}
