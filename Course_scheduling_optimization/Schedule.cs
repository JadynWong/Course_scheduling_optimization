using System.Text.Json;

namespace Course_scheduling_optimization
{
    [Serializable]
    public class Schedule : ICloneable
    {
        private int courseId;   //课程号
        private int classId;    //班级号
        private int teacherId;  //教师号
        private int roomId = 0; //教室
        private int weekDay = 0;//星期
        private int slot = 0;   //时间

        /// <summary>
        /// 课程号
        /// </summary>
        public int CourseId { get => courseId; set => courseId = value; }

        /// <summary>
        /// 班级号
        /// </summary>
        public int ClassId { get => classId; set => classId = value; }

        /// <summary>
        /// 教师号
        /// </summary>
        public int TeacherId { get => teacherId; set => teacherId = value; }

        /// <summary>
        /// 教室
        /// </summary>
        public int RoomId { get => roomId; set => roomId = value; }

        /// <summary>
        /// 星期
        /// </summary>
        public int WeekDay { get => weekDay; set => weekDay = value; }

        /// <summary>
        /// 时间
        /// </summary>
        public int Slot { get => slot; set => slot = value; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="courseId">课程号</param>
        /// <param name="classId">班级号</param>
        /// <param name="teacherId">教师ID</param>
        public Schedule(int courseId, int classId, int teacherId) //课程表类包含的内容，包括课程号，班级号，教师ID
        {
            this.courseId = courseId;
            this.classId = classId;
            this.teacherId = teacherId;
        }

        #region 拷贝主体
        /// <summary>
        /// 深度拷贝
        /// </summary>
        public Schedule DeepClone()
        {
            //using (Stream objectStream = new MemoryStream())
            //{
            //    IFormatter formatter = new BinaryFormatter();
            //    formatter.Serialize(objectStream, this);
            //    objectStream.Seek(0, SeekOrigin.Begin);
            //    return formatter.Deserialize(objectStream) as Schedule;
            //}
            var json = JsonSerializer.Serialize(this);
            return JsonSerializer.Deserialize<Schedule>(json);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        //随机匹配教室号和时间
        public void RandomInit(int roomRange)
        {
            var random = Random.Shared;
            this.RoomId = random.Next(1, roomRange + 1);
            this.WeekDay = random.Next(1, 6);
            this.Slot = random.Next(1, 6);  //随机生成时间
        }

        public override string ToString()
        {
            return string.Format("课程号：{0}\n班级号：{1}\n教师号：{2}\n房间号：{3}", CourseId, ClassId, TeacherId, RoomId);
        }
    }

}
