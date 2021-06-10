using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(IFA_Light))]
public class IFA_IceMark : MonoBehaviour//依法生成冰的能力
{
    public int record;//记录点个数
    public Vector3[] vertice;//是交点
    public Vector3 verticeall;//记录所有点总和的vector3
    private ArrayList[] mian;//储存面上的点的
    Vector4[] mianInformation;//储存面的abcd的
    public bool[] mianlook;//储存面是否遮挡
    public string a;

    private Vector3[] crossPoint;//相交点暂时储存
    private void Start()
    {
        crossPoint = new Vector3[100];
        IceMark();

    }
    public void IceMark()
    {
        /*流程:
         * 1:获取含面方程的string,了解有面的数量及面方程的abcd(ax+by+cz+d=0)
         * 2:每3个面方程用矩阵求交点,并将这个交点的编号记录在这3个面上(mian这个里面)
         * 3:然后从每个面开始,每3个点形成1个面,并根据所有点的位置平均处及这个面的位置计算这个面是否能遮挡这个点
         * 4:然后每3个点就按能遮挡和不能遮挡来决定是逆时针加入还是顺时针加入,完成!
         */
        //a = "x=0;y=0;z=0;x-1=0;y-1=0;z-1=0";// x=0;y=0;z=0;x-1=0;y-1=0;z-1=0
        string[] save = a.Split(';');//保存 例如 x=0;y=0;z=0;x-1=0;y-1=0;z-1=0  => x=0  y=0 ....
        mianInformation = new Vector4[save.Length];
        //save.Length为面的数量/方程数
        for (int i = 0; i < save.Length; i++)
        {
            bool havex = false; bool havey = false; bool havez = false;
            float a1 = 0; float a2 = 0; float a3 = 0; float a4 = 0;
            for (int ii = 0; ii < save[i].Length; ii++)//把第一个方程的点是否有xyz获得
            {
                //print(save[0].Substring(ii, 1));
                if (save[i].Substring(ii, 1) == "x")
                {
                    havex = true;
                }
                else if (save[i].Substring(ii, 1) == "y")
                {
                    havey = true;
                }
                else if (save[i].Substring(ii, 1) == "z")
                {
                    havez = true;
                }
            }
            string[] savesave = save[i].Split('x', 'y', 'z', '=');
            if (havex)//y
            {
                if (savesave[0] == "")
                {
                    a1 = 1;
                }
                else if (savesave[0] == "-")
                {
                    a1 = -1;
                }
                else
                {
                    a1 = float.Parse(savesave[0]);
                }

                if (havey)//yy
                {
                    if (savesave[1] == "+")
                    {
                        a2 = 1;
                    }
                    else if (savesave[1] == "-")
                    {
                        a2 = -1;
                    }
                    else
                    {
                        a2 = float.Parse(savesave[1]);
                    }

                    if (havez)//yyy
                    {
                        if (savesave[2] == "+")
                        {
                            a3 = 1;
                        }
                        else if (savesave[2] == "-")
                        {
                            a3 = -1;
                        }
                        else
                        {
                            a3 = float.Parse(savesave[2]);
                        }
                        if (savesave.Length == 5)
                        {
                            if (savesave[3] == "")
                            {
                                a4 = 0;
                            }
                            else
                            {
                                a4 = float.Parse(savesave[3]);
                            }
                        }
                    }
                    else//yyn
                    {
                        if (savesave.Length == 4)
                        {
                            if (savesave[2] == "")
                            {
                                a4 = 0;
                            }
                            else
                            {
                                a4 = float.Parse(savesave[2]);
                            }
                        }
                    }

                }
                else //yn
                {
                    if (havez)//yny
                    {
                        if (savesave[1] == "+")
                        {
                            a3 = 1;
                        }
                        else if (savesave[1] == "-")
                        {
                            a3 = -1;
                        }
                        else
                        {
                            a3 = float.Parse(savesave[1]);
                        }
                        if (savesave.Length == 4)
                        {
                            if (savesave[2] == "")
                            {
                                a4 = 0;
                            }
                            else
                            {
                                a4 = float.Parse(savesave[2]);
                            }
                        }
                    }
                    else//ynn x=0; "" "0"
                    {
                        if (savesave.Length == 3)
                        {
                            if (savesave[1] == "")
                            {
                                a4 = 0;
                            }
                            else
                            {
                                a4 = float.Parse(savesave[1]);
                            }
                        }
                    }
                }
            }
            else
            {
                if (havey)//ny
                {
                    if (savesave[0] == "")
                    {
                        a2 = 1;
                    }
                    else if (savesave[0] == "-")
                    {
                        a2 = -1;
                    }
                    else
                    {
                        a2 = float.Parse(savesave[0]);
                    }

                    if (havez)//nyy
                    {
                        if (savesave[1] == "+")
                        {
                            a3 = 1;
                        }
                        else if (savesave[1] == "-")
                        {
                            a3 = -1;
                        }
                        else
                        {
                            a3 = float.Parse(savesave[1]);
                        }
                        if (savesave.Length == 4)
                        {
                            if (savesave[2] == "")
                            {
                                a4 = 0;
                            }
                            else
                            {
                                a4 = float.Parse(savesave[2]);
                            }
                        }
                    }
                    else//nyn
                    {
                        if (savesave.Length == 3)
                        {
                            if (savesave[1] == "")
                            {
                                a4 = 0;
                            }
                            else
                            {
                                a4 = float.Parse(savesave[1]);
                            }
                        }
                    }
                }
                else //nn
                {
                    if (havez)//nny
                    {
                        if (savesave[0] == "")
                        {
                            a3 = 1;
                        }
                        else if (savesave[0] == "-")
                        {
                            a3 = -1;
                        }
                        else
                        {
                            a3 = float.Parse(savesave[1]);
                        }
                        if (savesave.Length == 3)
                        {
                            if (savesave[1] == "")
                            {
                                a4 = 0;
                            }
                            else
                            {
                                a4 = float.Parse(savesave[1]);
                            }
                        }
                    }
                    else
                    {
                        print("没有xyz");
                    }
                }
            }
            mianInformation[i] = new Vector4(a1,a2,a3,a4);
        }
        //1:获取含面方程的string,了解有面的数量及面方程的abcd(ax+by+cz+d=0) 完成
        record = 0;
        mian = new ArrayList[save.Length];
        mianlook = new bool[save.Length];
        for (int i = 0; i < save.Length; i++)
        {
            mian[i] = new ArrayList();//初始化
        }

        for (int i = 0; i < save.Length-2; i++)
        {
            for (int ii = i + 1; ii < save.Length - 1; ii++)
            {
                for (int iii = ii + 1; iii < save.Length; iii++)//012 013 014 023 024 034 123 124 134 234
                {
                    print("10?");
                    float a1 = mianInformation[i].x; float a2 = mianInformation[i].y; float a3 = mianInformation[i].z; float a4 = mianInformation[i].w;
                    float b1 = mianInformation[ii].x; float b2 = mianInformation[ii].y; float b3 = mianInformation[ii].z; float b4 = mianInformation[ii].w;
                    float c1 = mianInformation[iii].x; float c2 = mianInformation[iii].y; float c3 = mianInformation[iii].z; float c4 = mianInformation[iii].w;
                    if ((a1 * (b2 * c3 - b3 * c2) - b1 * (a2 * c3 - a3 * c2) + c1 * (a2 * b3 - a3 * b2)) == 0)
                    {
                        print("3个面没有形成唯一交点,这三个面是:"+save[i]+" "+ save[ii]+ " "+save[iii]);
                    }
                    else//此时有3面的矩阵,能记录面上有哪些交点
                    {
                        
                        crossPoint[record] = new Vector3(
                        (-a4 * (b2 * c3 - b3 * c2) + b4 * (a2 * c3 - a3 * c2) - c4 * (a2 * b3 - a3 * b2)) / (a1 * (b2 * c3 - b3 * c2) - b1 * (a2 * c3 - a3 * c2) + c1 * (a2 * b3 - a3 * b2)),
                        (a4 * (b1 * c3 - b3 * c1) - b4 * (a1 * c3 - a3 * c1) + c4 * (a1 * b3 - a3 * b1)) / (a1 * (b2 * c3 - b3 * c2) - b1 * (a2 * c3 - a3 * c2) + c1 * (a2 * b3 - a3 * b2)),
                        (-a4 * (b1 * c2 - b2 * c1) + b4 * (a1 * c2 - a2 * c1) - c4 * (a1 * b2 - a2 * b1)) / (a1 * (b2 * c3 - b3 * c2) - b1 * (a2 * c3 - a3 * c2) + c1 * (a2 * b3 - a3 * b2))
                        );
                        bool a = false,b=false,c=false,d=false;
                        for(int iiii=0;iiii<record; iiii++)
                        {
                            if (crossPoint[record]== crossPoint[iiii])
                            {
                                a = true;
                                break;
                            }
                        }
                        if (a)//有相同的
                        {
                            for (int iiii = 0; iiii < mian[i].Count; iiii++)
                            {
                                if (crossPoint[record] == crossPoint[int.Parse(mian[i][iiii].ToString())])
                                {
                                    b = true;//有重复的
                                    break;
                                }
                            }
                            for (int iiii = 0; iiii < mian[ii].Count; iiii++)
                            {
                                if (crossPoint[record] == crossPoint[int.Parse(mian[ii][iiii].ToString())])
                                {
                                    c = true;//有重复的
                                    break;
                                }
                            }
                            for (int iiii = 0; iiii < mian[iii].Count; iiii++)
                            {
                                if (crossPoint[record] == crossPoint[int.Parse(mian[iii][iiii].ToString())])
                                {
                                    
                                    d = true;//有重复的
                                    break;
                                }
                            }
                        }
                        
                        if (!b)
                        {
                                mian[i].Add(record);

                        }
                        if (!c)
                        {
                                mian[ii].Add(record);
                            
                        }
                        if (!d)
                        {
                            
                                mian[iii].Add(record);
                            
                                
                            /*else 
                            {
                                print("error!"); mian[iii].Add(4);
                                for (int iiii = 0; iiii < mian[iii].Count; iiii++)
                                {
                                    print(crossPoint[int.Parse(mian[iii][iiii].ToString())]);
                                }

                                print(crossPoint[record]!);
                            }*/
                                
                        }
                        if (!a)
                        {
                            verticeall += crossPoint[record];//记录所有点总和
                            record += 1;
                        }
                        
                    }
                }
            }
        }
        print("有"+record+"个交点");
        vertice = new Vector3[record];
        for(int i = 0; i < record; i++)
        {
            vertice[i] = crossPoint[i];
            print("第"+(i+1)+"个交点是"+ crossPoint[i]);
        }
        verticeall /= record;//获得中心点
        //2:每3个面方程用矩阵求交点,并将这个交点的编号记录在这3个面上(mian这个里面) 完成!


        int point = 0;
        for (int i = 0; i < mian.Length; i++)
        {
            point += mian[i].Count-2;
        }
        int[] triangle = new int[3 * point];//三角形总数

        float k = 0;
        for (int i = 0; i < save.Length; i++)
        {
            k = mianInformation[i].x * verticeall.x + mianInformation[i].y * verticeall.y + mianInformation[i].z * verticeall.z + mianInformation[i].w;
            if (k < 0)
            {
                mianlook[i] = true;//点在面下就会被面遮挡,1,1,1到0,0,0方向
            }

        }

        Vector3 d1; Vector3 d2; Vector3 d3;
        int recordpoint = 0; float kk = 0;
        
       


        for (int i = 0; i < mian.Length; i++)//遍历所有面
        {
            for (int ii = 0; ii < mian[i].Count - 2; ii++)//遍历点,会按123 234 345这样
            {
                kk = (vertice[int.Parse(mian[i][ii].ToString())].x + vertice[int.Parse(mian[i][ii].ToString())].y + vertice[int.Parse(mian[i][ii].ToString())].z) / 3;
                d1.x = vertice[int.Parse(mian[i][ii].ToString())].x - kk;
                d1.y = vertice[int.Parse(mian[i][ii].ToString())].y - kk;
                d1.z = vertice[int.Parse(mian[i][ii].ToString())].z - kk;

                kk = (vertice[int.Parse(mian[i][ii + 1].ToString())].x + vertice[int.Parse(mian[i][ii + 1].ToString())].y + vertice[int.Parse(mian[i][ii + 1].ToString())].z) / 3;
                d2.x = vertice[int.Parse(mian[i][ii + 1].ToString())].x - kk;
                d2.y = vertice[int.Parse(mian[i][ii + 1].ToString())].y - kk;
                d2.z = vertice[int.Parse(mian[i][ii + 1].ToString())].z - kk;

                kk = (vertice[int.Parse(mian[i][ii + 2].ToString())].x + vertice[int.Parse(mian[i][ii + 2].ToString())].y + vertice[int.Parse(mian[i][ii + 2].ToString())].z) / 3;
                d3.x = vertice[int.Parse(mian[i][ii + 2].ToString())].x - kk;
                d3.y = vertice[int.Parse(mian[i][ii + 2].ToString())].y - kk;
                d3.z = vertice[int.Parse(mian[i][ii + 2].ToString())].z - kk;

                if ((mianlook[i] && (d2 - d1).x * (d3 - d2).y + (d2 - d1).y * (d3 - d2).z + (d2 - d1).z * (d3 - d2).x - (d2 - d1).x * (d3 - d2).z - (d2 - d1).y * (d3 - d2).x - (d2 - d1).z * (d3 - d2).y > 0) ||
                    (!mianlook[i] && (d2 - d1).x * (d3 - d2).y + (d2 - d1).y * (d3 - d2).z + (d2 - d1).z * (d3 - d2).x - (d2 - d1).x * (d3 - d2).z - (d2 - d1).y * (d3 - d2).x - (d2 - d1).z * (d3 - d2).y < 0))
                {
                    triangle[3 * recordpoint] = int.Parse(mian[i][ii].ToString());
                    triangle[3 * recordpoint + 1] = int.Parse(mian[i][ii + 1].ToString());
                    triangle[3 * recordpoint + 2] = int.Parse(mian[i][ii + 2].ToString());
                }
                else
                {
                    triangle[3 * recordpoint] = int.Parse(mian[i][ii].ToString());
                    triangle[3 * recordpoint + 1] = int.Parse(mian[i][ii + 2].ToString());
                    triangle[3 * recordpoint + 2] = int.Parse(mian[i][ii + 1].ToString());
                }
                recordpoint += 1;
            }
        }
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertice;
        mesh.triangles = triangle;
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;//将网格碰撞器的网格切换到计算的网格


        //ObjExporter.MeshToFile(GetComponent<MeshFilter>(), "test.obj");
        print("YES!");
        //System.Diagnostics.Process.Start(Directory.GetCurrentDirectory()+ "/test.obj");会打开指定文件

        //Process.Start(Directory.GetCurrentDirectory());









        /*record = 0;
        
        mianInformation = new Vector4[save.Length];
        mian = new ArrayList[save.Length];
        mianlook = new bool[save.Length];
        for (int i = 0; i < mian.Length; i++)
        {
            mian[i] = new ArrayList();//初始化
            mianInformation[i] = new Vector4();
            mianlook[i] = false;
        }
        for (int i = 0; i < save.Length - 2; i++)
        {
            for (int ii = i + 1; ii < save.Length - 1; ii++)
            {
                for (int iii = ii + 1; iii < save.Length; iii++)//1~3个方程
                {
                    bool havex = false; bool havey = false; bool havez = false;
                    float a1 = 0; float a2 = 0; float a3 = 0; float a4 = 0;
                    float b1 = 0; float b2 = 0; float b3 = 0; float b4 = 0;
                    float c1 = 0; float c2 = 0; float c3 = 0; float c4 = 0;
                    for (int iiii = 0; iiii < save[i].Length; iiii++)//把第一个方程的点是否有xyz获得
                    {
                        //print(save[0].Substring(ii, 1));
                        if (save[i].Substring(iiii, 1) == "x")
                        {
                            havex = true;
                        }
                        else if (save[i].Substring(iiii, 1) == "y")
                        {
                            havey = true;
                        }
                        else if (save[i].Substring(iiii, 1) == "z")
                        {
                            havez = true;
                        }
                    }
                    string[] savesave = save[i].Split('x', 'y', 'z', '=');
                    if (havex)//y
                    {
                        if (savesave[0] == "")
                        {
                            a1 = 1;
                        }
                        else if (savesave[0] == "-")
                        {
                            a1 = -1;
                        }
                        else
                        {
                            a1 = float.Parse(savesave[0]);
                        }

                        if (havey)//yy
                        {
                            if (savesave[1] == "+")
                            {
                                a2 = 1;
                            }
                            else if (savesave[1] == "-")
                            {
                                a2 = -1;
                            }
                            else
                            {
                                a2 = float.Parse(savesave[1]);
                            }

                            if (havez)//yyy
                            {
                                if (savesave[2] == "+")
                                {
                                    a3 = 1;
                                }
                                else if (savesave[2] == "-")
                                {
                                    a3 = -1;
                                }
                                else
                                {
                                    a3 = float.Parse(savesave[2]);
                                }
                                if (savesave.Length == 5)
                                {
                                    if (savesave[3] == "")
                                    {
                                        a4 = 0;
                                    }
                                    else
                                    {
                                        a4 = float.Parse(savesave[3]);
                                    }
                                }
                            }
                            else//yyn
                            {
                                if (savesave.Length == 4)
                                {
                                    if (savesave[2] == "")
                                    {
                                        a4 = 0;
                                    }
                                    else
                                    {
                                        a4 = float.Parse(savesave[2]);
                                    }
                                }
                            }

                        }
                        else //yn
                        {
                            if (havez)//yny
                            {
                                if (savesave[1] == "+")
                                {
                                    a3 = 1;
                                }
                                else if (savesave[1] == "-")
                                {
                                    a3 = -1;
                                }
                                else
                                {
                                    a3 = float.Parse(savesave[1]);
                                }
                                if (savesave.Length == 4)
                                {
                                    if (savesave[2] == "")
                                    {
                                        a4 = 0;
                                    }
                                    else
                                    {
                                        a4 = float.Parse(savesave[2]);
                                    }
                                }
                            }
                            else//ynn x=0; "" "0"
                            {
                                if (savesave.Length == 3)
                                {
                                    if (savesave[1] == "")
                                    {
                                        a4 = 0;
                                    }
                                    else
                                    {
                                        a4 = float.Parse(savesave[1]);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (havey)//ny
                        {
                            if (savesave[0] == "")
                            {
                                a2 = 1;
                            }
                            else if (savesave[0] == "-")
                            {
                                a2 = -1;
                            }
                            else
                            {
                                a2 = float.Parse(savesave[0]);
                            }

                            if (havez)//nyy
                            {
                                if (savesave[1] == "+")
                                {
                                    a3 = 1;
                                }
                                else if (savesave[1] == "-")
                                {
                                    a3 = -1;
                                }
                                else
                                {
                                    a3 = float.Parse(savesave[1]);
                                }
                                if (savesave.Length == 4)
                                {
                                    if (savesave[2] == "")
                                    {
                                        a4 = 0;
                                    }
                                    else
                                    {
                                        a4 = float.Parse(savesave[2]);
                                    }
                                }
                            }
                            else//nyn
                            {
                                if (savesave.Length == 3)
                                {
                                    if (savesave[1] == "")
                                    {
                                        a4 = 0;
                                    }
                                    else
                                    {
                                        a4 = float.Parse(savesave[1]);
                                    }
                                }
                            }
                        }
                        else //nn
                        {
                            if (havez)//nny
                            {
                                if (savesave[0] == "")
                                {
                                    a3 = 1;
                                }
                                else if (savesave[0] == "-")
                                {
                                    a3 = -1;
                                }
                                else
                                {
                                    a3 = float.Parse(savesave[1]);
                                }
                                if (savesave.Length == 3)
                                {
                                    if (savesave[1] == "")
                                    {
                                        a4 = 0;
                                    }
                                    else
                                    {
                                        a4 = float.Parse(savesave[1]);
                                    }
                                }
                            }
                            else
                            {
                                print("你数学是体育老师教的嘛?");
                            }
                        }
                    }


                    havex = false; havey = false; havez = false; savesave = null;

                    for (int iiii = 0; iiii < save[ii].Length; iiii++)//把第一个方程的点是否有xyz获得
                    {
                        //print(save[0].Substring(ii, 1));
                        if (save[ii].Substring(iiii, 1) == "x")
                        {
                            havex = true;
                        }
                        else if (save[ii].Substring(iiii, 1) == "y")
                        {
                            havey = true;
                        }
                        else if (save[ii].Substring(iiii, 1) == "z")
                        {
                            havez = true;
                        }
                    }
                    savesave = save[ii].Split('x', 'y', 'z', '=');
                    if (havex)//y
                    {
                        if (savesave[0] == "+" || savesave[0] == "")
                        {
                            b1 = 1;
                        }
                        else if (savesave[0] == "-")
                        {
                            b1 = -1;
                        }
                        else
                        {
                            b1 = float.Parse(savesave[0]);
                        }

                        if (havey)//yy
                        {
                            if (savesave[1] == "+")
                            {
                                b2 = 1;
                            }
                            else if (savesave[1] == "-")
                            {
                                b2 = -1;
                            }
                            else
                            {
                                b2 = float.Parse(savesave[1]);
                            }

                            if (havez)//yyy
                            {
                                if (savesave[2] == "+")
                                {
                                    b3 = 1;
                                }
                                else if (savesave[2] == "-")
                                {
                                    b3 = -1;
                                }
                                else
                                {
                                    b3 = float.Parse(savesave[2]);
                                }
                                if (savesave.Length == 5)
                                {
                                    if (savesave[1] == "")
                                    {
                                        b4 = 0;
                                    }
                                    else
                                    {
                                        b4 = float.Parse(savesave[3]);
                                    }
                                }
                            }
                            else//yyn
                            {
                                if (savesave.Length == 4)
                                {
                                    if (savesave[2] == "")
                                    {
                                        b4 = 0;
                                    }
                                    else
                                    {
                                        b4 = float.Parse(savesave[2]);
                                    }
                                }
                            }

                        }
                        else //yn
                        {
                            if (havez)//yny
                            {
                                if (savesave[1] == "+")
                                {
                                    b3 = 1;
                                }
                                else if (savesave[1] == "-")
                                {
                                    b3 = -1;
                                }
                                else
                                {
                                    b3 = float.Parse(savesave[1]);
                                }
                                if (savesave.Length == 4)
                                {
                                    if (savesave[2] == "")
                                    {
                                        b4 = 0;
                                    }
                                    else
                                    {
                                        b4 = float.Parse(savesave[2]);
                                    }
                                }
                            }
                            else//ynn x=0; "" "0"
                            {
                                if (savesave.Length == 3)
                                {
                                    if (savesave[1] == "")
                                    {
                                        b4 = 0;
                                    }
                                    else
                                    {
                                        b4 = float.Parse(savesave[1]);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (havey)//ny
                        {
                            if (savesave[0] == "")
                            {
                                b2 = 1;
                            }
                            else if (savesave[0] == "-")
                            {
                                b2 = -1;
                            }
                            else
                            {
                                b2 = float.Parse(savesave[0]);
                            }

                            if (havez)//nyy
                            {
                                if (savesave[1] == "+")
                                {
                                    b3 = 1;
                                }
                                else if (savesave[1] == "-")
                                {
                                    b3 = -1;
                                }
                                else
                                {
                                    b3 = float.Parse(savesave[1]);
                                }
                                if (savesave.Length == 4)
                                {
                                    if (savesave[2] == "")
                                    {
                                        b4 = 0;
                                    }
                                    else
                                    {
                                        b4 = float.Parse(savesave[2]);
                                    }
                                }
                            }
                            else//nyn
                            {
                                if (savesave.Length == 3)
                                {
                                    if (savesave[1] == "")
                                    {
                                        b4 = 0;
                                    }
                                    else
                                    {
                                        b4 = float.Parse(savesave[1]);
                                    }
                                }
                            }
                        }
                        else //nn
                        {
                            if (havez)//nny
                            {
                                if (savesave[0] == "")
                                {
                                    b3 = 1;
                                }
                                else if (savesave[0] == "-")
                                {
                                    b3 = -1;
                                }
                                else
                                {
                                    b3 = float.Parse(savesave[1]);
                                }
                                if (savesave.Length == 3)
                                {
                                    if (savesave[1] == "")
                                    {
                                        b4 = 0;
                                    }
                                    else
                                    {
                                        b4 = float.Parse(savesave[1]);
                                    }
                                }
                            }
                            else
                            {
                                print("你数学是体育老师教的嘛?");
                            }
                        }
                    }

                    havex = false; havey = false; havez = false; savesave = null;

                    for (int iiii = 0; iiii < save[iii].Length; iiii++)//把第一个方程的点是否有xyz获得
                    {
                        //print(save[0].Substring(ii, 1));
                        if (save[iii].Substring(iiii, 1) == "x")
                        {
                            havex = true;
                        }
                        else if (save[iii].Substring(iiii, 1) == "y")
                        {
                            havey = true;
                        }
                        else if (save[iii].Substring(iiii, 1) == "z")
                        {
                            havez = true;
                        }
                    }
                    savesave = save[iii].Split('x', 'y', 'z', '=');
                    if (havex)//y
                    {
                        if (savesave[0] == "")
                        {
                            c1 = 1;
                        }
                        else
                        {
                            c1 = float.Parse(savesave[0]);
                        }

                        if (havey)//yy
                        {
                            if (savesave[1] == "+")
                            {
                                c2 = 1;
                            }
                            else if (savesave[1] == "-")
                            {
                                c2 = -1;
                            }
                            else
                            {
                                c2 = float.Parse(savesave[1]);
                            }

                            if (havez)//yyy
                            {
                                if (savesave[2] == "+")
                                {
                                    c3 = 1;
                                }
                                else if (savesave[2] == "-")
                                {
                                    c3 = -1;
                                }
                                else
                                {
                                    c3 = float.Parse(savesave[2]);
                                }
                                if (savesave.Length == 5)
                                {
                                    if (savesave[3] == "")
                                    {
                                        c4 = 0;
                                    }
                                    else
                                    {
                                        c4 = float.Parse(savesave[3]);
                                    }
                                }
                            }
                            else//yyn
                            {
                                if (savesave.Length == 4)
                                {
                                    if (savesave[2] == "")
                                    {
                                        c4 = 0;
                                    }
                                    else
                                    {
                                        c4 = float.Parse(savesave[2]);
                                    }
                                }
                            }

                        }
                        else //yn
                        {
                            if (havez)//yny
                            {
                                if (savesave[1] == "+")
                                {
                                    c3 = 1;
                                }
                                else if (savesave[1] == "-")
                                {
                                    c3 = -1;
                                }
                                else
                                {
                                    c3 = float.Parse(savesave[1]);
                                }
                                if (savesave.Length == 4)
                                {
                                    if (savesave[2] == "")
                                    {
                                        c4 = 0;
                                    }
                                    else
                                    {
                                        c4 = float.Parse(savesave[2]);
                                    }

                                }
                            }
                            else//ynn x=0; "" "0"
                            {
                                if (savesave.Length == 3)
                                {
                                    if (savesave[1] == "")
                                    {
                                        c4 = 0;
                                    }
                                    else
                                    {
                                        c4 = float.Parse(savesave[1]);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (havey)//ny
                        {
                            if (savesave[0] == "")
                            {
                                c2 = 1;
                            }
                            else if (savesave[0] == "-")
                            {
                                c2 = -1;
                            }
                            else
                            {
                                c2 = float.Parse(savesave[0]);
                            }

                            if (havez)//nyy
                            {
                                if (savesave[1] == "+")
                                {
                                    c3 = 1;
                                }
                                else if (savesave[1] == "-")
                                {
                                    c3 = -1;
                                }
                                else
                                {

                                    c3 = float.Parse(savesave[1]);
                                }
                                if (savesave.Length == 4)
                                {
                                    if (savesave[2] == "")
                                    {
                                        c4 = 0;
                                    }
                                    else
                                    {
                                        c4 = float.Parse(savesave[2]);
                                    }
                                }
                            }
                            else//nyn
                            {
                                if (savesave.Length == 3)

                                {

                                    if (savesave[1] == "")
                                    {
                                        c4 = 0;
                                    }
                                    else
                                    {
                                        c4 = float.Parse(savesave[1]);
                                    }

                                }
                            }
                        }
                        else //nn
                        {
                            if (havez)//nny
                            {
                                if (savesave[0] == "")
                                {
                                    c3 = 1;
                                }
                                else if (savesave[0] == "-")
                                {
                                    c3 = -1;
                                }
                                else
                                {
                                    c3 = float.Parse(savesave[1]);
                                }
                                if (savesave.Length == 3)
                                {
                                    if (savesave[1] == "")
                                    {
                                        c4 = 0;
                                    }
                                    else
                                    {
                                        c4 = float.Parse(savesave[1]);
                                    }
                                }
                            }
                            else
                            {
                                print("你数学是体育老师教的嘛?");
                            }
                        }
                    }
                    mianInformation[i] = new Vector4(a1, a2, a3, a4);
                    mianInformation[ii] = new Vector4(b1, b2, b3, b4);
                    mianInformation[iii] = new Vector4(c1, c2, c3, c4);
                    if ((a1 * (b2 * c3 - b3 * c2) - b1 * (a2 * c3 - a3 * c2) + c1 * (a2 * b3 - a3 * b2)) == 0)
                    {
                        print("有平行的面");
                    }
                    else//此时有3面的矩阵,能记录面上有哪些交点
                    {

                        vertice[record] = new Vector3(
                        (-a4 * (b2 * c3 - b3 * c2) + b4 * (a2 * c3 - a3 * c2) - c4 * (a2 * b3 - a3 * b2)) / (a1 * (b2 * c3 - b3 * c2) - b1 * (a2 * c3 - a3 * c2) + c1 * (a2 * b3 - a3 * b2)),
                        (a4 * (b1 * c3 - b3 * c1) - b4 * (a1 * c3 - a3 * c1) + c4 * (a1 * b3 - a3 * b1)) / (a1 * (b2 * c3 - b3 * c2) - b1 * (a2 * c3 - a3 * c2) + c1 * (a2 * b3 - a3 * b2)),
                        (-a4 * (b1 * c2 - b2 * c1) + b4 * (a1 * c2 - a2 * c1) - c4 * (a1 * b2 - a2 * b1)) / (a1 * (b2 * c3 - b3 * c2) - b1 * (a2 * c3 - a3 * c2) + c1 * (a2 * b3 - a3 * b2))
                        );//通过3面矩阵求交点
                          //此时的,面n,nn,nnn, point此点,new vector2[
                          //mian[0](方程1)=new array[1,1,1,1,....]

                        mian[i].Add(record);
                        mian[ii].Add(record);
                        mian[iii].Add(record);
                        print(vertice[record]);
                        verticeall += vertice[record];//记录所有点总和
                        record += 1;
                    }
                }
            }
        }
        verticeall /= record;//获得中心点 x4 y4 z4,那通过第4点向3-2和2-1进行某一操作,获得
        /*for (int i = 0; i < mian.Length; i++)//输出所有面上有哪些点
        {
            pointget = "";
            for (int ii = 0; ii < mian[i].Count; ii++)
            {
                pointget += vertice[int.Parse(mian[i][ii].ToString())];
            }
            print("方程" + save[i] + "上的点有" + pointget);
        }*/
        //判断2-1和3-2
        //x1 y1 z1 x2 y2 z2 x3 y3 z3 x4 y4 z4
        //遍历面与中心点比较




        /*int mianall = 0;
        for (int i = 0; i < mian.Length; i++)
        {
            mianall += mian[i].Count;//record是所有点,应该没有重合的,mianall是所有的面上所有点
        }
        int[] triangle = new int[3 * mianall - save.Length * 6];//三角形总数

        print(mianall + " " + save.Length);
        float k = 0;
        for (int i = 0; i < save.Length; i++)
        {
            k = mianInformation[i].x * verticeall.x + mianInformation[i].y * verticeall.y + mianInformation[i].z * verticeall.z + mianInformation[i].w;
            if (k < 0)
            {
                mianlook[i] = true;//点在面下就会被面遮挡,1,1,1到0,0,0方向

            }
            print(save[i] + " " + mianlook[i]);
        }
        Vector3 d1; Vector3 d2; Vector3 d3;
        int recordpoint = 0; float kk = 0;
        for (int i = 0; i < mian.Length; i++)//遍历所有面
        {
            for (int ii = 0; ii < mian[i].Count - 2; ii++)//遍历点,比如1~5,123 134 145,但排序怎么弄呢?
            {
                kk = (vertice[int.Parse(mian[i][ii].ToString())].x + vertice[int.Parse(mian[i][ii].ToString())].y + vertice[int.Parse(mian[i][ii].ToString())].z) / 3;
                d1.x = vertice[int.Parse(mian[i][ii].ToString())].x - kk;
                d1.y = vertice[int.Parse(mian[i][ii].ToString())].y - kk;
                d1.z = vertice[int.Parse(mian[i][ii].ToString())].z - kk;

                kk = (vertice[int.Parse(mian[i][ii + 1].ToString())].x + vertice[int.Parse(mian[i][ii + 1].ToString())].y + vertice[int.Parse(mian[i][ii + 1].ToString())].z) / 3;
                d2.x = vertice[int.Parse(mian[i][ii + 1].ToString())].x - kk;
                d2.y = vertice[int.Parse(mian[i][ii + 1].ToString())].y - kk;
                d2.z = vertice[int.Parse(mian[i][ii + 1].ToString())].z - kk;

                kk = (vertice[int.Parse(mian[i][ii + 2].ToString())].x + vertice[int.Parse(mian[i][ii + 2].ToString())].y + vertice[int.Parse(mian[i][ii + 2].ToString())].z) / 3;
                d3.x = vertice[int.Parse(mian[i][ii + 2].ToString())].x - kk;
                d3.y = vertice[int.Parse(mian[i][ii + 2].ToString())].y - kk;
                d3.z = vertice[int.Parse(mian[i][ii + 2].ToString())].z - kk;

                if ((mianlook[i] && (d2 - d1).x * (d3 - d2).y + (d2 - d1).y * (d3 - d2).z + (d2 - d1).z * (d3 - d2).x - (d2 - d1).x * (d3 - d2).z - (d2 - d1).y * (d3 - d2).x - (d2 - d1).z * (d3 - d2).y > 0) ||
                    (!mianlook[i] && (d2 - d1).x * (d3 - d2).y + (d2 - d1).y * (d3 - d2).z + (d2 - d1).z * (d3 - d2).x - (d2 - d1).x * (d3 - d2).z - (d2 - d1).y * (d3 - d2).x - (d2 - d1).z * (d3 - d2).y < 0))
                {
                    triangle[3 * recordpoint] = int.Parse(mian[i][ii].ToString());
                    triangle[3 * recordpoint + 1] = int.Parse(mian[i][ii + 1].ToString());
                    triangle[3 * recordpoint + 2] = int.Parse(mian[i][ii + 2].ToString());
                }
                else
                {
                    triangle[3 * recordpoint] = int.Parse(mian[i][ii].ToString());
                    triangle[3 * recordpoint + 1] = int.Parse(mian[i][ii + 2].ToString());
                    triangle[3 * recordpoint + 2] = int.Parse(mian[i][ii + 1].ToString());
                }
                recordpoint += 1;
            }
        }
        string printtri = "";
        for (int i = 0; i < triangle.Length; i++)
        {
            printtri += triangle[i] + " ";
        }
        print(printtri);

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertice;
        mesh.triangles = triangle;
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;//将网格碰撞器的网格切换到计算的网格*/
    }
}
