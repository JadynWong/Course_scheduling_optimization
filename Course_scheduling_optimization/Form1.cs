using System.Data;
using System.Windows.Forms;

namespace Course_scheduling_optimization
{
    public partial class Form1 : Form
    {
        private List<Schedule> Schedules;  //全局变量

        public Form1()
        {
            InitializeComponent();
        }

        //限制textBox1只能输入正数
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)13) && (e.KeyChar != (char)8))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)13) && (e.KeyChar != (char)8))
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)13) && (e.KeyChar != (char)8))
            {
                e.Handled = true;
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)13) && (e.KeyChar != (char)8))
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，也不是小数点，则取消该输入
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (textBox4.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(textBox4.Text, out oldf);
                    b2 = float.TryParse(textBox4.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Insert(0, "----请选择----");
            comboBox1.SelectedIndex = 0;
            //设置默认值
            textBox1.Text = "3";
            textBox3.Text = "50";
            textBox4.Text = "0.3";
            textBox5.Text = "10";
            textBox6.Text = "1000";

            toolStripStatusLabel1.Text = "当前系统时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            this.timer1.Interval = 1000;
            this.timer1.Start();

            //IrisSkin4
            //skinEngine1.SkinFile = Application.StartupPath + @"/Skins/mp10.ssk";

        }

        //输出课程表
        private void PrintSchedule(List<Schedule> res)
        {
            DataTable dt = new DataTable();
            List<List<String>> Arr = new List<List<string>>();
            for (int i = 1; i < 6; i++)
            {
                Arr.Add(new List<string>() { i.ToString(), "", "", "", "", "" });
            }
            //表头
            dt.Columns.Add("week/slot");
            dt.Columns.Add("星期一");
            dt.Columns.Add("星期二");
            dt.Columns.Add("星期三");
            dt.Columns.Add("星期四");
            dt.Columns.Add("星期五");

            foreach (var r in res)
            {
                int weekDay = r.WeekDay;
                int slot = r.Slot;
                Arr[slot - 1][weekDay] = r.ToString();
            }
            foreach (var arr in Arr)
            {
                dt.Rows.Add(arr[0], arr[1], arr[2], arr[3], arr[4], arr[5]);
            }
            dataGridView1.DataSource = dt;
            //实现自动换行
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        //private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        //{
        //    toolStripStatusLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
        //}


        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "当前系统时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //清空数据
            richTextBox1.Text = "";
            dataGridView1.DataSource = null;

            if (textBox1.Text == "")
            {
                MessageBox.Show("请输入教室数目！");
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("请选择文件路径！");
            }
            else
            {
                string filePath = textBox2.Text;
                int roomRange = Convert.ToInt32(textBox1.Text);
                int popsize = Convert.ToInt32(textBox3.Text);
                double mutprob = Convert.ToDouble(textBox4.Text);
                int elite = Convert.ToInt32(textBox5.Text);
                int maxiter = Convert.ToInt32(textBox6.Text);

                List<Schedule> schedules = new List<Schedule>();
                List<int> classIds = new List<int>();

                //清空数据
                Schedules = new List<Schedule>(); //全局变量初始化
                comboBox1.Items.Clear();    //清空comboBox1中的数据
                comboBox1.Items.Insert(0, "----请选择----");
                comboBox1.SelectedIndex = 0;

                //读入excel/csv文件 
                StreamReader reader = new StreamReader(filePath);
                string line = "";
                List<string[]> listStrArr = new List<string[]>();

                line = reader.ReadLine();//读取一行数据
                while (line != null)
                {
                    listStrArr.Add(line.Split(','));//将文件内容分割成数组

                    line = reader.ReadLine();
                }
                foreach (var s in listStrArr)
                {
                    //放入schedule中   
                    int courseId = Convert.ToInt32(s[0]);
                    int classId = Convert.ToInt32(s[1]);
                    int teacherId = Convert.ToInt32(s[2]);
                    schedules.Add(new Schedule(courseId, classId, teacherId));
                    classIds.Add(classId);
                }

                //优化
                GeneticOptimize ga = new GeneticOptimize(popsize, mutprob, elite, maxiter);
                Schedules = ga.evolution(schedules, roomRange, richTextBox1);

                //comboBox1绑定数据
                foreach (var c in classIds.Distinct().ToList())
                {
                    comboBox1.Items.Add(c);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();  //显示选择文件对话框
            //openFileDialog1.InitialDirectory = "c:\\";
            // openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = openFileDialog1.FileName;   //显示文件路径
                                                                 //去textBox2.Text的路径下读取当前excel/csv文件
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != 0)
            {
                int classId = Convert.ToInt32(comboBox1.Text);
                List<Schedule> vis_res = new List<Schedule>();
                foreach (var r in Schedules)
                {
                    if (r.ClassId == classId)
                    {
                        vis_res.Add(r);
                    }
                }
                PrintSchedule(vis_res);
            }
            else
            {
                dataGridView1.DataSource = null;
            }
        }
    }
}