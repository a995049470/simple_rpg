using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullFramework.Runtime
{

    public class AStart
    {

        public static List<Vector2Int> FindWay(Vector2Int start, Vector2Int end, int[,] map)
        {
            List<Vector2Int> pointlist = new List<Vector2Int>();
            List<ANode> closelist = new List<ANode>();
            List<ANode> openlist = new List<ANode>();
            openlist.Add(new ANode(end, start));
            bool isend = false;
            int len1 = map.GetLength(0);
            int len2 = map.GetLength(1);
            while (openlist.Count > 0)
            {
                var node = GetTopValue(openlist);
                closelist.Add(node);
                var id = node.id;
                if (id == end)
                {
                    break;
                }
                for (int i = id.x - 1; i < id.x + 2; i++)
                {
                    for (int j = id.y - 1; j < id.y + 2; j++)
                    {
                        if (i < 0 || i >= len1 || j < 0 || j >= len2)
                        {
                            continue;
                        }
                        if (i == id.x && j == id.y)
                        {
                            continue;
                        }
                        var n = map[i, j];
                        if (n != 0)
                        {
                            continue;
                        }
                        var v = new Vector2Int(i, j);
                        if (Vector2Int.Distance(v, id) > 1)
                        {
                            continue;
                        }
                        if (closelist.FindIndex(x => x.id == v) != -1)
                        {
                            continue;
                        }

                        AddNode(node, end, v, openlist);
                    }
                }
            }
            var temp = closelist[closelist.Count - 1];
            if (temp.id == end)
            {
                while (temp != null)
                {
                    pointlist.Insert(0, temp.id);
                    if (temp.id == start)
                    {
                        break;
                    }
                    temp = temp.parent;
                }
            }
            return pointlist;
        }

        //改变或增加元素
        public static void AddNode(ANode parnt, Vector2Int end, Vector2Int pos, List<ANode> list)
        {
            int index = list.FindIndex(x => x.id == pos);
            ANode node = null;
            bool ischange = false;
            if (index == -1)
            {
                ischange = true;
                node = new ANode(end, pos, parnt);
                list.Add(node);
                index = list.Count - 1;
            }
            else
            {
                node = list[index];
                ischange = node.ChangeParent(parnt);
            }
            if (!ischange)
            {
                return;
            }
            while (index > 0)
            {
                var root = (index - 1) / 2;
                float v0 = list[root].f;
                float v1 = list[index].f;
                if (v0 < v1)
                {
                    break;
                }
                var temp = list[root];
                list[root] = list[index];
                list[index] = temp;
                index = root;
            }

        }

        //从大小堆顶取值
        public static ANode GetTopValue(List<ANode> list)
        {
            ANode node = null;
            node = list[0];
            list[0] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            int root = 0;
            int left = 0;
            int right = 0;
            while (root < list.Count)
            {
                left = 2 * root + 1;
                right = 2 * root + 2;
                float v0 = list[root].f;
                float v1 = left >= list.Count ? float.MaxValue : list[left].f;
                float v2 = right >= list.Count ? float.MaxValue : list[right].f;
                if (v1 <= v0 && v1 <= v2)
                {
                    var temp = list[root];
                    list[root] = list[left];
                    list[left] = temp;
                    root = left;
                }
                else if (v2 <= v0 && v2 <= v1)
                {
                    var temp = list[root];
                    list[root] = list[right];
                    list[right] = temp;
                    root = right;
                }
                else
                {
                    break;
                }
            }

            return node;
        }

        private static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        public static void StartSW()
        {
            sw.Restart();
        }
        public static void EndSW(string tag)
        {
            sw.Stop();
            var t = sw.ElapsedMilliseconds;
            if (t < 1)
            {
                return;
            }
            Debug.Log($"<color=#ff0000ff>{tag}  time: {t}ms</color>");
        }
    }

}

