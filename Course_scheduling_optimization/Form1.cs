using System.Data;
using System.Windows.Forms;

namespace Course_scheduling_optimization
{
    public partial class Form1 : Form
    {
        private List<Schedule> Schedules;  //ȫ�ֱ���

        public Form1()
        {
            InitializeComponent();
        }

        //����textBox1ֻ����������
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //�������Ĳ������ּ���Ҳ���ǻس�����Backspace������ȡ��������
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)13) && (e.KeyChar != (char)8))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //�������Ĳ������ּ���Ҳ���ǻس�����Backspace������ȡ��������
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)13) && (e.KeyChar != (char)8))
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            //�������Ĳ������ּ���Ҳ���ǻس�����Backspace������ȡ��������
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)13) && (e.KeyChar != (char)8))
            {
                e.Handled = true;
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            //�������Ĳ������ּ���Ҳ���ǻس�����Backspace������ȡ��������
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)13) && (e.KeyChar != (char)8))
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            //�������Ĳ������ּ���Ҳ���ǻس�����Backspace����Ҳ����С���㣬��ȡ��������
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //С����Ĵ���
            if ((int)e.KeyChar == 46)                           //С����
            {
                if (textBox4.Text.Length <= 0)
                    e.Handled = true;   //С���㲻���ڵ�һλ
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
            comboBox1.Items.Insert(0, "----��ѡ��----");
            comboBox1.SelectedIndex = 0;
            //����Ĭ��ֵ
            textBox1.Text = "3";
            textBox3.Text = "50";
            textBox4.Text = "0.3";
            textBox5.Text = "10";
            textBox6.Text = "1000";

            toolStripStatusLabel1.Text = "��ǰϵͳʱ�䣺" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            this.timer1.Interval = 1000;
            this.timer1.Start();

            //IrisSkin4
            //skinEngine1.SkinFile = Application.StartupPath + @"/Skins/mp10.ssk";

        }

        //����γ̱�
        private void PrintSchedule(List<Schedule> res)
        {
            DataTable dt = new DataTable();
            List<List<String>> Arr = new List<List<string>>();
            for (int i = 1; i < 6; i++)
            {
                Arr.Add(new List<string>() { i.ToString(), "", "", "", "", "" });
            }
            //��ͷ
            dt.Columns.Add("week/slot");
            dt.Columns.Add("����һ");
            dt.Columns.Add("���ڶ�");
            dt.Columns.Add("������");
            dt.Columns.Add("������");
            dt.Columns.Add("������");

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
            //ʵ���Զ�����
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
            toolStripStatusLabel1.Text = "��ǰϵͳʱ�䣺" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //�������
            richTextBox1.Text = "";
            dataGridView1.DataSource = null;

            if (textBox1.Text == "")
            {
                MessageBox.Show("�����������Ŀ��");
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("��ѡ���ļ�·����");
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

                //�������
                Schedules = new List<Schedule>(); //ȫ�ֱ�����ʼ��
                comboBox1.Items.Clear();    //���comboBox1�е�����
                comboBox1.Items.Insert(0, "----��ѡ��----");
                comboBox1.SelectedIndex = 0;

                //����excel/csv�ļ� 
                StreamReader reader = new StreamReader(filePath);
                string line = "";
                List<string[]> listStrArr = new List<string[]>();

                line = reader.ReadLine();//��ȡһ������
                while (line != null)
                {
                    listStrArr.Add(line.Split(','));//���ļ����ݷָ������

                    line = reader.ReadLine();
                }
                foreach (var s in listStrArr)
                {
                    //����schedule��   
                    int courseId = Convert.ToInt32(s[0]);
                    int classId = Convert.ToInt32(s[1]);
                    int teacherId = Convert.ToInt32(s[2]);
                    schedules.Add(new Schedule(courseId, classId, teacherId));
                    classIds.Add(classId);
                }

                //�Ż�
                GeneticOptimize ga = new GeneticOptimize(popsize, mutprob, elite, maxiter);
                Schedules = ga.evolution(schedules, roomRange, richTextBox1);

                //comboBox1������
                foreach (var c in classIds.Distinct().ToList())
                {
                    comboBox1.Items.Add(c);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();  //��ʾѡ���ļ��Ի���
            //openFileDialog1.InitialDirectory = "c:\\";
            // openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = openFileDialog1.FileName;   //��ʾ�ļ�·��
                                                                 //ȥtextBox2.Text��·���¶�ȡ��ǰexcel/csv�ļ�
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