using System;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Kingdom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            // создание окна, которое помогает выбрать файл
            OpenFileDialog ofd = new OpenFileDialog();
            //файлы только .txt
            ofd.Filter = "txt files (*.txt)|*.txt";
            //вызов окна
            ofd.ShowDialog();
            //если выбранна папка, то читаем в TextBox
            if (ofd.FileName != String.Empty)
            {
                FileStream file1 = new FileStream(ofd.FileName, FileMode.Open);
                StreamReader reader = new StreamReader(file1);
                txbMain.Text = reader.ReadToEnd();
            }
        }


        private void btnCalc_Click(object sender, RoutedEventArgs e)
        {
            int edge, vertex;
            int[,] arr;
            //проверка на возможные ошибки в задании графа пользователем
            if (Check(out vertex, out edge, out arr))
            {
                int[][] stairsArr = GetCyclesAndWays(vertex, arr);
                int notExist = Reduse(ref stairsArr, vertex);
                GetAmountNecessaryWays(stairsArr, vertex, notExist);
            }
        }

        //выводим количество необходимых путей
        private void GetAmountNecessaryWays(int[][] stairsArr, int vertex, int notExist)
        {
            int result = 0;
            result += (stairsArr.Length + notExist) == 1?0: (stairsArr.Length + notExist);
            txbMain.Text += $"\nНеобходимо построить дорог: {result}.";
        }

        //сокращаем лишнии строки, объединяем строки
        private int Reduse(ref int[][] stairsArr, int vertex)
        {
            int resultExist = 0;
            bool[] isExist = new bool[vertex];
            Delete(ref stairsArr);

            for (int i = 0; i < stairsArr.Length; i++)
            {
                for (int j = 0; j < stairsArr[i].Length; j++)
                {
                    isExist[stairsArr[i][j]] = true;
                }
            }
            for (int i = 0; i < vertex; i++)
            {
                if (!isExist[i]) resultExist++;
            }

            int[] Cycles = new int[vertex];
            for (int i = 0; i < stairsArr.Length; i++)
            {
                if (stairsArr[i][0] == stairsArr[i][stairsArr[i].Length -1]) Cycles[stairsArr[i][0]]++;
            }

            for (int i = 0; i < stairsArr.Length; i++)
            {
                for (int j = 1; j < stairsArr[i].Length-1; j++)
                {
                    if (Cycles[stairsArr[i][j]] > 0) Cycles[stairsArr[i][j]] = 0;
                }
            }

            for (int i = 0; i < stairsArr.Length; i++)
            {
                if (stairsArr[i][0] == stairsArr[i][stairsArr[i].Length-1] && Cycles[stairsArr[i][0]] == 0) stairsArr[i] = null;
            }

            for (int i = 0; i < stairsArr.Length; i++)
            {
                for (int j = 0; j < stairsArr.Length; j++)
                {
                    if (i != j)
                    {
                        if(stairsArr[i][0] == stairsArr[j][stairsArr[j].Length - 1])
                        {
                            List<int> res = new List<int>(stairsArr[i]);
                            res.AddRange(new List<int>(stairsArr[j]));
                            stairsArr[i] = res.ToArray();
                            stairsArr[j] = null;
                            Delete(ref stairsArr);
                        }
                    }
                }
                
            }
            
            return resultExist;
        }

        //удаляем пустые строки из массивы
        private void Delete(ref int[][] stairsArr)
        {
            int count = 0;
            for (int i = 0; i < stairsArr.Length; i++)
            {
                if (stairsArr[i] != null) count++;
            }
            int[][]result = new int[count][];

            for (int i = 0, a = 0; i < stairsArr.Length; i++)
            {
                if (stairsArr[i] != null) result[a++] = stairsArr[i];
            }

            stairsArr = result;
        }

        //получаем Эйлеровы циклы и пути
        private int[][] GetCyclesAndWays(int vertex, int[,] arr)
        {
            int a = 0;
            int[][] stairsArr = new int[vertex][];
            bool[] IsExist = new bool[vertex];
            while (IsZero(arr) && a++ < vertex)
            {
                //присваивании итогового графа после проверок
                txbMain.Text += "\n";
                for (int i = 0; i < vertex; i++)
                {
                    for (int k = 0; k < vertex; k++)
                    {
                        txbMain.Text += arr[i, k] + " ";
                    }
                    txbMain.Text += "\n";
                }
                int[] deg = new int[vertex];
                for (int i = 0; i < vertex; ++i)
                    for (int j = 0; j < vertex; ++j)
                        deg[i] += arr[i, j];
                
                int first = 0;
                while (deg[first] % 2 == 0 && first < vertex-1) ++first;
                if (first == vertex - 1 && deg[first] == 0)
                {
                    first = 0;
                    while (deg[first] == 0 && first < vertex - 1) ++first;
                }

                int v1 = -1, v2 = -1;
                for (int i = 0; i < vertex; ++i)
                    if (deg[i] % 2 != 0)
                        if (v1 == -1) v1 = i;
                        else if (v2 == -1) v2 = i;

                int[] stack = new int[vertex];
                int pointer = 0;
                stack[pointer++] = first;
                int[] res = new int[vertex * vertex];
                string str = "";
                int resPointer = 0;
                while (pointer > 0)
                {
                    int v = stack[--pointer];
                    int i;
                    for (i = 0; i < vertex; ++i)
                        if (arr[v, i] > 0)
                        {
                            str += (v + 1) + " ";
                            break;
                        }

                    if (i == vertex)
                    {
                        res[resPointer++] = v;
                        pointer--;
                    }
                    else
                    {
                        --arr[v, i];
                        --arr[i, v];
                        stack[pointer++] = i;
                    }
                }

                if (v1 != -1)
                    for (int i = 0; i + 1 < resPointer; ++i)
                        if (res[i] == v1 && res[i + 1] == v2 || res[i] == v2 && res[i + 1] == v1)
                        {
                            int[] res2 = new int[vertex * vertex];
                            int res2Pointer = 0;
                            for (int j = i + 1; j < resPointer; ++j)
                                res2[res2Pointer++] = res[j];
                            for (int j = 1; j <= i; ++j)
                                res2[res2Pointer++] = res[j];
                            res = res2;
                            resPointer = res2Pointer;
                            break;
                        }


                for (int i = 0; i < resPointer; ++i)
                    str += (res[i] + 1) + " ";
                txbMain.Text += "\n";
                string[] arrStr = str.Split(' ');
                if (arrStr.Length - 1 > 1)
                {
                    stairsArr[a - 1] = new int[arrStr.Length - 1];

                    for (int i = 0; i < arrStr.Length - 1; i++)
                    {
                        stairsArr[a - 1][i] = Convert.ToInt32(arrStr[i]) - 1;
                        IsExist[stairsArr[a - 1][i]] = true;
                        txbMain.Text += (stairsArr[a - 1][i] + 1) + " ";
                    }
                }
            }

            return stairsArr;
        }

        //проверяем заполнен ли массив только нулями
        private bool IsZero(int[,] arr)
        {
            bool flag = false;
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int k = 0; k < arr.GetLength(1); k++)
                {
                    if (arr[i, k] != 0) flag = true;
                }
            }
            return flag;
        }

        //проверка на правильность ввода
        private bool Check(out int vertex, out int edge, out int[,] arrRes)
        {
            vertex = edge = 0;
            arrRes = new int[0, 0];
            if (txbMain.Text == String.Empty)
            {
                MessageBox.Show("Ничего не введено!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            //разрезаем строку на элементы по переносу строки, пробелу
            string[] strArr = txbMain.Text.Split(new char[] { ' ', '\n', '\0' });
            int[] arr = new int[strArr.Length];

            try
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    //конвертируем из строки в число
                    if (strArr[i] != String.Empty)
                        arr[i] = Convert.ToInt32(strArr[i]);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                string str = txbMain.Text;
                txbMain.Text = String.Empty;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] >= '0' && str[i] <= '9' || str[i] == ' ' || str[i] == '\n')
                        txbMain.Text += str[i];
                }
            }
            vertex = arr[0];
            edge = arr[1];
            arrRes = new int[vertex, vertex];
            for (int i = 2; i < arr.Length - 1; i += 2)
            {
                arrRes[arr[i] - 1, arr[i + 1] - 1]++;
                arrRes[arr[i + 1] - 1, arr[i] - 1]++;
            }
            txbMain.Text += "\n";
            return true;
        }

        //Технология Drag`N`Drop для перетаскивания файла непосредственно в TextBox
        private void txbMain_DragEnter(object sender, DragEventArgs e)
        {
            bool isCorrect = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }
                    FileInfo info = new FileInfo(filename);
                    if (info.Extension != ".txt")
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }
            if (isCorrect == true)
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void txbMain_PreviewDrop(object sender, DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            foreach (string filename in filenames)
                txbMain.Text = File.ReadAllText(filename);
            e.Handled = true;
        }
    }
}
