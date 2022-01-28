namespace Course_scheduling_optimization
{
    public class GeneticOptimize
    {
        private Random Random => Random.Shared;

        /// <summary>
        /// 种群规模
        /// </summary>
        public int Popsize { get; private set; }

        /// <summary>
        /// 突变的可能性
        /// </summary>
        public double Mutprob { get; private set; }

        /// <summary>
        /// 精英数目
        /// </summary>
        public int Elite { get; private set; }

        /// <summary>
        /// 执行次数
        /// </summary>
        public int Maxiter { get; private set; }

        //默认构造函数
        public GeneticOptimize()
        {
            this.Popsize = 30;
            this.Mutprob = 0.3;
            this.Elite = 5;
            this.Maxiter = 100;
        }

        //构造函数
        public GeneticOptimize(int popsize, double mutprob, int elite, int maxiter)
        {
            this.Popsize = popsize;
            this.Mutprob = mutprob;
            this.Elite = elite;
            this.Maxiter = maxiter;
        }

        //随机初始化不同的种群
        //schedules：List,课程表的人数
        //roomRange:int，教室数
        private List<List<Schedule>> InitPopulation(List<Schedule> schedules, int roomRange)
        {
            List<List<Schedule>> population = new List<List<Schedule>>();
            for (int i = 0; i < Popsize; i++)
            {
                List<Schedule> entity = new List<Schedule>();
                foreach (Schedule s in schedules)
                {
                    s.RandomInit(roomRange);
                    entity.Add(s.DeepClone());     //深层拷贝，备份
                }
                population.Add(entity);
            }
            return population;
        }

        //变异操作，随机对Schedule对象中的某个可改变属性在允许范围内进行随机加减，返回列表，变异后的种群
        //eiltePopulation：List，精英时间表的种群
        //roomRange: int,教室数
        private List<Schedule> Mutate(List<List<Schedule>> eiltePopulation, int roomRange)
        {
            int e = Random.Next(0, Elite);  //elite-精英数目
            int pos = Random.Next(0, 2);
            List<Schedule> ep = new List<Schedule>();    //再次生成Schedule对象
            foreach (var epe in eiltePopulation[e])
            {
                ep.Add(epe.DeepClone());
            }
            foreach (var p in ep)
            {
                pos = Random.Next(0, 3);
                double operation = Random.NextDouble();

                if (pos == 0) p.RoomId = AddSub(p.RoomId, operation, roomRange);
                if (pos == 1) p.WeekDay = AddSub(p.WeekDay, operation, 5);
                if (pos == 2) p.Slot = AddSub(p.Slot, operation, 5);
            }
            return ep;
        }

        private int AddSub(int value, double op, int valueRange)
        {
            if (op > 0.5)
            {
                if (value < valueRange) value += 1;
                else value -= 1;
            }
            else
            {
                if (value - 1 > 0) value -= 1;
                else value += 1;
            }
            return value;
        }

        //交叉操作，随机对两个对象交换不同位置的属性，返回列表，交叉后的种群
        //eiltePopulation:List,精英时间表的种群
        private List<Schedule> Crossover(List<List<Schedule>> eiltePopulation)
        {
            int e1 = Random.Next(0, Elite);
            int e2 = Random.Next(0, Elite);
            int pos = Random.Next(0, 2);

            List<Schedule> ep1 = new List<Schedule>();
            List<Schedule> ep2 = new List<Schedule>();

            foreach (var epe1 in eiltePopulation[e1])
            {
                ep1.Add(epe1.DeepClone());
            }

            ep2 = eiltePopulation[e2];

            for (int i = 0; i < ep1.Count; i++)
            {
                if (pos == 0)
                {
                    ep1[i].WeekDay = ep2[i].WeekDay;
                    ep1[i].Slot = ep2[i].Slot;
                }
                if (pos == 1)
                {
                    ep1[i].RoomId = ep2[i].RoomId;
                }
            }
            return ep1;
        }

        //进化，启动GA算法进行优化，返回最佳结果的索引和最佳冲突的分数
        //schedules:优化课程表
        //elite：int，最佳结果的数目
        public List<Schedule> evolution(List<Schedule> schedules, int roomRange, RichTextBox richTextBox)
        {
            //主循环
            int bestScore = 0;
            List<int> eliteIndex = new List<int>();
            List<Schedule> newp = new List<Schedule>();
            List<Schedule> bestSchedule = new List<Schedule>();
            List<List<Schedule>> population = new List<List<Schedule>>();   //种群

            population = InitPopulation(schedules, roomRange);   //初始化种群

            for (int i = 0; i < Maxiter; i++) //maxiter-执行次数
            {
                List<List<Schedule>> newPopulation = new List<List<Schedule>>();
                Tuple<List<int>, int> scheduleCostRes = ScheduleCost(population, Elite);//新的人口
                eliteIndex = scheduleCostRes.Item1;  //精英指数
                bestScore = scheduleCostRes.Item2;    //冲突最少的冲突数

                richTextBox.Text += String.Format("Iter: {0} | conflict: {1}\n", i + 1, bestScore);
                richTextBox.SelectionStart = richTextBox.TextLength;
                richTextBox.ScrollToCaret();
                //输出冲突

                if (bestScore == 0)
                {
                    richTextBox.Text += "排课完成";
                    bestSchedule = population[eliteIndex[0]];
                    break;
                }

                //从精英开始
                foreach (var ei in eliteIndex)
                {
                    newPopulation.Add(population[ei]);
                }

                //添加精英的变异和繁殖形式
                while (newPopulation.Count < Popsize)
                {
                    if (Random.NextDouble() < Mutprob)   //小于突变可能性
                    {
                        //突变
                        newp = Mutate(newPopulation, roomRange);   //变化
                    }
                    else
                    {
                        //交叉操作
                        newp = Crossover(newPopulation);
                    }
                    newPopulation.Add(newp);
                }
                population = newPopulation;

                if (i == Maxiter - 1)
                {
                    MessageBox.Show("未找到最佳排课结果！");
                }
            }
            return bestSchedule;
        }

        // 计算课表种群的冲突，返回最佳结果的索引，最佳冲突的分数
        // 当被测试课表冲突为0的时候，这个课表就是个符合规定的课表
        // population:课程表的种群
        // elite:最佳结果的数目
        public Tuple<List<int>, int> ScheduleCost(List<List<Schedule>> population, int elite)
        {
            List<int> conflicts = new List<int>();
            List<int> index = new List<int>();
            List<int> bestResultIndex = new List<int>();
            int n = population[0].Count;

            foreach (List<Schedule> p in population)
            {
                int conflict = 0;
                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        //同一个教室在同一个时间只能有一门课
                        if (p[i].RoomId == p[j].RoomId & p[i].WeekDay == p[j].WeekDay & p[i].Slot == p[j].Slot)
                            conflict += 1;
                        //同一个班级在同一个时间只能有一门课
                        if (p[i].ClassId == p[j].ClassId & p[i].WeekDay == p[j].WeekDay & p[i].Slot == p[j].Slot)
                            conflict += 1;
                        //同一个教师在同一个时间只能有一门课
                        if (p[i].TeacherId == p[j].TeacherId & p[i].WeekDay == p[j].WeekDay & p[i].Slot == p[j].Slot)
                            conflict += 1;
                        //同一个班级在同一天不能有相同的课
                        if (p[i].ClassId == p[j].ClassId & p[i].CourseId == p[j].CourseId & p[i].WeekDay == p[j].WeekDay)
                            conflict += 1;
                    }
                }
                conflicts.Add(conflict);
            }
            index = argsort(conflicts);   //返回列表值从小到大的索引值
            for (int i = 0; i < elite; i++)
            {
                bestResultIndex.Add(index[i]);
            }
            return Tuple.Create(bestResultIndex, conflicts[index[0]]);
        }

        //argsort为手动实现Python中的numpy.argsort，返回列表值从小到大的索引值
        public List<int> argsort(List<int> list)
        {
            Dictionary<int, int> indexDict = new Dictionary<int, int>();
            List<int> indexList = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                indexDict.Add(i, list[i]);
            }
            var orderedDict = indexDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach (var key in orderedDict.Keys)
            {
                indexList.Add(key);
            }
            return indexList;
        }
    }
}

